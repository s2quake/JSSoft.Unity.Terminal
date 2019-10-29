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
using UnityEngine.TextCore;
using KeyBinding = JSSoft.UI.KeyBinding<JSSoft.UI.TerminalGrid>;

namespace JSSoft.UI
{
    [RequireComponent(typeof(Terminal))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [SelectionBase]
    class TerminalGrid : MaskableGraphic, ITerminalGrid,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler
    {
        public static readonly Color32 DefaultSelectionColor = new Color32(49, 79, 129, 255);
        public static readonly Color32 DefaultBackgroundColor = new Color32(23, 23, 23, 255);
        public static readonly Color32 DefaultForegroundColor = new Color32(255, 255, 255, 255);
        public static readonly Color32 DefaultCursorColor = new Color32(139, 139, 139, 255);
        private static KeyBindingCollection keyBindings;

        [SerializeField]
        private TMP_FontAsset fontAsset = null;
        [SerializeField]
        [TextArea(5, 10)]
        public string text = string.Empty;
        [SerializeField]
        private Color32 fontColor = DefaultForegroundColor;
        [SerializeField]
        private Color32 selectionColor = DefaultSelectionColor;
        [SerializeField]
        private Color32 cursorColor = DefaultCursorColor;
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
        private readonly List<TerminalPoint> selectionList = new List<TerminalPoint>();

        private Terminal terminal;
        private bool isSelecting;
        private bool isFocused;
        private bool isScrolling;
        private TerminalPoint downPoint;
        private TerminalPoint beginPoint;
        private TerminalPoint endPoint;
        private float scrollPos;
        private Rect rectangle;
        private Vector2 itemSize;

        public TerminalGrid()
        {
            this.rows = new TerminalRowCollection(this);
            this.characterInfos = new TerminalCharacterInfoCollection(this);
        }

        public static bool IsSelecting(TerminalGrid grid, ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var point = new TerminalPoint(cell.Index, cell.Row.Index);
            var isSelecting = IsSelecting(grid, point);
            return IsSelected(grid, point) != isSelecting;
        }

        public static bool IsSelecting(TerminalGrid grid, TerminalPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grid.isSelecting == true)
            {
                var p1 = grid.beginPoint < grid.endPoint ? grid.beginPoint : grid.endPoint;
                var p2 = grid.beginPoint > grid.endPoint ? grid.beginPoint : grid.endPoint;
                return point >= p1 && point <= p2;
            }
            return false;
        }

        public static bool IsSelected(TerminalGrid grid, TerminalPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grid.selectionList.Any())
            {
                var p1 = grid.selectionList.First();
                var p2 = grid.selectionList.Last();
                return point >= p1 && point <= p2;
            }
            return false;
        }

        public Vector2 WorldToGrid(Vector2 position)
        {
            var rect = this.GetComponent<RectTransform>().rect;
            position.y = rect.height - position.y;
            position.y += TerminalGridUtility.GetItemHeight(this) * this.visibleIndex;
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
            if (this.selectionList.Any() == true)
            {
                this.selectionList.Clear();
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public TerminalPoint IndexToPoint(int index) => this.characterInfos[index].Point;

        public int PointToIndex(TerminalPoint point) => this.characterInfos.PointToIndex(point);

        public Color32? IndexToBackgroundColor(int index) => this.Terminal.GetBackgroundColor(index);

        public Color32? IndexToForegroundColor(int index) => this.Terminal.GetForegroundColor(index);

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

        public void ScrollToTop()
        {
            if (this.MaximumVisibleIndex > 0)
            {
                this.isScrolling = true;
                this.VisibleIndex = 0;
                this.isScrolling = false;
            }
        }

        public void ScrollToBottom()
        {
            if (this.MaximumVisibleIndex > 0)
            {
                this.isScrolling = true;
                this.VisibleIndex = this.MaximumVisibleIndex;
                this.isScrolling = false;
            }
        }

        public void PageUp()
        {
            this.Scroll(-this.RowCount);
        }

        public void PageDown()
        {
            this.Scroll(this.RowCount);
        }

        public void LineUp()
        {
            this.Scroll(-1);
        }

        public void LineDown()
        {
            this.Scroll(1);
        }

        public void Scroll(int value)
        {
            var visibleIndex = this.VisibleIndex + value;
            if (visibleIndex >= 0 && visibleIndex <= this.MaximumVisibleIndex)
            {
                this.isScrolling = true;
                this.VisibleIndex = visibleIndex;
                this.isScrolling = false;
            }
        }

        public void Copy()
        {
            if (this.selectionList.Any() == true)
            {
                var p1 = this.selectionList.First();
                var p2 = this.selectionList.Last();
                var capacity = p1.DistanceOf(p2, this.ColumnCount);
                var list = new List<char>(capacity);
                var i1 = this.characterInfos.PointToIndex(p1);
                var i2 = this.characterInfos.PointToIndex(p2);
                for (var i = i1; i <= i2; i++)
                {
                    var item = this.characterInfos[i];
                    if (item.Character != char.MinValue)
                    {
                        list.Add(item.Character);
                    }
                }
                ClipboardUtility.Text = new string(list.ToArray());
            }
            else
            {
                ClipboardUtility.Text = string.Empty;
            }
        }

        public void SelectAll()
        {
            this.selectionList.Clear();
            this.selectionList.Add(new TerminalPoint(0, 0));
            this.selectionList.Add(new TerminalPoint(this.ColumnCount, this.rows.Count));
            this.OnSelectionChanged(EventArgs.Empty);
        }

        public static KeyBindingCollection KeyBindings
        {
            get
            {
                if (keyBindings == null)
                {
                    if (TerminalEnvironment.IsMac == true)
                        return TerminalKeyBindings.Mac;
                    else if (TerminalEnvironment.IsWindows == true)
                        return TerminalKeyBindings.Windows;
                }
                return keyBindings;
            }
            set
            {
                keyBindings = value;
            }
        }

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
                    this.scrollPos = this.visibleIndex;
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
                return this.Rows.Count - this.RowCount;
            }
        }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public Color32 SelectionColor
        {
            get => this.selectionColor;
            set
            {
                this.selectionColor = value;
                this.OnLayoutChanged(EventArgs.Empty);
            }
        }

        public Color32 CursorColor
        {
            get => this.cursorColor;
            set
            {
                this.cursorColor = value;
                this.OnLayoutChanged(EventArgs.Empty);
            }
        }

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

        public Rect Rectangle => this.rectangle;

        public bool IsCursorVisible
        {
            get => this.isCursorVisible;
            set
            {
                this.isCursorVisible = value;
                this.OnCursorPositionChanged(EventArgs.Empty);
            }
        }

        public bool IsScrolling => this.isScrolling;

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
            this.AttachEvent();
            this.VisibleIndex = this.MaximumVisibleIndex;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.DetachEvent();
        }

        protected virtual bool OnPreviewKeyDown(EventModifiers modifiers, KeyCode keyCode)
        {
            if (this.Terminal.ProcessKeyEvent(modifiers, keyCode) == true)
                return true;
            return KeyBindings.Process(this, modifiers, keyCode);
        }

        protected virtual bool OnPreviewKeyPress(char character)
        {
            // 27: escape key
            if (character == '\n' || character == 27)
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
            var rect = this.GetComponent<RectTransform>().rect;
            if (this.fontAsset != null)
            {
                var fontAsset = this.fontAsset;
                var itemWidth = FontUtility.GetItemWidth(fontAsset);
                var itemHeight = FontUtility.GetItemHeight(fontAsset);
                var rectWidth = itemWidth * this.ColumnCount;
                var rectHeight = itemHeight * this.RowCount;
                this.itemSize = new Vector2(itemWidth, itemHeight);
                this.ColumnCount = (int)(rect.width / itemWidth);
                this.RowCount = (int)(rect.height / itemHeight);
                this.rectangle.x = (int)((rect.width - rectWidth) / 2);
                this.rectangle.y = (int)((rect.height - rectHeight) / 2);
                this.rectangle.width = rectWidth;
                this.rectangle.height = rectHeight;
            }
            else
            {
                this.itemSize = new Vector2(0, 0);
                this.ColumnCount = 0;
                this.RowCount = 0;
                this.rectangle.x = (int)(rect.width / 2);
                this.rectangle.y = (int)(rect.height / 2);
                this.rectangle.width = 0 * this.ColumnCount;
                this.rectangle.height = 0 * this.RowCount;
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

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var (s1, s2) = p1 < p2 ? (p1, p2) : (p2, p1);
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

        private (TerminalPoint s1, TerminalPoint s2) UpdatePoint(TerminalPoint p1, TerminalPoint p2)
        {
            var (s1, s2) = p1 < p2 ? (p1, p2) : (p2, p1);
            var cell1 = this.rows[s1.Y].Cells[s1.X];
            var cell2 = this.rows[s2.Y].Cells[s2.X];
            var row1 = cell1.Row;
            var row2 = cell2.Row;
            var columnCount = this.ColumnCount;
            var gap = 5;
            if (cell1.IsEnabled == true && cell2.IsEnabled == true)
            {
                return (s1, s2);
            }
            else if (cell1.IsEnabled == true)
            {
                var l2 = row2.LastPoint(false);
                var distance = l2.DistanceOf(s2, columnCount);
                s2.X = distance > gap ? columnCount : l2.X;
            }
            else if (cell2.IsEnabled == true)
            {
                var l1 = row1.LastPoint(true);
                var distance = l1.DistanceOf(s1, columnCount);
                s1.X = distance > gap ? columnCount : l1.X;
            }
            else
            {
                if (row1.IsEmpty == false)
                {
                    var l1 = row1.LastPoint(false);
                    var distance = l1.DistanceOf(s1, columnCount);
                    s1.X = distance > gap ? columnCount : l1.X;
                }
                else
                {
                    s1.X = columnCount;
                }

                if (row2.IsEmpty == false)
                {
                    var l2 = row2.LastPoint(false);
                    var distance = l2.DistanceOf(s2, columnCount);
                    s2.X = distance > gap ? columnCount : l2.X;
                }
                else
                {
                    s2.X = columnCount;
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
            if (eventData.button == PointerEventData.InputButton.Left && this.downPoint != TerminalPoint.Invalid)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    var (beginPoint, endPoint) = this.UpdatePoint(this.downPoint, point);
                    this.beginPoint = beginPoint;
                    this.endPoint = endPoint;
                    this.isSelecting = true;
                }
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && this.downPoint != TerminalPoint.Invalid)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    var (beginPoint, endPoint) = this.UpdatePoint(this.downPoint, point);
                    this.beginPoint = beginPoint;
                    this.endPoint = endPoint;
                    this.OnLayoutChanged(EventArgs.Empty);
                }
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && this.downPoint != TerminalPoint.Invalid)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    var (beginPoint, endPoint) = this.UpdatePoint(this.downPoint, point);
                    this.beginPoint = beginPoint;
                    this.endPoint = endPoint;
                }
                this.selectionList.Clear();
                this.selectionList.Add(beginPoint);
                this.selectionList.Add(endPoint);
                this.selectionList.Sort();
                this.downPoint = TerminalPoint.Invalid;
                this.beginPoint = TerminalPoint.Invalid;
                this.endPoint = TerminalPoint.Invalid;
                this.isSelecting = false;
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            // if (eventData.button == PointerEventData.InputButton.Left)
            // {
            //     var position = this.WorldToGrid(eventData.position);
            //     var point = this.Intersect(position);
            //     Debug.Log($"Intersect: {point}");
            // }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.downPoint = this.Intersect(position);
                if (this.downPoint != TerminalPoint.Invalid)
                {
                    this.ClearSelection();
                }
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

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (this.MaximumVisibleIndex > 0)
            {
                this.scrollPos -= eventData.scrollDelta.y;
                this.scrollPos = Math.Max((int)this.scrollPos, 0);
                this.scrollPos = Math.Min((int)this.scrollPos, this.MaximumVisibleIndex);
                this.visibleIndex = (int)this.scrollPos;
                this.isScrolling = true;
                this.UpdateVisibleIndex();
                this.OnVisibleIndexChanged(EventArgs.Empty);
                this.isScrolling = false;
            }
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
                    if (this.OnPreviewKeyDown(modifiers, keyCode) == true)
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

        ITerminal ITerminalGrid.Terminal => this.terminal;

        #endregion
    }
}
