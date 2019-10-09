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

namespace JSSoft.UI
{
    [RequireComponent(typeof(Terminal))]
    public partial class TerminalGrid : MaskableGraphic, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
    {
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

        private readonly TerminalRowCollection rows;
        private readonly TerminalCharacterInfoCollection characterInfos;
        private Terminal terminal;
        private bool isSelecting;
        private TerminalPoint startPoint;
        private TerminalPoint endPoint;

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

        public static IEnumerable<TerminalCell> GetVisibleCells(TerminalGrid grid, Func<TerminalCell, bool> predicate)
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

        public static TerminalCell GetCell(TerminalGrid grid, int cursorLeft, int cursorTop)
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

        public static GlyphRect GetCellRect(TerminalGrid grid, TerminalCell cell)
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

        public static bool IsSelecting(TerminalGrid grid, TerminalCell cell)
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
                var p1 = grid.startPoint < grid.endPoint ? grid.startPoint : grid.endPoint;
                var p2 = grid.startPoint > grid.endPoint ? grid.startPoint : grid.endPoint;
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
            var rect = this.rectTransform.rect;
            position.y = rect.height - position.y;
            position.y += GetItemHeight(this) * this.visibleIndex;
            return position;
        }

        public TerminalPoint Intersect(Vector2 position)
        {
            if (this.IntersectWithCell(position) is TerminalCell cell)
            {
                return new TerminalPoint(cell.Index, cell.Row.Index);
            }
            return TerminalPoint.Invalid;
        }

        public TerminalCell IntersectWithCell(Vector2 position)
        {
            foreach (var item in this.rows)
            {
                if (item.Intersect(position) is TerminalCell cell)
                {
                    return cell;
                }
            }
            return null;
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

        public IReadOnlyList<TerminalRow> Rows => this.rows;

        public int VisibleIndex
        {
            get => this.visibleIndex;
            set
            {
                if (value < 0)
                    throw new ArgumentNullException(nameof(value));
                if (this.visibleIndex != value)
                {
                    this.visibleIndex = value;
                    this.UpdateVisibleIndex();
                    this.OnVisibleIndexChanged(EventArgs.Empty);
                }
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
            Debug.Log($"{nameof(TerminalGrid)}.{nameof(OnValidate)}");
            this.SetVerticesDirty();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.UpdateGrid();
            this.UpdateVisibleIndex();
            this.UpdateRows();
            this.UpdateCursorPosition();
            this.SetVerticesDirty();
            this.OnTextChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
            this.OnCompositionStringChanged(EventArgs.Empty);
        }

        // protected override void OnPopulateMesh(VertexHelper vh)
        // {
        //     base.OnPopulateMesh(vh);
        // }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("TextMeshPro/Bitmap"));
            this.material.color = base.color;
            this.AttachEvent();
            this.SetVerticesDirty();
            Debug.Log($"{nameof(TerminalGrid)}.{nameof(OnEnable)}");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.DetachEvent();
            Debug.Log($"{nameof(TerminalGrid)}.{nameof(OnDisable)}");
        }

        private void UpdateGrid()
        {
            if (this.fontAsset != null)
            {
                var rect = this.rectTransform.rect;
                var fontAsset = this.fontAsset;
                var itemWidth = FontUtility.GetItemWidth(fontAsset);
                var itemHeight = FontUtility.GetItemHeight(fontAsset);
                this.ColumnCount = (int)(rect.width / itemWidth);
                this.RowCount = (int)(rect.height / itemHeight);
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
            Debug.Log(1);
            this.Text = this.Terminal.Text;
            Debug.Log(2);
        }

        private void Terminal_PromptTextChanged(object sender, EventArgs e)
        {
            this.Text = this.Terminal.Text;
            this.VisibleIndex = int.MaxValue;
        }

        private void Terminal_CursorPositionChanged(object sender, EventArgs e)
        {
            var index = this.Terminal.CursorPosition + this.Terminal.OutputText.Length + this.Terminal.Prompt.Length;
            this.CursorPosition = this.IndexToPoint(index);
            this.VisibleIndex = int.MaxValue;
        }

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var i = p1.X + p1.Y * this.ColumnCount;
            var end = p2.X + p2.Y * this.ColumnCount;
            while (i <= end)
            {
                var x = i % this.ColumnCount;
                var y = i / this.ColumnCount;
                yield return this.rows[y].Cells[x];
                i++;
            }
        }

        private IEnumerable<TerminalCell> GetCells()
        {
            var query = from row in this.rows
                        from cell in row.Cells
                        select cell;
            return query;
        }

        #region implemetations

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.endPoint = this.Intersect(position);
                this.isSelecting = true;
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                var endPoint = this.Intersect(position);
                if (this.endPoint != endPoint)
                {
                    this.endPoint = endPoint;
                    this.OnLayoutChanged(EventArgs.Empty);
                }
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            this.isSelecting = false;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                var s1 = this.startPoint;
                var s2 = this.Intersect(position);
                var p1 = s1 < s2 ? s1 : s2;
                var p2 = s1 > s2 ? s1 : s2;
                foreach (var item in this.GetCells(p1, p2))
                {
                    item.IsSelected = !item.IsSelected;
                }
                this.startPoint = TerminalPoint.Invalid;
                this.endPoint = TerminalPoint.Invalid;
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                if (this.IntersectWithCell(position) is TerminalCell cell)
                {
                    // Debug.Log($"TextIndex: {cell.TextIndex}");
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.startPoint = this.Intersect(position);
            }
        }

        #endregion
    }
}
