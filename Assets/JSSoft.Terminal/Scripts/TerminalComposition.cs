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
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    class TerminalComposition : UIBehaviour, INotifyValidated
    {
        private static readonly int[] backgroundTriangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        private static readonly int[] foregroundTriangles = new int[6] { 4, 5, 6, 6, 7, 4 };

        [SerializeField]
        private string text = string.Empty;
        [SerializeField]
        private Color foregroundColor = Color.white;
        [SerializeField]
        private Color backgroundColor = new Color(0, 0, 0, 0);
        [SerializeField]
        private TerminalThickness foregroundMargin = TerminalThickness.Empty;
        [SerializeField]
        private TerminalThickness backgroundMargin = new TerminalThickness(2, 0, 0, 0);
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private int columnIndex;
        [SerializeField]
        private int rowIndex;

        public TerminalComposition()
        {

        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set
            {
                if (this.grid != value)
                {
                    this.grid = value;
                    this.InvokePropertyChanged(nameof(Grid));
                }
            }
        }

        public string Text
        {
            get => this.text;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.text != value)
                {
                    this.text = value;
                    this.InvokePropertyChanged(nameof(Text));
                }
            }
        }

        public int ColumnIndex
        {
            get => this.columnIndex;
            set
            {
                if (value < 0 || value >= this.BufferWidth)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.columnIndex != value)
                {
                    this.columnIndex = value;
                    this.InvokePropertyChanged(nameof(ColumnIndex));
                }
            }
        }

        public int RowIndex
        {
            get => this.rowIndex;
            set
            {
                if (value < 0 || value >= this.BufferHeight)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.rowIndex != value)
                {
                    this.rowIndex = value;
                    this.InvokePropertyChanged(nameof(RowIndex));
                }
            }
        }

        public Color ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                if (this.foregroundColor != value)
                {
                    this.foregroundColor = value;
                    this.InvokePropertyChanged(nameof(ForegroundColor));
                }
            }
        }

        public Color BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                if (this.backgroundColor != value)
                {
                    this.backgroundColor = value;
                    this.InvokePropertyChanged(nameof(BackgroundColor));
                }
            }
        }

        public TerminalThickness ForegroundMargin
        {
            get => this.foregroundMargin;
            set
            {
                if (this.foregroundMargin != value)
                {
                    this.foregroundMargin = value;
                    this.InvokePropertyChanged(nameof(ForegroundMargin));
                }
            }
        }

        public TerminalThickness BackgroundMargin
        {
            get => this.backgroundMargin;
            set
            {
                if (this.backgroundMargin != value)
                {
                    this.backgroundMargin = value;
                    this.InvokePropertyChanged(nameof(BackgroundMargin));
                }
            }
        }

        public Vector2 Offset
        {
            get; set;
        } = new Vector2(0, 0);

        public TerminalFont Font => this.grid?.Font;

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.columnIndex = Math.Min(this.BufferWidth - 1, this.columnIndex);
            this.columnIndex = Math.Max(0, this.columnIndex);
            this.rowIndex = Math.Min(this.BufferHeight - 1, this.rowIndex);
            this.rowIndex = Math.Max(0, this.rowIndex);
            this.OnValidated(EventArgs.Empty);
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalValidationEvents.Validated += Object_Validated;
            TerminalValidationEvents.Register(this);
            this.UpdateColor();
        }

        protected override void OnDisable()
        {
            TerminalValidationEvents.Unregister(this);
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
            base.OnDisable();
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                switch (e.PropertyName)
                {
                    case nameof(ITerminalGrid.VisibleIndex):
                    case nameof(ITerminalGrid.CursorPoint):
                        {
                            var cursorPoint = this.grid.CursorPoint;
                            var visibleIndex = this.grid.VisibleIndex;
                            this.columnIndex = cursorPoint.X;
                            this.rowIndex = cursorPoint.Y - visibleIndex;
                        }
                        break;
                    case nameof(ITerminalGrid.CompositionString):
                        {
                            this.text = this.grid.CompositionString;
                        }
                        break;
                }
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.UpdateColor();
            }
        }

        private async void Object_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalStyle style && style == this.grid?.Style)
            {
                await Task.Delay(1);
                this.UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (this.IsDestroyed() == true)
                return;
            if (this.grid != null)
            {
                this.foregroundColor = this.grid.ForegroundColor;
            }
        }

        private void InvokePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private int BufferWidth => this.grid != null ? this.grid.BufferWidth : 0;

        private int BufferHeight => this.grid != null ? this.grid.BufferHeight : 0;
    }
}
