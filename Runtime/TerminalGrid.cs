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
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(Terminal))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [SelectionBase]
    public class TerminalGrid : TerminalGridBase,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IPropertyChangedNotifyable,
        IValidatable
    {
        [SerializeField]
        private TerminalFont font = null;
        [SerializeField]
        private Color backgroundColor = DefaultBackgroundColor;
        [SerializeField]
        private Color foregroundColor = DefaultForegroundColor;
        [SerializeField]
        private Color selectionColor = DefaultSelectionColor;
        [SerializeField]
        private Color selectionTextColor = DefaultSelectionTextColor;
        [SerializeField]
        private Color cursorColor = DefaultCursorColor;
        [SerializeField]
        private Color cursorTextColor = DefaultCursorTextColor;
        [SerializeField]
        private TerminalColorPalette colorPalette;
        [SerializeField]
        private int visibleIndex;
        [SerializeField]
        [Range(5, 1000)]
        private int maxBufferHeight = 500;

        [SerializeField]
        private TerminalCursorStyle cursorStyle;
        [SerializeField]
        [Range(0, 100)]
        private int cursorThickness = 2;
        [SerializeField]
        private bool isCursorBlinkable;
        [SerializeField]
        [Range(0, 3)]
        private float cursorBlinkDelay = 0.5f;
        [SerializeField]
        private bool isScrollForwardEnabled;
        [SerializeField]
        private List<TerminalBehaviourBase> behaviourList = new List<TerminalBehaviourBase>();
        [SerializeField]
        private TerminalPoint cursorPoint;
        [SerializeField]
        private bool isCursorVisible = true;

        [SerializeField]
        private string compositionString = string.Empty;
        [SerializeField]
        private TerminalThickness padding = new TerminalThickness(2);
        [SerializeField]
        private TerminalStyle style;

        private readonly TerminalRowCollection rows;
        private readonly TerminalCharacterInfoCollection characterInfos;
        private readonly TerminalGridSelection selections;
        private readonly PropertyNotifier notifier;
        private readonly Queue<KeyInfo> eventQueue = new Queue<KeyInfo>();

        private IInputHandler inputHandler;
        private Terminal terminal;
        private bool isFocused;
        private TerminalRange selectingRange = TerminalRange.Empty;
        private float scrollPos;
        private Rect rectangle;
        private IKeyBindingCollection keyBindings;
        private int cursorVolume;
        private int bufferWidth;
        private int bufferHeight;
        private string text;
        private bool scrollFlag;

        public TerminalGrid()
        {
            this.characterInfos = new TerminalCharacterInfoCollection(this);
            this.rows = new TerminalRowCollection(this, this.characterInfos);
            this.selections = new TerminalGridSelection(this);
            this.selections.CollectionChanged += Selections_CollectionChanged;
            this.notifier = new PropertyNotifier(this.InvokePropertyChangedEvent);
            this.PropertyChanged += TerminalGrid_PropertyChanged;
        }

        public override Vector2 WorldToGrid(Vector2 position, Camera camera)
        {
            var pixelRect = this.canvas.pixelRect;
            var localPosition = new Vector2(position.x, pixelRect.height - position.y);
            var rect = this.GetRect();
            var y = TerminalGridUtility.GetItemHeight(this) * this.visibleIndex;
            return localPosition - rect.position + new Vector2(0, y);
        }

        public override TerminalPoint Intersect(Vector2 position)
        {
            var padding = this.Padding;
            var itemWidth = this.Font.Width;
            var itemHeight = this.Font.Height;
            if (position.x < 0 || position.y < 0)
                return TerminalPoint.Invalid;
            position.x -= padding.Left;
            position.y -= padding.Top;
            var x = (int)(position.x / itemWidth);
            var y = (int)(position.y / itemHeight);
            if (x >= this.BufferWidth || y >= this.rows.Count)
                return TerminalPoint.Invalid;
            return new TerminalPoint(x, y);
        }

        public override ITerminalCell IntersectWithCell(Vector2 position)
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

        public override TerminalPoint IndexToPoint(int index) => this.characterInfos[index].Point;

        public override int PointToIndex(TerminalPoint point)
        {
            if (point.Y >= 0 && point.Y < this.rows.Count && point.X >= 0 && point.X < this.BufferWidth)
            {
                return this.rows[point.Y].Cells[point.X].TextIndex;
            }
            return -1;
        }

        public Color32? IndexToBackgroundColor(int index)
        {
            var pallete = this.ColorPalette;
            var color = this.terminal.GetBackgroundColor(index);
            if (color == null)
                return null;
            if (pallete != null)
                return pallete.GetColor(color.Value);
            return TerminalColors.GetColor(color.Value);
        }

        public Color32? IndexToForegroundColor(int index)
        {
            var pallete = this.ColorPalette;
            var color = this.terminal.GetForegroundColor(index);
            if (color == null)
                return null;
            if (pallete != null)
                return pallete.GetColor(color.Value);
            return TerminalColors.GetColor(color.Value);
        }

        public override void Focus()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != this.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
        }

        public override void ScrollToTop()
        {
            this.IsScrolling = true;
            this.VisibleIndex = this.MinimumVisibleIndex;
            this.IsScrolling = false;
        }

        public override void ScrollToBottom()
        {
            this.IsScrolling = true;
            this.VisibleIndex = this.MaximumVisibleIndex;
            this.IsScrolling = false;
        }

        public override void ScrollToCursor()
        {
            if (this.CursorPoint.Y < this.VisibleIndex)
            {
                this.VisibleIndex = this.CursorPoint.Y;
            }
            if (this.CursorPoint.Y >= this.VisibleIndex + this.BufferHeight && this.BufferHeight > 0)
            {
                this.VisibleIndex = this.CursorPoint.Y - this.BufferHeight + 1;
            }
        }

        public override void PageUp()
        {
            this.Scroll(-this.BufferHeight);
        }

        public override void PageDown()
        {
            this.Scroll(this.BufferHeight);
        }

        public override void LineUp()
        {
            this.Scroll(-1);
        }

        public override void LineDown()
        {
            this.Scroll(1);
        }

        public override void Scroll(int value)
        {
            var visibleIndex = this.VisibleIndex + value;
            visibleIndex = Math.Max(0, visibleIndex);
            visibleIndex = Math.Min(this.MaximumVisibleIndex, visibleIndex);
            this.IsScrolling = true;
            this.VisibleIndex = visibleIndex;
            this.IsScrolling = false;
        }

        public override string Copy()
        {
            if (this.Selections.Any() == true)
            {
                var range = this.Selections.First();
                return this.GetString(range);
            }
            else
            {
                return string.Empty;
            }
        }

        public override void Paste(string text)
        {
            foreach (var item in text)
            {
                this.eventQueue.Enqueue(new KeyInfo()
                {
                    Character = item
                });
            }
        }

        public override void SelectAll()
        {
            var p1 = new TerminalPoint(0, 0);
            var p2 = new TerminalPoint(this.BufferWidth, this.MaximumVisibleIndex + this.BufferHeight - 1);
            var range = new TerminalRange(p1, p2);
            this.Selections.Clear();
            this.Selections.Add(range);
        }

        public override TerminalGridData Save()
        {
            var data = new TerminalGridData()
            {
                TerminalData = (this.terminal ?? GetComponent<Terminal>()).Save(),
                Selections = this.Selections.ToArray(),
                BufferWidth = this.bufferWidth,
                BufferHeight = this.bufferHeight,
                Font = this.font,
                BackgroundColor = this.backgroundColor,
                ForegroundColor = this.foregroundColor,
                SelectionColor = this.selectionColor,
                SelectionTextColor = this.selectionTextColor,
                CursorColor = this.cursorColor,
                CursorTextColor = this.cursorTextColor,
                ColorPalette = this.colorPalette,
                VisibleIndex = this.visibleIndex,
                MaxBufferHeight = this.maxBufferHeight,
                CursorStyle = this.cursorStyle,
                CursorThickness = this.cursorThickness,
                IsCursorBlinkable = this.isCursorBlinkable,
                CursorBlinkDelay = this.cursorBlinkDelay,
                IsScrollForwardEnabled = this.isScrollForwardEnabled,
                Behaviours = this.behaviourList.ToArray(),
                CursorPoint = this.cursorPoint,
                IsCursorVisible = this.isCursorVisible,
                CompositionString = this.compositionString,
                Padding = this.padding,
                Style = this.style
            };
            return data;
        }

        public override void Load(TerminalGridData data)
        {
            this.terminal.Load(data.TerminalData);

            this.notifier.Begin();
            this.notifier.SetField(ref this.font, data.Font, nameof(Font));
            this.notifier.SetField(ref this.backgroundColor, data.BackgroundColor, nameof(BackgroundColor));
            this.notifier.SetField(ref this.foregroundColor, data.ForegroundColor, nameof(ForegroundColor));
            this.notifier.SetField(ref this.selectionColor, data.SelectionColor, nameof(SelectionColor));
            this.notifier.SetField(ref this.selectionTextColor, data.SelectionTextColor, nameof(SelectionTextColor));
            this.notifier.SetField(ref this.cursorColor, data.CursorColor, nameof(CursorColor));
            this.notifier.SetField(ref this.cursorTextColor, data.CursorTextColor, nameof(CursorTextColor));
            this.notifier.SetField(ref this.colorPalette, data.ColorPalette, nameof(ColorPalette));
            this.notifier.SetField(ref this.visibleIndex, data.VisibleIndex, nameof(VisibleIndex));
            this.notifier.SetField(ref this.maxBufferHeight, data.MaxBufferHeight, nameof(MaxBufferHeight));
            this.notifier.SetField(ref this.cursorStyle, data.CursorStyle, nameof(CursorStyle));
            this.notifier.SetField(ref this.cursorThickness, data.CursorThickness, nameof(CursorThickness));
            this.notifier.SetField(ref this.isCursorBlinkable, data.IsCursorBlinkable, nameof(IsCursorBlinkable));
            this.notifier.SetField(ref this.cursorBlinkDelay, data.CursorBlinkDelay, nameof(CursorBlinkDelay));
            this.notifier.SetField(ref this.isScrollForwardEnabled, data.IsScrollForwardEnabled, nameof(IsScrollForwardEnabled));
            this.notifier.SetField(ref this.cursorPoint, data.CursorPoint, nameof(CursorPoint));
            this.notifier.SetField(ref this.isCursorVisible, data.IsCursorVisible, nameof(IsCursorVisible));
            this.notifier.SetField(ref this.compositionString, data.CompositionString, nameof(CompositionString));
            this.notifier.SetField(ref this.padding, data.Padding, nameof(Padding));
            this.scrollPos = this.visibleIndex;
            this.notifier.SetField(ref this.style, data.Style, nameof(Style));
            this.notifier.End();
            this.behaviourList.Clear();
            this.behaviourList.AddRange(data.Behaviours);

            if (this.bufferWidth != data.BufferWidth || this.bufferHeight != data.BufferHeight)
            {
                // var sb = new StringBuilder();
                // sb.AppendLine($"BufferWidth or BufferHeight mismatch. Some data loads are ignored.");
                // sb.AppendLine($"width : {this.bufferWidth} != {data.BufferWidth}, height: {this.bufferHeight} != {data.BufferHeight}");
                // Debug.LogWarning(sb.ToString());
            }
            else
            {
                this.selections.Clear();
                foreach (var item in data.Selections)
                {
                    this.selections.Add(item);
                }
            }
            this.UpdateColor();
            this.SetLayoutDirty();
        }

        public void ProcessEvent()
        {
            var itemList = new List<char>();
            while (this.eventQueue.Any() == true)
            {
                var item = this.eventQueue.Dequeue();
                if (this.OnKeyDown(item.Modifiers, item.KeyCode) == true)
                    continue;
                if (this.terminal.IsReadOnly == false && item.Character != 0 && this.OnKeyPress(item.Character) == false)
                {
                    if (item.Character == '\n')
                    {
                        var items = itemList.ToArray();
                        itemList.Clear();
                        this.terminal.InsertCharacter(items);
                        this.terminal.Execute();
                        break;
                    }
                    else
                    {
                        itemList.Add(item.Character);
                    }
                }
            }
            if (itemList.Count > 0)
                this.terminal.InsertCharacter(itemList.ToArray());
        }

        public override IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.Unity.Terminal.KeyBindings.TerminalGridKeyBindings.GetDefaultBindings();
            set => this.keyBindings = value;
        }

        public override IInputHandler InputHandler
        {
            get => this.inputHandler;
            set
            {
                this.inputHandler?.Detach(this);
                this.inputHandler = value ?? InputHandlerInstances.DefaultHandler;
                this.inputHandler.Attach(this);
            }
        }

        public override TerminalBase Terminal => this.terminal;

        public override bool IsFocused => this.isFocused;

        public override string Text => this.terminal.Text;

        [FieldName(nameof(font))]
        public override TerminalFont Font
        {
            get
            {
                if (this.style != null && this.style.Font != null)
                    return this.style.Font;
                return this.font;
            }
            set
            {
                if (this.font != value)
                {
                    this.font = value;
                    this.UpdateLayout();
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPoint();
                    this.InvokePropertyChangedEvent(nameof(Font));
                    this.SetLayoutDirty();
                }
            }
        }

        [FieldName(nameof(maxBufferHeight))]
        public override int MaxBufferHeight
        {
            get => this.maxBufferHeight;
            set
            {
                if (value < this.bufferHeight)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.maxBufferHeight != value)
                {
                    this.maxBufferHeight = value;
                    this.InvokePropertyChangedEvent(nameof(MaxBufferHeight));
                    this.SetLayoutDirty();
                }
            }
        }

        public override int BufferWidth => this.bufferWidth;

        public override int BufferHeight => this.bufferHeight;

        public override IReadOnlyList<ITerminalRow> Rows => this.rows;

        public override IReadOnlyList<TerminalCharacterInfo> CharacterInfos => this.characterInfos;

        public override int VisibleIndex
        {
            get => this.visibleIndex;
            set
            {
                if (value < this.MinimumVisibleIndex || value > this.MaximumVisibleIndex)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.visibleIndex != value)
                {
                    this.visibleIndex = value;
                    this.scrollPos = this.visibleIndex;
                    this.InvokePropertyChangedEvent(nameof(VisibleIndex));
                }
            }
        }

        public override int MinimumVisibleIndex => this.rows.MinimumIndex;

        public override int MaximumVisibleIndex
        {
            get
            {
                if (this.IsScrollForwardEnabled == false)
                    return Math.Max(this.rows.MinimumIndex, this.rows.MaximumIndex - this.BufferHeight);
                return Math.Max(this.rows.MaximumIndex, this.maxBufferHeight) - this.BufferHeight;
            }
        }

        public int CursorVisibleIndex => Math.Max(this.rows.MinimumIndex, this.rows.MaximumIndex - this.BufferHeight);

        [FieldName(nameof(backgroundColor))]
        public override Color BackgroundColor
        {
            get => this.style != null ? this.style.BackgroundColor : this.backgroundColor;
            set
            {
                if (this.backgroundColor != value)
                {
                    this.backgroundColor = value;
                    this.InvokePropertyChangedEvent(nameof(BackgroundColor));
                }
            }
        }

        [FieldName(nameof(foregroundColor))]
        public override Color ForegroundColor
        {
            get => this.style != null ? this.style.ForegroundColor : this.foregroundColor;
            set
            {
                if (this.foregroundColor != value)
                {
                    this.foregroundColor = value;
                    this.InvokePropertyChangedEvent(nameof(ForegroundColor));
                }
            }
        }

        [FieldName(nameof(selectionColor))]
        public override Color SelectionColor
        {
            get => this.style != null ? this.style.SelectionColor : this.selectionColor;
            set
            {
                if (this.selectionColor != value)
                {
                    this.selectionColor = value;
                    this.InvokePropertyChangedEvent(nameof(SelectionColor));
                }
            }
        }

        [FieldName(nameof(selectionTextColor))]
        public override Color SelectionTextColor
        {
            get => this.style != null ? this.style.SelectionTextColor : this.selectionTextColor;
            set
            {
                if (this.selectionTextColor != value)
                {
                    this.selectionTextColor = value;
                    this.InvokePropertyChangedEvent(nameof(SelectionTextColor));
                }
            }
        }

        [FieldName(nameof(cursorColor))]
        public override Color CursorColor
        {
            get => this.style != null ? this.style.CursorColor : this.cursorColor;
            set
            {
                if (this.cursorColor != value)
                {
                    this.cursorColor = value;
                    this.InvokePropertyChangedEvent(nameof(CursorColor));
                }
            }
        }

        [FieldName(nameof(cursorTextColor))]
        public override Color CursorTextColor
        {
            get => this.style != null ? this.style.CursorTextColor : this.cursorTextColor;
            set
            {
                if (this.cursorTextColor != value)
                {
                    this.cursorTextColor = value;
                    this.InvokePropertyChangedEvent(nameof(CursorTextColor));
                }
            }
        }

        [FieldName(nameof(colorPalette))]
        public TerminalColorPalette ColorPalette
        {
            get
            {
                if (this.style != null && this.style.ColorPalette != null)
                    return this.style.ColorPalette;
                return this.colorPalette;
            }
            set
            {
                if (this.colorPalette != value)
                {
                    this.colorPalette = value;
                    this.InvokePropertyChangedEvent(nameof(ColorPalette));
                }
            }
        }

        public override TerminalPoint CursorPoint
        {
            get => this.cursorPoint;
            set
            {
                if (this.cursorPoint != value)
                {
                    this.cursorPoint = value;
                    this.UpdateCursorPoint();
                    this.InvokePropertyChangedEvent(nameof(CursorPoint));
                }
            }
        }

        public override Rect Rectangle => this.rectangle;

        [FieldName(nameof(padding))]
        public override TerminalThickness Padding
        {
            get => this.padding;
            set
            {
                if (this.padding != value)
                {
                    this.padding = value;
                    this.InvokePropertyChangedEvent(nameof(Padding));
                    this.UpdateLayout();
                }
            }
        }

        [FieldName(nameof(style))]
        public override TerminalStyle Style
        {
            get => this.style;
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    this.InvokePropertyChangedEvent(nameof(Style));
                }
            }
        }

        public override bool IsCursorVisible
        {
            get => this.isCursorVisible;
            set
            {
                if (this.isCursorVisible != value)
                {
                    this.isCursorVisible = value;
                    this.InvokePropertyChangedEvent(nameof(IsCursorVisible));
                }
            }
        }

        public override bool IsScrolling { get; set; }

        public override TerminalRange SelectingRange
        {
            get => this.selectingRange;
            set
            {
                if (this.selectingRange != value)
                {
                    this.selectingRange = value;
                    this.InvokePropertyChangedEvent(nameof(SelectingRange));
                }
            }
        }

        public override string CompositionString
        {
            get => this.compositionString;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.compositionString != value)
                {
                    this.compositionString = value;
                    this.InvokePropertyChangedEvent(nameof(CompositionString));
                }
            }
        }

        [FieldName(nameof(cursorStyle))]
        public override TerminalCursorStyle CursorStyle
        {
            get => this.style != null ? this.style.CursorStyle : this.cursorStyle;
            set
            {
                if (this.cursorStyle != value)
                {
                    this.cursorStyle = value;
                    this.InvokePropertyChangedEvent(nameof(CursorStyle));
                }
            }
        }

        [FieldName(nameof(cursorThickness))]
        public override int CursorThickness
        {
            get => this.style != null ? this.style.CursorThickness : this.cursorThickness;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.cursorThickness != value)
                {
                    this.cursorThickness = value;
                    this.InvokePropertyChangedEvent(nameof(CursorThickness));
                }
            }
        }

        [FieldName(nameof(isCursorBlinkable))]
        public override bool IsCursorBlinkable
        {
            get => this.style != null ? this.style.IsCursorBlinkable : this.isCursorBlinkable;
            set
            {
                if (this.isCursorBlinkable != value)
                {
                    this.isCursorBlinkable = value;
                    this.InvokePropertyChangedEvent(nameof(IsCursorBlinkable));
                }
            }
        }

        [FieldName(nameof(cursorBlinkDelay))]
        public override float CursorBlinkDelay
        {
            get => this.style != null ? this.style.CursorBlinkDelay : this.cursorBlinkDelay;
            set
            {
                if (value < 0.0f)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.cursorBlinkDelay != value)
                {
                    this.cursorBlinkDelay = value;
                    this.InvokePropertyChangedEvent(nameof(CursorBlinkDelay));
                }
            }
        }

        [FieldName(nameof(isScrollForwardEnabled))]
        public override bool IsScrollForwardEnabled
        {
            get => this.style != null ? this.style.IsScrollForwardEnabled : this.isScrollForwardEnabled;
            set
            {
                if (this.isScrollForwardEnabled != value)
                {
                    this.isScrollForwardEnabled = value;
                    this.InvokePropertyChangedEvent(nameof(IsScrollForwardEnabled));
                }
            }
        }

        [FieldName(nameof(behaviourList))]
        public override IList<TerminalBehaviourBase> BehaviourList => this.behaviourList;

        public override IList<TerminalRange> Selections => this.selections;

        public override TerminalDispatcher Dispatcher => this.terminal?.Dispatcher;

        public override event EventHandler LayoutChanged;

        public override event NotifyCollectionChangedEventHandler SelectionChanged;

        public override event EventHandler GotFocus;

        public override event EventHandler LostFocus;

        public override event EventHandler Validated;

        public override event PropertyChangedEventHandler PropertyChanged;

        public override event EventHandler Enabled;

        public override event EventHandler Disabled;

        public override event EventHandler<TerminalKeyDownEventArgs> PreviewKeyDown;

        public override event EventHandler<TerminalKeyDownEventArgs> KeyDown;

        public override event EventHandler<TerminalKeyPressEventArgs> KeyPress;

        internal TerminalCell GetCell(TerminalPoint point)
        {
            if (point.Y < 0 || point.Y >= this.rows.Count)
                return null;
            var row = this.rows[point.Y];
            if (point.X < 0 || point.X >= row.Cells.Count)
                return null;
            return row.Cells[point.X];
        }

        internal void UpdateLayout()
        {
            if (this.terminal == null)
                return;
            var cursorIndex = this.terminal.CursorIndex;
            var size = this.rectTransform.rect.size;
            size.x = Math.Abs(size.x);
            size.y = Math.Abs(size.y);
            var bufferSize = TerminalGridUtility.GetBufferSize(this, size);
            if (bufferSize.y <= 0)
                return;
            this.UpdateRectangle(bufferSize);
            this.notifier.Begin();
            this.notifier.SetField(ref this.bufferWidth, (int)bufferSize.x, nameof(BufferWidth));
            this.notifier.SetField(ref this.bufferHeight, (int)bufferSize.y, nameof(BufferHeight));
            this.characterInfos.Update();
            this.rows.Update();
            this.notifier.SetField(ref this.cursorPoint, this.IndexToPoint(cursorIndex), nameof(CursorPoint));
            this.notifier.SetField(ref this.visibleIndex, TerminalGridValidator.GetVisibleIndex(this), nameof(VisibleIndex));
            this.notifier.End();
            this.OnLayoutChanged(EventArgs.Empty);
        }

        protected virtual void OnLayoutChanged(EventArgs e)
        {
            this.LayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGotFocus(EventArgs e)
        {
            this.GotFocus?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLostFocus(EventArgs e)
        {
            this.LostFocus?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnEnabled(EventArgs e)
        {
            this.Enabled?.Invoke(this, e);
        }

        protected virtual void OnDisabled(EventArgs e)
        {
            this.Disabled?.Invoke(this, e);
        }

        protected virtual void OnPreviewKeyDown(TerminalKeyDownEventArgs e)
        {
            this.PreviewKeyDown?.Invoke(this, e);
        }

        protected virtual void OnKeyDown(TerminalKeyDownEventArgs e)
        {
            this.KeyDown?.Invoke(this, e);
        }

        protected virtual void OnKeyPress(TerminalKeyPressEventArgs e)
        {
            this.KeyPress?.Invoke(this, e);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (this.IsActive() == true)
            {
                this.UpdateLayout();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.inputHandler = InputHandlerInstances.DefaultHandler;
            this.inputHandler.Attach(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.terminal = this.GetComponent<Terminal>();
            this.terminal.Worked += Terminal_Worked;
            this.terminal.defaultProgressGenerator = new ProgressGenerator(this);
            this.text = this.terminal.Text;
            this.UpdateLayout();
            this.UpdateColor();
            TerminalGridEvents.Register(this);
            TerminalEvents.Validated += Terminal_Validated;
            TerminalEvents.PropertyChanged += Terminal_PropertyChanged;
            TerminalEvents.TextChanged += Terminal_TextChanged;
            TerminalEvents.Executed += Terminal_Executed;
            TerminalValidationEvents.Validated += Object_Validated;
            this.OnEnabled(EventArgs.Empty);
        }

        protected override void OnDisable()
        {
            TerminalEvents.Validated -= Terminal_Validated;
            TerminalEvents.PropertyChanged -= Terminal_PropertyChanged;
            TerminalEvents.TextChanged -= Terminal_TextChanged;
            TerminalEvents.Executed -= Terminal_Executed;
            TerminalValidationEvents.Validated -= Object_Validated;
            this.terminal.Worked -= Terminal_Worked;
            this.terminal = null;
            base.OnDisable();
            this.OnDisabled(EventArgs.Empty);
            TerminalGridEvents.Unregister(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.inputHandler?.Detach(this);
        }

        protected virtual void Update()
        {
            if (this.terminal.IsExecuting == false)
                this.ProcessEvent();
        }

        protected virtual bool OnPreviewKeyDown(EventModifiers modifiers, KeyCode keyCode)
        {
            var args = new TerminalKeyDownEventArgs(modifiers, keyCode);
            this.OnPreviewKeyDown(args);
            if (args.Handled == true)
                return true;
            if (this.terminal.ProcessKeyEvent(modifiers, keyCode, true) == true)
                return true;
            return this.KeyBindings.Process(this, modifiers, keyCode, true);
        }

        protected virtual bool OnKeyDown(EventModifiers modifiers, KeyCode keyCode)
        {
            var args = new TerminalKeyDownEventArgs(modifiers, keyCode);
            this.OnKeyDown(args);
            if (args.Handled == true)
                return true;
            if (this.terminal.ProcessKeyEvent(modifiers, keyCode, false) == true)
                return true;
            return this.KeyBindings.Process(this, modifiers, keyCode, false);
        }

        protected virtual bool OnKeyPress(char character)
        {
            var args = new TerminalKeyPressEventArgs(character);
            this.OnKeyPress(args);
            if (args.Handled == true)
                return true;
            if (character == '\t' || character == 27 || character == 25)
                return true;
            return false;
        }

        protected virtual void OnTranslateEvents(IList<Event> eventList)
        {
            for (var i = 0; i < eventList.Count; i++)
            {
                var item = eventList[i];
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

        private void ProcessEvent(bool scroll)
        {

        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateColor()
        {
            if (this.style != null)
            {
                base.color = this.style.BackgroundColor;
            }
            else
            {
                base.color = this.backgroundColor;
            }
        }

        private void UpdateRectangle(Vector2 bufferSize)
        {
            var rect = this.GetComponent<RectTransform>().rect;
            var padding = this.padding;
            var itemWidth = TerminalGridUtility.GetItemWidth(this);
            var itemHeight = TerminalGridUtility.GetItemHeight(this);
            var bufferWidth = (int)(rect.width / itemWidth);
            var bufferHeight = (int)(rect.height / itemHeight);
            var rectWidth = bufferSize.x * itemWidth + padding.Left + padding.Right;
            var rectHeight = bufferSize.y * itemHeight + padding.Top + padding.Bottom;
            this.rectangle.x = 0;
            this.rectangle.y = 0;
            this.rectangle.width = rectWidth;
            this.rectangle.height = rectHeight;
        }

        private void UpdateVisibleIndex()
        {
            this.UpdateVisibleIndex(false);
        }

        private void UpdateVisibleIndex(bool fire)
        {
            var visibleIndex = this.visibleIndex;
            this.visibleIndex = Math.Max(this.visibleIndex, this.MinimumVisibleIndex);
            this.visibleIndex = Math.Min(this.visibleIndex, this.MaximumVisibleIndex);
            if (fire == true && this.visibleIndex != visibleIndex)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(VisibleIndex)));
            }
        }

        private void UpdateCursorPoint()
        {
            var x = this.cursorPoint.X;
            var y = this.cursorPoint.Y;
            var bufferWidth = this.bufferWidth;
            var bufferHeight = this.bufferHeight;
            var rowCount = this.rows.Count;
            var maxBufferHeight = Math.Max(bufferHeight, rowCount);
            x = Math.Min(x, bufferWidth - 1);
            x = Math.Max(x, 0);
            y = Math.Min(y, maxBufferHeight - 1);
            y = Math.Max(y, 0);
            this.cursorPoint = new TerminalPoint(x, y);
        }

        private void Terminal_Validated(object sender, EventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.terminal)
            {
                var cursorIndex = this.terminal.CursorIndex;
                this.notifier.Begin();
                this.notifier.SetField(ref this.text, this.terminal.Text, nameof(Text));
                this.characterInfos.Update();
                this.rows.Update();
                this.notifier.SetField(ref this.cursorPoint, this.IndexToPoint(cursorIndex), nameof(CursorPoint));
                this.notifier.SetField(ref this.visibleIndex, TerminalGridValidator.GetVisibleIndex(this), nameof(VisibleIndex));
                this.notifier.End();
                this.Selections.Clear();
            }
        }

        private void Terminal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.terminal)
            {
                switch (e.PropertyName)
                {
                    case nameof(Terminal.CursorPosition):
                        {
                            this.notifier.Begin();
                            this.notifier.SetField(ref this.cursorPoint, this.IndexToPoint(this.terminal.CursorIndex), nameof(CursorPoint));
                            this.notifier.SetField(ref this.visibleIndex, TerminalGridValidator.GetVisibleIndex(this), nameof(VisibleIndex));
                            this.notifier.End();
                            this.Selections.Clear();
                        }
                        break;
                }
            }
        }

        private void Terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.terminal)
            {
                var scroll = this.visibleIndex + this.bufferHeight >= this.cursorPoint.Y;
                this.notifier.Begin();
                this.notifier.SetField(ref this.text, this.terminal.Text, nameof(Text));
                foreach (var item in e.Changes)
                {
                    var time = DateTime.Now;
                    if (item.Length > 0)
                    {
                        this.characterInfos.Update(item.Index);
                        this.rows.Update(item.Index);
                    }
                    else
                    {
                        this.characterInfos.Update(item.Index + item.Length);
                        this.rows.Update(item.Index + item.Length);
                    }
                }
                this.notifier.SetField(ref this.cursorPoint, this.IndexToPoint(this.terminal.CursorIndex), nameof(CursorPoint));
                this.notifier.SetField(ref this.visibleIndex, TerminalGridValidator.GetVisibleIndex(this), nameof(VisibleIndex));
                this.notifier.End();
                this.Selections.Clear();
                if (scroll == true)
                    this.ScrollToCursor();
            }
        }

        private void Terminal_Executed(object sender, TerminalExecutedEventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.terminal)
            {
                this.ScrollToCursor();
            }
        }

        private void Terminal_Worked(object sender, EventArgs e)
        {
            if (this.scrollFlag == true)
            {
                this.ScrollToCursor();
                this.scrollFlag = false;
            }
        }

        private void Object_Validated(object sender, EventArgs e)
        {
            switch (sender)
            {
                case TerminalFont font when font == this.font:
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPoint();
                    this.SetLayoutDirty();
                    break;
                case TerminalFontDescriptor descriptor when this.Font is TerminalFont font && font.DescriptorList.Contains(descriptor) == true:
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPoint();
                    this.SetLayoutDirty();
                    break;
                case TerminalStyle style when style == this.style:
                    this.UpdateColor();
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPoint();
                    this.SetLayoutDirty();
                    break;
            }
        }

        private void TerminalGrid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Style):
                    {
                        this.UpdateColor();
                        this.SetLayoutDirty();
                    }
                    break;
            }
        }

        private void Selections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        private Vector2 GetParentSize()
        {
            if (this.rectTransform.parent is RectTransform parent)
            {
                return parent.sizeDelta;
            }
            return new Vector2(Screen.width, Screen.height);
        }

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var (s1, s2) = p1 < p2 ? (p1, p2) : (p2, p1);
            var x = s1.X;
            var y = s1.Y;
            for (; y <= s2.Y; y++)
            {
                var bufferWidth = y == s2.Y ? s2.X : this.BufferWidth;
                if (bufferWidth >= this.BufferWidth)
                {
                    bufferWidth = this.BufferWidth - 1;
                }
                for (; x <= bufferWidth; x++)
                {
                    yield return this.rows[y].Cells[x];
                }
                x = 0;
            }
        }

        private IEnumerable<TerminalCell> GetCells()
        {
            var query = from row in this.rows
                        from cell in row.Cells
                        select cell;
            return query;
        }

        private string GetString(TerminalRange range)
        {
            var bufferWidth = this.BufferWidth;
            var p1 = range.BeginPoint;
            var p2 = range.EndPoint;
            var (s1, s2) = p1 < p2 ? (p1, p2) : (p2, p1);
            var capacity = (s2.Y - s1.Y + 1) * bufferWidth;
            var list = new List<char>(capacity);
            var sb = new StringBuilder();
            var x = s1.X;
            for (var y = s1.Y; y <= s2.Y; y++)
            {
                var i = 0;
                var count = y == s2.Y ? s2.X : bufferWidth;
                for (; x < count; x++)
                {
                    var point = new TerminalPoint(x, y);
                    var index = this.PointToIndex(point);
                    if (index >= 0)
                    {
                        var charInfo = this.characterInfos[index];
                        var character = charInfo.Character;
                        if (character != char.MinValue)
                        {
                            sb.Append(character);
                            i++;
                        }
                    }
                }
                x = 0;
                if (i == 0 && y > s1.Y)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

        private void SetFocused(bool value)
        {
            if (this.isFocused == value)
                return;
            this.isFocused = value;
            if (this.isFocused == true)
                this.OnGotFocus(EventArgs.Empty);
            else
                this.OnLostFocus(EventArgs.Empty);
            this.InvokePropertyChangedEvent(nameof(IsFocused));
        }

        private IList<Event> PopEvents()
        {
            var eventCount = Event.GetEventCount();
            if (eventCount > 0)
            {
                var eventList = new List<Event>(eventCount);
                for (var i = 0; i < eventCount; i++)
                {
                    var item = new Event();
                    Event.PopEvent(item);
                    eventList.Add(item);
                }
                return eventList;
            }
            else
            {
                return null;
            }
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

        internal void Validate()
        {
            this.UpdateColor();
        }

        #region implemetations

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            Current = this;
            this.InputSystem.imeCompositionMode = IMECompositionMode.On;
            this.SetFocused(true);
            this.InputHandler?.Select(this, eventData);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            Current = null;
            this.SetFocused(false);
            this.InputHandler?.Deselect(this, eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) => this.InputHandler.BeginDrag(this, eventData);

        void IDragHandler.OnDrag(PointerEventData eventData) => this.InputHandler.Drag(this, eventData);

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) => this.InputHandler.EndDrag(this, eventData);

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => this.InputHandler.PointerClick(this, eventData);

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => this.InputHandler.PointerDown(this, eventData);

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => this.InputHandler.PointerUp(this, eventData);

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => this.InputHandler.PointerEnter(this, eventData);

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => this.InputHandler.PointerExit(this, eventData);

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (this.MaximumVisibleIndex > this.MinimumVisibleIndex)
            {
                this.scrollPos -= eventData.scrollDelta.y;
                this.scrollPos = Math.Max(this.scrollPos, this.MinimumVisibleIndex);
                this.scrollPos = Math.Min(this.scrollPos, this.MaximumVisibleIndex);
                this.visibleIndex = (int)this.scrollPos;
                this.IsScrolling = true;
                this.UpdateVisibleIndex();
                this.InvokePropertyChangedEvent(nameof(VisibleIndex));
                this.IsScrolling = false;
            }
        }

        void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
        {
            if (this.PopEvents() is IList<Event> events)
            {
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
                        if (this.OnPreviewKeyDown(modifiers, keyCode) == false)
                        {
                            var keyInfo = new KeyInfo()
                            {
                                KeyCode = item.keyCode,
                                Modifiers = item.modifiers,
                                Character = item.character,
                            };
                            this.eventQueue.Enqueue(keyInfo);
                            this.CompositionString = this.InputSystem != null ? this.InputSystem.compositionString : Input.compositionString;
                        }
                    }
                }
                eventData.Use();
            }
            if (this.terminal.IsExecuting == false)
            {
                this.scrollFlag = true;
                this.ProcessEvent();
            }
            this.InputHandler.Update(this, eventData);
            this.scrollFlag = false;
        }

        #endregion

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
