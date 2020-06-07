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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.ComponentModel;
using System.Collections.Specialized;
using JSSoft.Terminal.InputHandlers;

namespace JSSoft.Terminal
{
    [RequireComponent(typeof(Terminal))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [SelectionBase]
    class TerminalGrid : TerminalGridBase,
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
        IDeselectHandler
    {
        [SerializeField]
        private TerminalFont font = null;
        [SerializeField]
        [TextArea(5, 10)]
        public string text = string.Empty;
        [SerializeField]
        private Color backgroundColor = DefaultBackgroundColor;
        [SerializeField]
        private Color foregroundColor = DefaultForegroundColor;
        [SerializeField]
        private Color selectionColor = DefaultSelectionColor;
        [SerializeField]
        private Color cursorColor = DefaultCursorColor;
        [SerializeField]
        private TerminalColorPalette colorPalette;
        [SerializeField]
        private int visibleIndex;
        [SerializeField]
        [Range(20, 1000)]
        private int bufferWidth = 10;
        [SerializeField]
        [Range(5, 1000)]
        private int bufferHeight = 2;
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
        [SerializeField]
        private HorizontalAlignment horizontalAlignment;
        [SerializeField]
        private VerticalAlignment verticalAlignment;

        private readonly TerminalRowCollection rows;
        private readonly TerminalCharacterInfoCollection characterInfos;
        private readonly TerminalGridSelection selections;
        private readonly PropertyNotifier notifier;

        private IInputHandler inputHandler;
        private Terminal terminal;
        private bool isFocused;
        private bool isLayoutUpdating;
        private bool isLayoutUpdate;
        private TerminalRange selectingRange = TerminalRange.Empty;
        private float scrollPos;
        private Rect rectangle;
        private IKeyBindingCollection keyBindings;
        private int cursorVolume;
        private int actualBufferWidth;
        private int actualBufferHeight;

        public TerminalGrid()
        {
            this.characterInfos = new TerminalCharacterInfoCollection(this);
            this.rows = new TerminalRowCollection(this, this.characterInfos);
            this.selections = new TerminalGridSelection(this);
            this.selections.CollectionChanged += (s, e) => this.SelectionChanged?.Invoke(this, e);
            this.notifier = new PropertyNotifier(this.InvokePropertyChangedEvent);
        }

        public override Vector2 WorldToGrid(Vector2 position, Camera camera)
        {
            var pixelRect = this.canvas.pixelRect;
            var localPosition = new Vector2(position.x, pixelRect.height - position.y);
            var rect = this.GetRect();
            return localPosition - rect.position;
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
            if (x >= this.ActualBufferWidth || y >= this.rows.Count)
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
            if (point.Y >= 0 && point.Y < this.rows.Count && point.X >= 0 && point.X < this.ActualBufferWidth)
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
            if (EventSystem.current != null)
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
            if (this.CursorPoint.Y >= this.VisibleIndex + this.bufferHeight)
            {
                this.VisibleIndex = this.CursorPoint.Y - this.bufferHeight + 1;
            }
        }

        public override void PageUp()
        {
            this.Scroll(-this.ActualBufferHeight);
        }

        public override void PageDown()
        {
            this.Scroll(this.ActualBufferHeight);
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
            if (visibleIndex >= 0 && visibleIndex <= this.MaximumVisibleIndex)
            {
                this.IsScrolling = true;
                this.VisibleIndex = visibleIndex;
                this.IsScrolling = false;
            }
        }

        public override void Copy()
        {
            if (this.Selections.Any() == true)
            {
                var range = this.Selections.First();
                var p1 = range.BeginPoint;
                var p2 = range.EndPoint;
                var capacity = p1.DistanceOf(p2, this.ActualBufferWidth);
                var list = new List<char>(capacity);
                var i1 = this.PointToIndex(p1);
                var i2 = this.PointToIndex(p2);
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

        public override void SelectAll()
        {
            var p1 = new TerminalPoint(0, 0);
            var p2 = new TerminalPoint(this.ActualBufferWidth, this.rows.Count);
            var range = new TerminalRange(p1, p2);
            this.Selections.Clear();
            this.Selections.Add(range);
        }

        public override IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.Terminal.KeyBindings.TerminalGridKeyBindings.GetDefaultBindings();
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

        public override string Text
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

        public override TerminalFont Font
        {
            get => this.style != null ? this.style.Font : this.font;
            set
            {
                if (this.font != value)
                {
                    this.font = value;
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPoint();
                    this.InvokePropertyChangedEvent(nameof(Font));
                    this.SetLayoutDirty();
                }
            }
        }

        public override int BufferWidth
        {
            get => this.bufferWidth;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.bufferWidth != value)
                {
                    this.bufferWidth = value;
                    this.InvokePropertyChangedEvent(nameof(BufferWidth));
                    this.CursorPoint = this.IndexToPoint(this.terminal.CursorIndex);
                    this.UpdateVisibleIndex();
                    this.SetLayoutDirty();
                }
            }
        }

        public override int BufferHeight
        {
            get => this.bufferHeight;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.bufferHeight != value)
                {
                    this.bufferHeight = value;
                    this.InvokePropertyChangedEvent(nameof(BufferHeight));
                    this.CursorPoint = this.IndexToPoint(this.terminal.CursorIndex);
                    this.UpdateVisibleIndex();
                    this.SetLayoutDirty();
                }
            }
        }

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

        public override int ActualBufferWidth => this.actualBufferWidth;

        public override int ActualBufferHeight => this.actualBufferHeight;

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
                    this.UpdateVisibleIndex();
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
                    return Math.Max(this.rows.MinimumIndex, this.rows.MaximumIndex - this.ActualBufferHeight);
                return Math.Max(this.rows.MaximumIndex, this.maxBufferHeight) - this.ActualBufferHeight;
            }
        }

        public int CursorVisibleIndex => Math.Max(this.rows.MinimumIndex, this.rows.MaximumIndex - this.ActualBufferHeight);

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

        public TerminalColorPalette ColorPalette
        {
            get => this.style != null ? this.style.ColorPalette : this.colorPalette;
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

        public override TerminalThickness Padding
        {
            get => this.style != null ? this.style.Padding : this.padding;
            set
            {
                if (this.padding != value)
                {
                    this.padding = value;
                    this.InvokePropertyChangedEvent(nameof(Padding));
                }
            }
        }

        public override TerminalStyle Style
        {
            get => this.style;
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    this.UpdateColor();
                    this.InvokePropertyChangedEvent(nameof(Style));
                    this.SetLayoutDirty();
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

        public override IList<TerminalBehaviourBase> BehaviourList => this.behaviourList;

        public override IList<TerminalRange> Selections => this.selections;

        public override TerminalDispatcher Dispatcher => this.terminal?.Dispatcher;

        public override event EventHandler LayoutChanged;

        public override event NotifyCollectionChangedEventHandler SelectionChanged;

        public override event EventHandler GotFocus;

        public override event EventHandler LostFocus;

        public override event EventHandler Validated;

        public override event EventHandler Enabled;

        public override event EventHandler Disabled;

        public override event PropertyChangedEventHandler PropertyChanged;

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
            var pos = Vector2.zero;
            var pivot = RectTransformUtility.GetPivot(this.horizontalAlignment, this.verticalAlignment);
            var bufferSize = TerminalGridUtility.GetActualBufferSize(this, this.horizontalAlignment, this.verticalAlignment);
            var size = TerminalGridUtility.GetActualSize(this, this.horizontalAlignment, this.verticalAlignment);
            var (min, max) = RectTransformUtility.GetAnchor(this.horizontalAlignment, this.verticalAlignment);
            this.isLayoutUpdate = false;
            this.isLayoutUpdating = true;
            this.rectTransform.anchorMin = min;
            this.rectTransform.anchorMax = max;
            this.rectTransform.pivot = pivot;
            this.rectTransform.anchoredPosition = pos;
            this.UpdateRectangle(bufferSize);
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            this.notifier.Begin();
            this.notifier.SetField(ref this.actualBufferWidth, (int)bufferSize.x, nameof(ActualBufferWidth));
            this.notifier.SetField(ref this.actualBufferHeight, (int)bufferSize.y, nameof(ActualBufferHeight));
            this.notifier.End();
            this.isLayoutUpdating = false;
            this.UpdateVisibleIndex();
            this.OnLayoutChanged(EventArgs.Empty);
            Debug.Log($"OnLayoutChanged: {this.verticalAlignment}");
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

        protected virtual void OnEnabled(EventArgs e)
        {
            this.Enabled?.Invoke(this, e);
        }

        protected virtual void OnDisabled(EventArgs e)
        {
            this.Disabled?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.terminal = this.GetComponent<Terminal>();
            this.ValidateValue();
            this.UpdateColor();
            this.UpdateCursorPoint();
            this.UpdateBufferSize();
            this.UpdateVisibleIndex();
            this.OnValidated(EventArgs.Empty);
            Debug.Log("OnValidated");
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            // Debug.Log("OnRectTransformDimensionsChange");
            this.terminal = this.GetComponent<Terminal>();
            base.OnRectTransformDimensionsChange();
            if (this.isLayoutUpdate == true)
            {
                this.UpdateLayout();
            }
            else if (this.isLayoutUpdating == false)
            {
                var horzAlign = RectTransformUtility.GetHorizontalAlignment(this.rectTransform);
                var vertAlign = RectTransformUtility.GetVerticalAlignment(this.rectTransform);
                if ((horzAlign != null && vertAlign != null))
                {
                    if (this.horizontalAlignment != horzAlign.Value || this.verticalAlignment != vertAlign.Value)
                    {
                        this.horizontalAlignment = horzAlign.Value;
                        this.verticalAlignment = vertAlign.Value;
                        this.isLayoutUpdate = true;
                        this.SetLayoutDirty();
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.inputHandler = InputHandlerInstances.DefaultHandler;
            this.inputHandler.Attach(this);
            this.isLayoutUpdate = true;
            // this.rectTransform.hideFlags |= HideFlags.HideInInspector;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.terminal = this.GetComponent<Terminal>();
            TerminalGridEvents.Register(this);
            TerminalEvents.Validated += Terminal_Validated;
            TerminalEvents.PropertyChanged += Terminal_PropertyChanged;
            TerminalValidationEvents.Validated += Object_Validated;
            this.OnEnabled(EventArgs.Empty);
            Debug.Log("OnEnabled");
        }

        protected override void OnDisable()
        {
            TerminalEvents.Validated -= Terminal_Validated;
            TerminalEvents.PropertyChanged += Terminal_PropertyChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
            this.terminal = null;
            base.OnDisable();
            this.OnDisabled(EventArgs.Empty);
            TerminalGridEvents.Unregister(this);
            Debug.Log("OnDisabled");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.inputHandler?.Detach(this);
        }

        protected virtual bool OnPreviewKeyDown(EventModifiers modifiers, KeyCode keyCode)
        {
            if (this.terminal.ProcessKeyEvent(modifiers, keyCode) == true)
                return true;
            return this.KeyBindings.Process(this, modifiers, keyCode);
        }

        protected virtual bool OnPreviewKeyPress(char character)
        {
            // 27: escape key
            if (character == '\n' || character == '\t' || character == 27)
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

        private void ValidateValue()
        {
            this.maxBufferHeight = Math.Max(this.bufferHeight, this.maxBufferHeight);
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
            var itemWidth = TerminalGridUtility.GetItemWidth(this);
            var itemHeight = TerminalGridUtility.GetItemHeight(this);
            var actualWidth = (int)(rect.width / itemWidth);
            var actualHeight = (int)(rect.height / itemHeight);
            var rectWidth = bufferSize.x * itemWidth + this.Padding.Left + this.Padding.Right;
            var rectHeight = bufferSize.y * itemHeight + this.Padding.Top + this.Padding.Bottom;
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
            var bufferHeight = this.actualBufferHeight;
            var rowCount = this.rows.Count;
            var maxBufferHeight = Math.Max(bufferHeight, rowCount);
            x = Math.Min(x, bufferHeight - 1);
            x = Math.Max(x, 0);
            y = Math.Min(y, maxBufferHeight - 1);
            y = Math.Max(y, 0);
            this.cursorPoint = new TerminalPoint(x, y);
        }

        private void UpdateBufferSize()
        {
            var bufferSize = TerminalGridUtility.GetActualBufferSize(this, this.horizontalAlignment, this.verticalAlignment);
            this.actualBufferWidth = (int)bufferSize.x;
            this.actualBufferHeight = (int)bufferSize.y;
        }

        private void Terminal_Validated(object sender, EventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.terminal)
            {
                var index = this.terminal.CursorIndex;
                this.text = this.terminal.Text;
                this.characterInfos.Update();
                this.rows.Update();
                this.UpdateVisibleIndex();
                this.CursorPoint = this.IndexToPoint(index);
                this.Selections.Clear();
                this.ScrollToCursor();
            }
        }

        private void Terminal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.terminal)
            {
                switch (e.PropertyName)
                {
                    case nameof(Terminal.OutputText):
                        {
                            this.Text = this.terminal.Text;
                            this.CursorPoint = this.IndexToPoint(this.terminal.CursorIndex);
                            this.Selections.Clear();
                            this.UpdateVisibleIndex(true);
                        }
                        break;
                    case nameof(Terminal.PromptText):
                        {
                            this.Text = this.terminal.Text;
                            this.CursorPoint = this.IndexToPoint(this.terminal.CursorIndex);
                            this.Selections.Clear();
                            this.UpdateVisibleIndex(true);
                            this.ScrollToCursor();
                        }
                        break;
                    case nameof(Terminal.CursorPosition):
                        {
                            var index = this.terminal.CursorIndex;
                            this.CursorPoint = this.IndexToPoint(index);
                            this.Selections.Clear();
                            this.UpdateVisibleIndex(true);
                            this.ScrollToCursor();
                        }
                        break;
                    case nameof(Terminal.Text):
                        {
                            this.ScrollToCursor();
                        }
                        break;
                }
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
                case TerminalFontDescriptor descriptor when this.Font is TerminalFont font && font.Descriptors.Contains(descriptor) == true:
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

        private Vector2 GetParentSize()
        {
            if (this.rectTransform.parent is RectTransform parent)
            {
                return parent.sizeDelta;
            }
            return new Vector2(Screen.width, Screen.height);
        }

        private Vector2 CalculateBufferSize()
        {
            var bufferSize = new Vector2(this.bufferWidth, this.bufferHeight);
            var parentSize = this.GetParentSize();
            if (this.horizontalAlignment == HorizontalAlignment.Stretch)
            {
                var itemWidth = TerminalGridUtility.GetItemWidth(this);
                bufferSize.x = (int)(parentSize.x - (this.padding.Left + this.padding.Right)) / itemWidth;
            }
            if (this.verticalAlignment == VerticalAlignment.Stretch)
            {
                var itemHeight = TerminalGridUtility.GetItemHeight(this);
                bufferSize.y = (int)(parentSize.y - (this.padding.Top + this.padding.Bottom)) / itemHeight;
            }
            return new Vector2(this.bufferWidth, this.bufferHeight);
        }

        private IEnumerable<TerminalCell> GetCells(TerminalPoint p1, TerminalPoint p2)
        {
            var (s1, s2) = p1 < p2 ? (p1, p2) : (p2, p1);
            var x = s1.X;
            var y = s1.Y;
            for (; y <= s2.Y; y++)
            {
                var bufferWidth = y == s2.Y ? s2.X : this.ActualBufferWidth;
                if (bufferWidth >= this.ActualBufferWidth)
                {
                    bufferWidth = this.ActualBufferWidth - 1;
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

        private void SetFocused(bool value)
        {
            if (this.isFocused == value)
                return;
            this.isFocused = value;
            if (this.isFocused == true)
                this.OnGotFocus(EventArgs.Empty);
            else
                this.OnLostFocus(EventArgs.Empty);
        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
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

        #region implemetations

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            this.InputSystem.imeCompositionMode = IMECompositionMode.On;
            this.SetFocused(true);
            this.InputHandler.Select(this, eventData);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            this.SetFocused(false);
            this.InputHandler.Deselect(this, eventData);
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
        private System.Random r = new System.Random();
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
                        var k = r.Next(0, 10);
                        // if (k < 3)
                        //     Debug.LogWarning($"{modifiers} + {keyCode}: {item.character}");
                        // else if (k < 9)
                        //     Debug.LogError($"{modifiers} + {keyCode}: {item.character}");
                        // else
                        //     Debug.Log($"{modifiers} + {keyCode}: {item.character}");
                        if (this.OnPreviewKeyDown(modifiers, keyCode) == true)
                            continue;
                        if (item.character != 0 && this.OnPreviewKeyPress(item.character) == false)
                        {
                            if (this.terminal.IsReadOnly == false && this.terminal.IsExecuting == false)
                                this.terminal.InsertCharacter(item.character);
                        }
                        this.CompositionString = this.InputSystem != null ? this.InputSystem.compositionString : Input.compositionString;
                    }
                }

                eventData.Use();
            }
            this.InputHandler.Update(this, eventData);
        }

        // ITerminal ITerminalGrid.Terminal => this.terminal;

        // IList<TerminalRange> ITerminalGrid.Selections => this.Selections;

        // GameObject ITerminalGrid.GameObject => this.gameObject;

        #endregion
    }
}
