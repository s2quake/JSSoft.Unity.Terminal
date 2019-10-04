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

namespace JSSoft.UI
{
    public class TerminalGrid : MaskableGraphic, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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

        private readonly List<TerminalRow> rowList = new List<TerminalRow>();
        private bool isUpdating;

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

        public static TerminalRow[] GenerateTerminalRows(TerminalGrid grid, string text)
        {
            var fontAsset = grid.FontAsset;
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var rowList = new List<TerminalRow>(lines.Length);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                while (line != string.Empty)
                {
                    var row = new TerminalRow(grid, rowList.Count);
                    line = FillString(fontAsset, row, line);
                    rowList.Add(row);
                }
            }
            return rowList.ToArray();
        }

        public static string FillString(TMP_FontAsset fontAsset, TerminalRow row, string text)
        {
            var columnIndex = 0;
            var cellCount = row.Cells.Count;
            var i = 0;
            while (columnIndex < cellCount && i < text.Length)
            {
                var character = text[i];
                var cell = row.Cells[columnIndex];
                var volume = FontUtility.GetCharacterVolume(fontAsset, character);
                if (columnIndex + volume > cellCount)
                    break;
                cell.Refresh(fontAsset, character);
                columnIndex += volume;
                i++;
            }
            return text.Substring(i);
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
            foreach (var item in this.rowList)
            {
                if (item.Intersect(position) is TerminalCell cell)
                {
                    return new TerminalPoint(cell.Index, item.Index);
                }
            }
            return TerminalPoint.Invalid;
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

        public Color32 BackgroundColor
        {
            get => base.color;
            set => base.color = value;
        }

        public Color32 ForegroundColor
        {
            get => this.fontColor;
            set => this.fontColor = value;
        }

        public event EventHandler TextChanged;

        public event EventHandler VisibleIndexChanged;

        public event EventHandler LayoutChanged;

        public event EventHandler SelectionChanged;

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
        }

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

        protected override void OnValidate()
        {
            base.OnValidate();
            this.UpdateGrid();
            this.UpdateVisibleIndex();
            this.UpdateScrollVisible();
            this.UpdateScrollbarSize();
            this.UpdateScrollbarValue();
            this.OnTextChanged(EventArgs.Empty);
        }

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

        TerminalPoint startPoint;
        TerminalPoint endPoint;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.startPoint = this.endPoint = this.Intersect(position);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.endPoint = this.Intersect(position);
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = this.WorldToGrid(eventData.position);
                this.endPoint = this.Intersect(position);

                var p1 = this.startPoint < this.endPoint ? this.startPoint : this.endPoint;
                var p2 = this.startPoint > this.endPoint ? this.startPoint : this.endPoint;

                foreach(var item in this.GetCells(p1, p2))
                {
                    item.IsSelected = !item.IsSelected;
                }
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var x = p1.X;
            var y = p2.Y;
            while (x != p2.X && y != p2.Y)
            {
                yield return this.rowList[y].Cells[x];
                if (++x == this.ColumnCount)
                {
                    y++;
                    x = 0;
                }
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // var position = eventData.position;
                // var rect = this.rectTransform.rect;
                // position.y = rect.height - position.y;
                // position.y += GetItemHeight(this) * this.visibleIndex;
                // Debug.Log(position);
                // foreach (var item in this.rowList)
                // {
                //     if (item.Intersect(position) is TerminalCell cell)
                //     {
                //         cell.IsSelected = !cell.IsSelected;
                //         this.OnTextChanged(EventArgs.Empty);
                //         break;
                //     }
                // }
            }
        }
    }
}
