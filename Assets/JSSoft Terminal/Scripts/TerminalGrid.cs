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

namespace JSSoft.UI
{
    public class TerminalGrid : MaskableGraphic
    {
        [SerializeField]
        private TMP_FontAsset fontAsset;
        [SerializeField]
        [TextArea(5, 10)]
        private string text = "123";
        [SerializeField]
        private Color32 fontColor = TerminalColors.White;
        [SerializeField]
        private int visibleIndex;
        [SerializeField]
        private Scrollbar verticalScrollbar;

        private TerminalRow[] rows = new TerminalRow[] { };
        private bool isUpdating;

        public TerminalGrid()
        {
            // base.color = TerminalColors.Black;
        }

        public static Rect TransformRect(TerminalGrid grid, Rect rect)
        {
            if (grid != null && grid.fontAsset != null && grid.Rows.Length > 0)
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

        public static event EventHandler TextChanged;

        public event EventHandler VisibleIndexChanged;

        public TMP_FontAsset FontAsset => this.fontAsset;

        public int ColumnCount { get; private set; }

        public int RowCount { get; private set; }

        public int MaxRowCount => this.Rows.Length;

        public TerminalRow[] Rows
        {
            get => this.rows;
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.rows = value;
                this.UpdateScrollbarSize();
            }
        }

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
                    this.visibleIndex = Math.Max(this.visibleIndex, 0);
                    this.visibleIndex = Math.Min(this.visibleIndex, this.rows.Length - this.RowCount);
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

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
        }

        protected virtual void OnVisibleIndexChanged(EventArgs e)
        {
            this.VisibleIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.fontAsset != null)
            {
                var rect = this.rectTransform.rect;
                var itemWidth = FontUtility.GetItemWidth(this.fontAsset);
                var itemHeight = (int)this.fontAsset.faceInfo.lineHeight;
                this.ColumnCount = (int)(rect.width / itemWidth);
                this.RowCount = (int)(rect.height / itemHeight);
                this.Rows = GenerateTerminalRows(this, this.text, this.ColumnCount);
            }
            else
            {
                this.Rows = new TerminalRow[] { };
            }
            // for (var i = 0; i < this.rectTransform.childCount; i++)
            // {
            //     var childTransform = this.rectTransform.GetChild(i);
            //     var childGraphic = childTransform.GetComponent<Graphic>();
            //     if (childGraphic != null)
            //     {
            //         CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(childGraphic);
            //         // Debug.Log($"{nameof(OnValidate)}.{childGraphic.name}");
            //     }
            // }
            // if (this.verticalScrollbar != null)
            // {
            //     this.verticalScrollbar.value = this.visibleIndex;
            //     this.verticalScrollbar.size = this.rows.Length;
            // }
            this.visibleIndex = Math.Max(this.visibleIndex, 0);
            this.visibleIndex = Math.Min(this.visibleIndex, this.rows.Length - this.RowCount);
            this.UpdateScrollbarValue();
            TextChanged?.Invoke(this, EventArgs.Empty);
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
                this.verticalScrollbar.size = this.Rows.Length;
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
                var value2 = (float)Math.Max(1, this.MaxRowCount - this.RowCount);
                this.isUpdating = true;
                this.VisibleIndex = (int)(value1 * value2);
                this.isUpdating = false;
            }
        }

        public static TerminalRow[] GenerateTerminalRows(TerminalGrid grid, string text, int columnCount)
        {
            var fontAsset = grid.FontAsset;
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var rowList = new List<TerminalRow>(lines.Length);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                while (line != string.Empty)
                {
                    var row = new TerminalRow(grid, rowList.Count, columnCount);
                    line = FillString(fontAsset, row, line);
                    rowList.Add(row);
                }
            }
            return rowList.ToArray();
        }

        public static string FillString(TMP_FontAsset fontAsset, TerminalRow row, string text)
        {
            var columnIndex = 0;
            var cellCount = row.Cells.Length;
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

        private void UpdateScrollbarValue()
        {
            if (this.verticalScrollbar != null && this.isUpdating == false)
            {
                var value1 = this.visibleIndex;
                var value2 = (float)Math.Max(1, this.MaxRowCount - this.RowCount);
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
                var value2 = (float)Math.Max(1, this.MaxRowCount);
                this.verticalScrollbar.size = value1 / value2;
            }
        }
    }
}
