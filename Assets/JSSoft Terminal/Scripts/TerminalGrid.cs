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
    public class TerminalGrid : MaskableGraphic, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
    {
        [SerializeField]
        private TMP_FontAsset fontAsset;
        [SerializeField]
        [TextArea(5, 10)]
        private string text = string.Empty;
        [SerializeField]
        private Color32 fontColor = TerminalColors.White;
        [SerializeField]
        private int visibleIndex;
        [SerializeField]
        private Scrollbar verticalScrollbar;
        [SerializeField]
        private TerminalPoint cursorPostion;

        private readonly List<TerminalRow> rowList = new List<TerminalRow>();
        private bool isUpdating;
        private bool isSelecting;
        private TerminalPoint startPoint;
        private TerminalPoint endPoint;

        public TerminalGrid()
        {

        }

        public static Rect TransformRect(TerminalGrid grid, Rect rect)
        {
            if (grid != null && grid.fontAsset != null && grid.rowList.Count > 0)
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

        private static TerminalRow[] GenerateTerminalRows(TerminalGrid grid, string text)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var s = text.Replace("\r\n", "\n");
            var fontAsset = grid.FontAsset;
            var i = 0;
            var rowList = new List<TerminalRow>();
            while (i < s.Length)
            {
                var row = new TerminalRow(grid, rowList.Count);
                FillString(fontAsset, row, s, ref i);
                rowList.Add(row);
            }
            return rowList.ToArray();
        }

        private static void FillString(TMP_FontAsset fontAsset, TerminalRow row, string text, ref int i)
        {
            var columnIndex = 0;
            var cellCount = row.Cells.Count;
            while (columnIndex < cellCount && i < text.Length)
            {
                var character = text[i];
                if (character == '\n')
                {
                    i++;
                    break;
                }
                var cell = row.Cells[columnIndex];
                var volume = FontUtility.GetCharacterVolume(fontAsset, character);
                if (columnIndex + volume > cellCount)
                    break;
                cell.Character = character;
                cell.TextIndex = i;
                columnIndex += volume;
                i++;
            }
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
            foreach (var item in this.rowList)
            {
                if (item.Intersect(position) is TerminalCell cell)
                {
                    return cell;
                }
            }
            return null;
        }

        public void Append(string text)
        {
            this.Append(text, this.text.Length);
        }

        public void Append(string text, int index)
        {
            var query = from item in this.GetCells()
                        where item.Character != 0
                        select item;
            var cell = query.FirstOrDefault(item => item.TextIndex == index);
            var row = cell.Row;
            for (var i = cell.Index; i < row.Cells.Count; i++)
            {
                row.Cells[i].Character = char.MinValue;
            }
            var poolList = new Stack<TerminalRow>();
            for (var i = this.rowList.Count - 1; i >= row.Index; i--)
            {
                var item = this.rowList[i];
                poolList.Push(item);
                this.rowList.RemoveAt(i);
            }

            var s = this.text.Insert(index, text);

            {
                var fontAsset = this.FontAsset;
                var i = index;
                // var rowList = new List<TerminalRow>();
                while (i < s.Length)
                {
                    var row1 = poolList.Any() ? poolList.Pop() : new TerminalRow(this, this.rowList.Count);
                    FillString(fontAsset, row1, s, ref i);
                    rowList.Add(row1);
                }
            }
            this.OnLayoutChanged(EventArgs.Empty);
        }

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value ?? throw new ArgumentNullException(nameof(value));
                this.UpdateGrid();
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        public TMP_FontAsset FontAsset => this.fontAsset;

        public int ColumnCount { get; private set; }

        public int RowCount { get; private set; }

        public IReadOnlyList<TerminalRow> Rows => this.rowList;

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
                    this.UpdateScrollbarValue();
                    this.OnVisibleIndexChanged(EventArgs.Empty);
                }
            }
        }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public TerminalPoint CursorLocation
        {
            get => this.cursorPostion;
            set
            {
                if (this.cursorPostion != value)
                {
                    this.cursorPostion = value;
                    this.UpdateCursorPosition();
                    this.OnCursorPositionChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler TextChanged;

        public event EventHandler VisibleIndexChanged;

        public event EventHandler LayoutChanged;

        public event EventHandler SelectionChanged;

        public event EventHandler CursorPositionChanged;

        // public override void Rebuild(CanvasUpdate executing)
        // {
        //     base.Rebuild(executing);
        // }

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

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.UpdateGrid();
            this.UpdateVisibleIndex();
            this.UpdateScrollVisible();
            this.UpdateScrollbarSize();
            this.UpdateScrollbarValue();
            this.UpdateCursorPosition();
            this.OnTextChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
            // Debug.Log($"{TerminalGrid.GetItemWidth(this) * 80}x{TerminalGrid.GetItemHeight(this) * 24}");
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("TextMeshPro/Bitmap"));
            this.material.color = base.color;
            if (this.verticalScrollbar != null)
            {
                this.verticalScrollbar.onValueChanged.AddListener(VerticalScrollbar_OnValueChanged);
            }
            this.SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.verticalScrollbar != null)
            {
                this.verticalScrollbar.onValueChanged.RemoveListener(VerticalScrollbar_OnValueChanged);
            }
        }

        private void VerticalScrollbar_OnValueChanged(float arg0)
        {
            if (this.isUpdating == false)
            {
                var value1 = (float)this.verticalScrollbar.value;
                var value2 = (float)Math.Max(1, this.rowList.Count - this.RowCount);
                this.isUpdating = true;
                this.VisibleIndex = (int)(value1 * value2);
                this.isUpdating = false;
            }
        }

        private void UpdateGrid()
        {
            if (this.fontAsset != null)
            {
                var rect = this.rectTransform.rect;
                var itemWidth = FontUtility.GetItemWidth(this.fontAsset);
                var itemHeight = FontUtility.GetItemHeight(this.fontAsset);
                this.ColumnCount = (int)(rect.width / itemWidth);
                this.RowCount = (int)(rect.height / itemHeight);
                this.rowList.Clear();
                this.rowList.AddRange(GenerateTerminalRows(this, this.text));
            }
            else
            {
                this.ColumnCount = 0;
                this.RowCount = 0;
                this.rowList.Clear();
            }
            Debug.Log("Grid Update.");
        }

        private void UpdateVisibleIndex()
        {
            if (this.Rows.Count < this.RowCount)
            {
                this.visibleIndex = 0;
            }
            else
            {
                this.visibleIndex = Math.Max(this.visibleIndex, 0);
                this.visibleIndex = Math.Min(this.visibleIndex, this.Rows.Count - this.RowCount);
            }
        }

        private void UpdateScrollVisible()
        {
            if (this.verticalScrollbar != null)
            {
                var gameObject = this.verticalScrollbar.gameObject;
                gameObject.SetActive(this.rowList.Count >= this.RowCount);
            }
        }

        private void UpdateScrollbarValue()
        {
            if (this.verticalScrollbar != null && this.isUpdating == false)
            {
                var value1 = this.visibleIndex;
                var value2 = (float)Math.Max(1, this.rowList.Count - this.RowCount);
                this.isUpdating = true;
                this.verticalScrollbar.value = value1 / value2;
                this.isUpdating = false;
            }
        }

        private void UpdateScrollbarSize()
        {
            if (this.verticalScrollbar != null)
            {
                var value1 = (float)Math.Max(1, this.RowCount);
                var value2 = (float)Math.Max(1, this.rowList.Count);
                this.verticalScrollbar.size = value1 / value2;
            }
        }

        private void UpdateCursorPosition()
        {
            var x = this.cursorPostion.X;
            var y = this.cursorPostion.Y;
            x = Math.Max(x, 0);
            x = Math.Min(x, this.ColumnCount - 1);
            y = Math.Max(y, 0);
            y = Math.Min(y, this.RowCount - 1);
            this.cursorPostion = new TerminalPoint(x, y);
        }

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var i = p1.X + p1.Y * this.ColumnCount;
            var end = p2.X + p2.Y * this.ColumnCount;
            while (i <= end)
            {
                var x = i % this.ColumnCount;
                var y = i / this.ColumnCount;
                yield return this.rowList[y].Cells[x];
                i++;
            }
        }

        private IEnumerable<TerminalCell> GetCells()
        {
            var query = from row in this.rowList
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
                    Debug.Log($"TextIndex: {cell.TextIndex}");
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
