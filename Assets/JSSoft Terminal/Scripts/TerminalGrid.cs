// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.ObjectModel;
using UnityEngine.TextCore;
using KeyBinding = JSSoft.UI.KeyBinding<JSSoft.UI.TerminalGrid>;

namespace JSSoft.UI
{
    [RequireComponent(typeof(Terminal))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [SelectionBase]
    public partial class TerminalGrid : MaskableGraphic,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler
    {
        private static readonly Dictionary<string, IKeyBinding> bindingByKey = new Dictionary<string, IKeyBinding>();

        [SerializeField]
        private TMP_FontAsset fontAsset = null;
        [SerializeField]
        [TextArea(5, 10)]
        public string text = string.Empty;
        [SerializeField]
        private Color32 fontColor = TerminalColors.White;
        [SerializeField]
        private int visibleIndex;
        [SerializeField]
        private TerminalPoint cursorPosition;
        [SerializeField]
        private bool isCursorVisible = true;
        [SerializeField]
        private string compositionString = string.Empty;

        private readonly TerminalEventCollection events = new TerminalEventCollection();
        private readonly TerminalRowCollection rows;
        private readonly TerminalCharacterInfoCollection characterInfos;

        private Terminal terminal;
        private bool isSelecting;
        private TerminalPoint downPoint;
        private TerminalPoint beginPoint;
        private TerminalPoint endPoint;
        private bool isFocused;
        private bool hasSelection;

        // 전체적으로 왜 키 이벤트 호출시에 EventModifiers.FunctionKey 가 기본적으로 설정되어 있는지 모르겠음.
        static TerminalGrid()
        {

        }

        public TerminalGrid()
        {
            this.rows = new TerminalRowCollection(this);
            this.characterInfos = new TerminalCharacterInfoCollection(this);
        }

        public static Rect TransformRect(TerminalGrid grid, Rect rect)
        {
            if (grid != null && grid.fontAsset != null && grid.rows.Count > 0)
            {
                var itemHeight = FontUtility.GetItemHeight(grid.fontAsset);
                rect.y += itemHeight * grid.visibleIndex;
            }
            return rect;
        }

        public static IEnumerable<ITerminalCell> GetVisibleCells(TerminalGrid grid, Func<ITerminalCell, bool> predicate)
        {
            if (grid != null)
            {
                var topIndex = grid.visibleIndex;
                var bottomIndex = topIndex + grid.RowCount;
                var query = from row in grid.Rows
                            where row.Index >= topIndex && row.Index < bottomIndex
                            from cell in row.Cells
                            where predicate(cell)
                            select cell;
                return query;
            }
            return Enumerable.Empty<TerminalCell>();
        }

        public static ITerminalCell GetCell(TerminalGrid grid, int cursorLeft, int cursorTop)
        {
            if (grid != null)
            {
                if (cursorTop >= grid.Rows.Count)
                    throw new ArgumentOutOfRangeException(nameof(cursorTop));
                if (cursorLeft >= grid.ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(cursorLeft));
                return grid.Rows[cursorTop].Cells[cursorLeft];
            }
            return null;
        }

        public static int GetItemWidth(TerminalGrid grid)
        {
            if (grid != null && grid.fontAsset != null)
            {
                return FontUtility.GetItemWidth(grid.fontAsset);
            }
            return 0;
        }

        public static int GetItemHeight(TerminalGrid grid)
        {
            if (grid != null && grid.fontAsset != null)
            {
                return FontUtility.GetItemHeight(grid.fontAsset);
            }
            return 0;
        }

        public static GlyphRect GetCellRect(TerminalGrid grid, ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var itemWidth = TerminalGrid.GetItemWidth(grid);
            var itemHeight = TerminalGrid.GetItemHeight(grid);
            var x = cell.Index * itemWidth;
            var y = cell.Row.Index * itemHeight;
            return new GlyphRect(x, y, itemWidth, itemHeight);
        }

        public static int GetItemWidth(TerminalGrid grid, char character)
        {
            if (grid != null && grid.fontAsset != null)
            {
                return FontUtility.GetItemWidth(grid.fontAsset, character);
            }
            return 0;
        }

        public static bool IsSelecting(TerminalGrid grid, ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var point = new TerminalPoint(cell.Index, cell.Row.Index);
            var isSelecting = IsSelecting(grid, point);
            return cell.IsSelected != isSelecting;
        }

        public static bool IsSelecting(TerminalGrid grid, TerminalPoint point)
        {
            if (grid != null && grid.isSelecting == true)
            {
                var p1 = grid.beginPoint < grid.endPoint ? grid.beginPoint : grid.endPoint;
                var p2 = grid.beginPoint > grid.endPoint ? grid.beginPoint : grid.endPoint;
                return point >= p1 && point <= p2;
            }
            return false;
        }

        public static Color32 GetBackgroundColor(TerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            return grid.BackgroundColor ?? grid.color;
        }

        public static Color32 GetForegroundColor(TerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            return grid.ForegroundColor ?? grid.fontColor;
        }

        public Vector2 WorldToGrid(Vector2 position)
        {
            var rect = this.GetComponent<RectTransform>().rect;
            position.y = rect.height - position.y;
            position.y += GetItemHeight(this) * this.visibleIndex;
            return position;
        }

        public TerminalPoint Intersect(Vector2 position)
        {
            foreach (var item in this.rows)
            {
                var point = item.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    return point;
                }
            }
            return TerminalPoint.Invalid;
        }

        public ITerminalCell IntersectWithCell(Vector2 position)
        {
            foreach (var item in this.rows)
            {
                if (item.IntersectWithCell(position) is TerminalCell cell)
                {
                    return cell;
                }
            }
            return null;
        }

        public void ClearSelection()
        {
            if (this.hasSelection == true)
            {
                foreach (var item in this.rows)
                {
                    item.ClearSelection();
                }
                this.hasSelection = false;
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public TerminalPoint IndexToPoint(int index) => this.characterInfos[index].Point;

        public int PointToIndex(TerminalPoint point) => this.characterInfos.PointToIndex(point);

        public Color32? IndexToBackgroundColor(int index) => this.Terminal.GetBackgroundColor(index);

        public Color32? IndexToForegroundColor(int index) => this.Terminal.GetForegroundColor(index);

        public Terminal Terminal
        {
            get
            {
                if (this.terminal == null)
                {
                    this.terminal = this.GetComponent<Terminal>();
                }
                return this.terminal;
            }
        }

        public void Append(string text)
        {
            this.Append(text, this.text.Length);
        }

        public void Append(string text, int index)
        {
            this.text = this.text.Insert(index, text);
            this.UpdateRows();
            this.OnTextChanged(EventArgs.Empty);
        }

        public void Remove(int startIndex, int length)
        {
            this.text.Remove(startIndex, length);
            this.UpdateRows();
            this.OnTextChanged(EventArgs.Empty);
        }

        public bool IsFocused
        {
            get => this.isFocused;
            set
            {
                if (this.isFocused == value)
                    return;
                this.isFocused = value;
                if (this.isFocused == true)
                    this.OnGotFocus(EventArgs.Empty);
                else
                    this.OnLostFocus(EventArgs.Empty);
            }
        }

        public string Text
        {
            get => this.text;
            set
            {
                var newValue = value ?? throw new ArgumentNullException(nameof(value));
                var oldValue = this.text;
                var minLength = Math.Min(newValue.Length, oldValue.Length);
                var index = minLength;

                for (var i = 0; i < minLength; i++)
                {
                    if (newValue[i] != oldValue[i])
                    {
                        index = i;
                        break;
                    }
                }

                this.text = newValue;
                this.UpdateRows();
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        public TMP_FontAsset FontAsset => this.fontAsset;

        public int ColumnCount { get; private set; }

        public int RowCount { get; private set; }

        public IReadOnlyList<ITerminalRow> Rows => this.rows;

        public int VisibleIndex
        {
            get => this.visibleIndex;
            set
            {
                if (value < 0 || value > this.MaximumVisibleIndex)
                    throw new ArgumentNullException(nameof(value));
                if (this.visibleIndex != value)
                {
                    this.visibleIndex = value;
                    this.UpdateVisibleIndex();
                    this.OnVisibleIndexChanged(EventArgs.Empty);
                }
            }
        }

        public int MaximumVisibleIndex
        {
            get
            {
                if (this.visibleIndex < 0 || this.Rows.Count < this.RowCount)
                    return 0;
                return Math.Min(this.visibleIndex, this.Rows.Count - this.RowCount);
            }
        }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public TerminalPoint CursorPosition
        {
            get => this.cursorPosition;
            set
            {
                if (this.cursorPosition != value)
                {
                    this.cursorPosition = value;
                    this.UpdateCursorPosition();
                    this.OnCursorPositionChanged(EventArgs.Empty);
                }
                this.ClearSelection();
            }
        }

        public bool IsCursorVisible
        {
            get => this.isCursorVisible;
            set
            {
                this.isCursorVisible = value;
                this.OnCursorPositionChanged(EventArgs.Empty);
            }
        }

        public string CompositionString
        {
            get => this.compositionString;
            set
            {
                this.compositionString = value ?? throw new ArgumentNullException(nameof(value));
                this.OnCompositionStringChanged(EventArgs.Empty);
            }
        }

        public event EventHandler TextChanged;

        public event EventHandler VisibleIndexChanged;

        public event EventHandler LayoutChanged;

        public event EventHandler SelectionChanged;

        public event EventHandler CursorPositionChanged;

        public event EventHandler CompositionStringChanged;

        public event EventHandler GotFocus;

        public event EventHandler LostFocus;

        protected virtual void OnTextChanged(EventArgs e)
        {
            this.TextChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLayoutChanged(EventArgs e)
        {
            this.LayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnVisibleIndexChanged(EventArgs e)
        {
            this.VisibleIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCursorPositionChanged(EventArgs e)
        {
            this.CursorPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCompositionStringChanged(EventArgs e)
        {
            this.CompositionStringChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGotFocus(EventArgs e)
        {
            this.GotFocus?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLostFocus(EventArgs e)
        {
            this.LostFocus?.Invoke(this, EventArgs.Empty);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.UpdateGrid();
            this.UpdateVisibleIndex();
            this.UpdateRows();
            this.UpdateCursorPosition();
            this.OnTextChanged(EventArgs.Empty);
            this.OnVisibleIndexChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
            this.OnCompositionStringChanged(EventArgs.Empty);
            // Debug.Log($"{nameof(TerminalGrid)}.{nameof(OnValidate)}");
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.UpdateGrid();
            this.UpdateVisibleIndex();
            this.UpdateRows();
            this.UpdateCursorPosition();
            this.OnTextChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
            this.OnCompositionStringChanged(EventArgs.Empty);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // this.material = new Material(Shader.Find("TextMeshPro/Bitmap"));
            // this.material.color = base.color;
            this.AttachEvent();
            // this.SetVerticesDirty();
            // Debug.Log($"{nameof(TerminalGrid)}.{nameof(OnEnable)}");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.DetachEvent();
            // Debug.Log($"{nameof(TerminalGrid)}.{nameof(OnDisable)}");
        }

        protected virtual bool OnPreviewKeyDown(KeyCode keyCode, EventModifiers modifiers)
        {
            if (this.Terminal.ProcessKeyEvent(keyCode, modifiers) == true)
                return true;
            var key = $"{modifiers}+{keyCode}";
            if (bindingByKey.ContainsKey(key) == true)
            {
                var binding = bindingByKey[key];
                if (binding.Verify(this) == true && binding.Action(this) == true)
                    return true;
            }
            return false;
        }

        protected virtual bool OnPreviewKeyPress(char character)
        {
            if (character == '\n')
                return true;
            return false;
        }

        protected virtual void OnTranslateEvents(IList<Event> eventList)
        {
            for (var i = 0; i < eventList.Count; i++)
            {
                var item = eventList[i];
                // ime 에 의해서 문자가 조합중일때 enter 키 입력시 이벤트가 두번 호출되는 어이없는 상황을 하드코딩으로 방어
                if (item.rawType == EventType.KeyDown &&
                    item.keyCode == KeyCode.Return &&
                    item.modifiers == EventModifiers.None &&
                    i + 1 < eventList.Count)
                {
                    var nextItem = eventList[i + 1];
                    if (nextItem.character != '\n')
                    {
                        eventList[i] = null;
                    }
                }
            }
        }

        private void UpdateGrid()
        {
            if (this.fontAsset != null)
            {
                var rect = this.GetComponent<RectTransform>().rect;
                var fontAsset = this.fontAsset;
                var itemWidth = FontUtility.GetItemWidth(fontAsset);
                var itemHeight = FontUtility.GetItemHeight(fontAsset);
                this.ColumnCount = (int)(rect.width / itemWidth);
                this.RowCount = (int)(rect.height / itemHeight);
                // this.startPoint = new TerminalPoint(0, 0);
                // this.endPoint = new TerminalPoint(this.ColumnCount, this.RowCount);
            }
            else
            {
                this.ColumnCount = 0;
                this.RowCount = 0;
            }
        }

        private void UpdateVisibleIndex()
        {
            if (this.visibleIndex < 0 || this.Rows.Count < this.RowCount)
                this.visibleIndex = 0;
            else
                this.visibleIndex = Math.Min(this.visibleIndex, this.Rows.Count - this.RowCount);
        }

        private void UpdateCursorPosition()
        {
            var x = this.cursorPosition.X;
            var y = this.cursorPosition.Y;
            var maxRowCount = Math.Max(this.RowCount, this.rows.Count);
            x = Math.Min(x, this.ColumnCount - 1);
            x = Math.Max(x, 0);
            y = Math.Min(y, maxRowCount - 1);
            y = Math.Max(y, 0);
            this.cursorPosition = new TerminalPoint(x, y);
        }

        private void UpdateRows()
        {
            this.characterInfos.Update();
            this.rows.Udpate(this.characterInfos);
        }

        private void AttachEvent()
        {
            this.Terminal.OutputTextChanged += Terminal_OutputTextChanged;
            this.Terminal.PromptTextChanged += Terminal_PromptTextChanged;
            this.Terminal.CursorPositionChanged += Terminal_CursorPositionChanged;
        }

        private void DetachEvent()
        {
            this.Terminal.OutputTextChanged -= Terminal_OutputTextChanged;
            this.Terminal.PromptTextChanged -= Terminal_PromptTextChanged;
            this.Terminal.CursorPositionChanged -= Terminal_CursorPositionChanged;
        }

        private void Terminal_OutputTextChanged(object sender, EventArgs e)
        {
            this.Text = this.Terminal.Text;
        }

        private void Terminal_PromptTextChanged(object sender, EventArgs e)
        {
            this.Text = this.Terminal.Text;
            this.VisibleIndex = this.MaximumVisibleIndex;
        }

        private void Terminal_CursorPositionChanged(object sender, EventArgs e)
        {
            var index = this.Terminal.CursorPosition + this.Terminal.OutputText.Length + this.Terminal.Prompt.Length;
            this.CursorPosition = this.IndexToPoint(index);
            this.VisibleIndex = this.MaximumVisibleIndex;
        }

        private static void AddBinding(IKeyBinding binding)
        {
            bindingByKey.Add($"{binding.Modifiers}+{binding.KeyCode}", binding);
        }

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var s1 = p1;
            var s2 = p2;
            var x = s1.X;
            var y = s1.Y;
            for (; y <= s2.Y; y++)
            {
                var columnCount = y == s2.Y ? s2.X : this.ColumnCount;
                if (columnCount >= this.ColumnCount)
                {
                    columnCount = this.ColumnCount - 1;
                }
                for (; x <= columnCount; x++)
                {
                    yield return this.rows[y].Cells[x];
                }
                x = 0;
            }
        }

        private void Select(TerminalPoint p1, TerminalPoint p2)
        {
            foreach (var item in this.GetCells(p1, p2))
            {
                item.IsSelected = !item.IsSelected;
            }
        }

        private (TerminalPoint s1, TerminalPoint s2) UpdatePoint(TerminalPoint p1, TerminalPoint p2)
        {
            var s1 = p1 < p2 ? p1 : p2;
            var s2 = p1 > p2 ? p1 : p2;

            var cell1 = this.rows[s1.Y].Cells[s1.X];
            var cell2 = this.rows[s2.Y].Cells[s2.X];
            if (cell1.IsEnabled == true && cell2.IsEnabled == true)
            {
                return (s1, s2);
            }
            else if (cell1.IsEnabled == true)
            {
                var row2 = this.rows[s2.Y];
                if (row2.IsEmpty == true)
                {
                    s2.X = this.ColumnCount;
                }
                else
                {
                    var cell = row2.Cells[s2.X];
                    if (cell.IsEnabled == false)
                    {
                        s2.X = this.ColumnCount;
                    }
                }
            }
            else if (cell2.IsEnabled == true)
            {
                var row1 = this.rows[s1.Y];
                if (row1.IsEmpty == true)
                {
                    s1.X = this.ColumnCount;
                }
                else
                {
                    var cell = row1.Cells[s1.X];
                    if (cell.IsEnabled == false)
                    {
                        var j = Math.Max(0, s1.X - 6);
                        for (var i = s1.X; i >= j; i--)
                        {
                            cell = row1.Cells[i];
                            if (cell.IsEnabled == true)
                            {
                                s1.X = i + 1;
                                break;
                            }
                        }
                    }
                }
            }

            return (s1, s2);
        }

        private IEnumerable<TerminalCell> GetCells()
        {
            var query = from row in this.rows
                        from cell in row.Cells
                        select cell;
            return query;
        }

        private BaseInput InputSystem
        {
            get
            {
                if (EventSystem.current && EventSystem.current.currentInputModule)
                    return EventSystem.current.currentInputModule.input;
                return null;
            }
        }

        #region implemetations

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            this.InputSystem.imeCompositionMode = IMECompositionMode.On;
            this.IsFocused = true;
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            this.IsFocused = false;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                (this.beginPoint, this.endPoint) = this.UpdatePoint(this.downPoint, point);
                this.isSelecting = true;
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                (this.beginPoint, this.endPoint) = this.UpdatePoint(this.downPoint, point);
                this.OnLayoutChanged(EventArgs.Empty);
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            this.isSelecting = false;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                var (beginPoint, endPoint) = this.UpdatePoint(this.downPoint, point);
                this.downPoint = TerminalPoint.Invalid;
                this.beginPoint = TerminalPoint.Invalid;
                this.endPoint = TerminalPoint.Invalid;
                this.hasSelection = true;
                this.Select(beginPoint, endPoint);
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                // Debug.Log($"Intersect: {point}");
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.downPoint = this.Intersect(position); ;
                this.ClearSelection();
                eventData.useDragThreshold = false;
            }
            eventData.selectedObject = this.gameObject;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("enter");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("exit");
        }

        void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
        {
            if (this.events.PopEvents() == false)
                return;

            this.OnTranslateEvents(events);
            for (var i = 0; i < events.Count; i++)
            {
                var item = events[i];
                if (item == null)
                    continue;
                if (item.rawType == EventType.KeyDown)
                {
                    var keyCode = item.keyCode;
                    var modifiers = item.modifiers;
                    if (this.OnPreviewKeyDown(keyCode, modifiers) == true)
                        continue;
                    if (item.character != 0 && this.OnPreviewKeyPress(item.character) == false)
                    {
                        this.Terminal.InsertCharacter(item.character);
                    }
                    else
                    {
                        this.CompositionString = this.InputSystem != null ? this.InputSystem.compositionString : Input.compositionString;
                    }
                }
            }

            eventData.Use();
        }

        #endregion
    }
}
