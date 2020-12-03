////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class TerminalComposition : UIBehaviour, INotifyValidated, IPropertyChangedNotifyable, IValidatable
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

        private readonly PropertyNotifier notifier;

        public TerminalComposition()
        {
            this.notifier = new PropertyNotifier(this.InvokePropertyChangedEvent);
        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set
            {
                if (this.grid != value)
                {
                    this.grid = value;
                    this.InvokePropertyChangedEvent(nameof(Grid));
                }
            }
        }

        [FieldName(nameof(text))]
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
                    this.InvokePropertyChangedEvent(nameof(Text));
                }
            }
        }

        [FieldName(nameof(columnIndex))]
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
                    this.InvokePropertyChangedEvent(nameof(ColumnIndex));
                }
            }
        }

        [FieldName(nameof(rowIndex))]
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
                    this.InvokePropertyChangedEvent(nameof(RowIndex));
                }
            }
        }

        [FieldName(nameof(foregroundColor))]
        public Color ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                if (this.foregroundColor != value)
                {
                    this.foregroundColor = value;
                    this.InvokePropertyChangedEvent(nameof(ForegroundColor));
                }
            }
        }

        [FieldName(nameof(backgroundColor))]
        public Color BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                if (this.backgroundColor != value)
                {
                    this.backgroundColor = value;
                    this.InvokePropertyChangedEvent(nameof(BackgroundColor));
                }
            }
        }

        [FieldName(nameof(foregroundMargin))]
        public TerminalThickness ForegroundMargin
        {
            get => this.foregroundMargin;
            set
            {
                if (this.foregroundMargin != value)
                {
                    this.foregroundMargin = value;
                    this.InvokePropertyChangedEvent(nameof(ForegroundMargin));
                }
            }
        }

        [FieldName(nameof(backgroundMargin))]
        public TerminalThickness BackgroundMargin
        {
            get => this.backgroundMargin;
            set
            {
                if (this.backgroundMargin != value)
                {
                    this.backgroundMargin = value;
                    this.InvokePropertyChangedEvent(nameof(BackgroundMargin));
                }
            }
        }

        public Vector2 Offset { get; set; } = Vector2.zero;

        public TerminalFont Font => this.grid?.Font;

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void Start()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.UpdateColor();
            TerminalValidationEvents.Register(this);
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalValidationEvents.Validated += Object_Validated;
            this.OnEnabled(EventArgs.Empty);
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
            base.OnDisable();
            this.OnDisabled(EventArgs.Empty);
            TerminalValidationEvents.Unregister(this);
        }

        protected virtual void OnEnabled(EventArgs e)
        {
            this.Enabled?.Invoke(this, e);
        }

        protected virtual void OnDisabled(EventArgs e)
        {
            this.Disabled?.Invoke(this, e);
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                switch (e.PropertyName)
                {
                    case nameof(ITerminalGrid.ForegroundColor):
                    case nameof(ITerminalGrid.Style):
                        {
                            this.UpdateColor();
                        }
                        break;
                    case nameof(ITerminalGrid.VisibleIndex):
                    case nameof(ITerminalGrid.CursorPoint):
                        {
                            var cursorPoint = this.grid.CursorPoint;
                            var visibleIndex = this.grid.VisibleIndex;
                            this.notifier.Begin();
                            this.notifier.SetField(ref this.columnIndex, cursorPoint.X, nameof(ColumnIndex));
                            this.notifier.SetField(ref this.rowIndex, cursorPoint.Y - visibleIndex, nameof(RowIndex));
                            this.notifier.End();
                        }
                        break;
                    case nameof(ITerminalGrid.CompositionString):
                        {
                            this.Text = this.grid.CompositionString;
                        }
                        break;
                }
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                var cursorPoint = this.grid.CursorPoint;
                var visibleIndex = this.grid.VisibleIndex;
                this.columnIndex = cursorPoint.X;
                this.rowIndex = cursorPoint.Y - visibleIndex;
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
                this.notifier.Begin();
                this.notifier.SetField(ref this.foregroundColor, this.grid.ForegroundColor, nameof(ForegroundColor));
                this.notifier.SetField(ref this.backgroundColor, this.grid.BackgroundColor, nameof(BackgroundColor));
                this.notifier.End();
            }
        }

        private int BufferWidth => this.grid != null ? this.grid.BufferWidth : 0;

        private int BufferHeight => this.grid != null ? this.grid.BufferHeight : 0;

        internal void Validate()
        {
            this.columnIndex = Math.Min(this.BufferWidth - 1, this.columnIndex);
            this.columnIndex = Math.Max(0, this.columnIndex);
            this.rowIndex = Math.Min(this.BufferHeight - 1, this.rowIndex);
            this.rowIndex = Math.Max(0, this.rowIndex);
            this.OnValidated(EventArgs.Empty);
        }

        #region IPropertyChangedNotifyable

        void IPropertyChangedNotifyable.InvokePropertyChangedEvent(string propertyName)
        {
            this.InvokePropertyChangedEvent(propertyName);
        }

        #endregion

        #region IValidatable

        void IValidatable.Validate()
        {
            this.Validate();
        }

        #endregion
    }
}
