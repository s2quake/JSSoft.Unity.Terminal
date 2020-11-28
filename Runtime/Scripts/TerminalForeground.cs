// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class TerminalForeground : UIBehaviour
    {
        [SerializeField]
        private TerminalGrid grid = null;

        private readonly List<ITerminalCell> cellList = new List<ITerminalCell>();
        private readonly List<TerminalForegroundItem> itemsToDelete = new List<TerminalForegroundItem>();

        private int visibleIndex;
        private string text = string.Empty;
        private TerminalFont font;

        public TerminalForeground()
        {
        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        internal IEnumerable<ITerminalCell> GetCells(Texture2D texture)
        {
            foreach (var item in this.cellList)
            {
                if (item.Texture == texture)
                    yield return item;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.Enabled += Grid_Enabled;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalGridEvents.SelectionChanged += Grid_SelectionChanged;
            TerminalValidationEvents.Validated += Object_Validated;
            this.font = this.grid != null ? this.grid.Font : null;
            this.text = this.grid != null ? this.grid.Text : string.Empty;
            this.Invoke(nameof(SetDirty), float.Epsilon);
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.Enabled -= Grid_Enabled;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.SelectionChanged -= Grid_SelectionChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
            this.text = string.Empty;
            this.font = null;
            base.OnDisable();
        }

        private void SetDirty()
        {
            this.SetDirty(true);
        }

        private void SetDirty(bool force)
        {
            if (this.font != this.grid.Font)
            {
                this.font = this.grid.Font;
                this.RefreshChilds();
            }
            if (this.text != this.grid.Text ||
                this.visibleIndex != this.grid.VisibleIndex || force == true)
            {
                this.UpdateCellList();
                this.text = this.grid.Text;
                this.visibleIndex = this.grid.VisibleIndex;
            }
            foreach (var item in this.Items)
            {
                item.SetVerticesDirty();
            }
        }

        private void UpdateCellList()
        {
            var visibleCells = TerminalGridUtility.GetVisibleCells(this.grid, item => item.Character != 0 && item.Texture != null);
            this.cellList.Clear();
            this.cellList.AddRange(visibleCells);
        }

        private void Grid_Enabled(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetDirty(true);
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                switch (e.PropertyName)
                {
                    case nameof(ITerminalGrid.Font):
                    case nameof(ITerminalGrid.Style):
                    case nameof(ITerminalGrid.ForegroundColor):
                    case nameof(ITerminalGrid.VisibleIndex):
                    case nameof(ITerminalGrid.Text):
                    case nameof(ITerminalGrid.SelectingRange):
                    case nameof(ITerminalGrid.IsFocused):
                    case nameof(ITerminalGrid.CursorPoint):
                        {
                            this.SetDirty(false);
                        }
                        break;
                }
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetDirty(true);
            }
        }

        private void Grid_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetDirty(false);
            }
        }

        private void Object_Validated(object sender, EventArgs e)
        {
            switch (sender)
            {
                case TerminalStyle style when this.grid?.Style:
                    {
                        this.SetDirty(true);
                    }
                    break;
                case TerminalColorPalette palette when this.grid?.ColorPalette:
                    {
                        this.SetDirty(true);
                    }
                    break;
            }
        }

        private void RefreshChilds()
        {
            var textures = this.font != null ? this.font.Textures.ToArray() : new Texture2D[] { };
            var items = this.Items.ToArray();
            for (var i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                var gameObject = new GameObject($"Item{i}", typeof(TerminalForegroundItem));
                var foregroundItem = gameObject.GetComponent<TerminalForegroundItem>();
                var transform = foregroundItem.rectTransform;
                foregroundItem.material = new Material(Shader.Find("UI/Default"));
                foregroundItem.Texture = texture;
                foregroundItem.Grid = this.grid;
                foregroundItem.Foreground = this;
                transform.SetParent(this.transform);
                transform.anchorMin = Vector3.zero;
                transform.anchorMax = Vector3.one;
                transform.offsetMin = Vector3.zero;
                transform.offsetMax = Vector3.zero;
            }
            foreach (var item in items)
            {
                var rect = item.GetComponent<RectTransform>();
                rect.SetParent(null);
                this.itemsToDelete.Add(item);
            }
            this.Invoke(nameof(DeleteItems), float.Epsilon);
        }

        private void DeleteItems()
        {
            foreach (var item in this.itemsToDelete)
            {
                GameObject.DestroyImmediate(item.gameObject);
            }
            this.itemsToDelete.Clear();
        }

        private IEnumerable<TerminalForegroundItem> Items
        {
            get
            {
                for (var i = 0; i < this.transform.childCount; i++)
                {
                    var childTransform = this.transform.GetChild(i);
                    if (childTransform.GetComponent<TerminalForegroundItem>() is TerminalForegroundItem component)
                    {
                        yield return component;
                    }
                }
            }
        }
    }
}
