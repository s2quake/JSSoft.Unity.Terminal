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
using System.Threading.Tasks;
using System.ComponentModel;
#if UNITY_EDITOR
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor")]
#endif

namespace JSSoft.UI
{
    [RequireComponent(typeof(Terminal))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [SelectionBase]
    class TerminalGrid : MaskableGraphic, ITerminalGrid, INotifyPropertyChanged,
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
        public static readonly Color DefaultBackgroundColor = new Color32(23, 23, 23, 255);
        public static readonly Color DefaultForegroundColor = new Color32(255, 255, 255, 255);
        public static readonly Color DefaultSelectionColor = new Color32(49, 79, 129, 255);
        public static readonly Color DefaultCursorColor = new Color32(139, 139, 139, 255);
        public static readonly Color DefaultCompositionColor = new Color32(255, 255, 255, 255);
        public static readonly Color DefaultScrollbarColor = new Color32(139, 139, 139, 0);

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
        private Color compositionColor = DefaultCompositionColor;
        [SerializeField]
        private TerminalColorPalette colorPalette;
        [SerializeField]
        private int visibleIndex;
        [SerializeField]
        [Range(20, 1000)]
        private int bufferWidth = 80;
        [SerializeField]
        [Range(5, 1000)]
        private int bufferHeight = 25;

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
        private TerminalBehaviourBase behaviour;

        private readonly TerminalEventCollection events = new TerminalEventCollection();
        private readonly TerminalRowCollection rows;
        private readonly TerminalCharacterInfoCollection characterInfos;

        private IInputHandler inputHandler;
        private Terminal terminal;
        private bool isFocused;
        private TerminalRange selectingRange = TerminalRange.Empty;
        private float scrollPos;
        private Rect rectangle;
        private IKeyBindingCollection keyBindings;
        private int cursorVolume;

        public TerminalGrid()
        {
            this.characterInfos = new TerminalCharacterInfoCollection(this);
            this.rows = new TerminalRowCollection(this, this.characterInfos);
            this.Selections = new TerminalGridSelection(() => this.OnSelectionChanged(EventArgs.Empty));
        }

        public Vector2 WorldToGrid(Vector2 position)
        {
            var rect = this.rectTransform.rect;
            var camera = this.GetComponentInParent<Canvas>().worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, position, camera, out var localPosition);
            localPosition.y = rect.height - localPosition.y;
            localPosition.y += TerminalGridUtility.GetItemHeight(this) * this.visibleIndex;
            localPosition.x += (int)this.Rectangle.width / 2;
            localPosition.y -= (int)this.Rectangle.height / 2;
            return localPosition;
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

        public TerminalPoint IndexToPoint(int index) => this.characterInfos[index].Point;

        public int PointToIndex(TerminalPoint point) => this.characterInfos.PointToIndex(point);

        public Color32? IndexToBackgroundColor(int index)
        {
            var pallete = this.ColorPalette;
            var color = this.Terminal.GetBackgroundColor(index);
            if (color == null)
                return null;
            if (pallete != null)
                return pallete.GetColor(color.Value);
            return TerminalColors.GetColor(color.Value);
        }

        public Color32? IndexToForegroundColor(int index)
        {
            var pallete = this.ColorPalette;
            var color = this.Terminal.GetForegroundColor(index);
            if (color == null)
                return null;
            if (pallete != null)
                return pallete.GetColor(color.Value);
            return TerminalColors.GetColor(color.Value);
        }

        public TerminalCell GetCell(TerminalPoint point)
        {
            if (point.Y < 0 || point.Y >= this.rows.Count)
                return null;
            var row = this.rows[point.Y];
            if (point.X < 0 || point.X >= row.Cells.Count)
                return null;
            return row.Cells[point.X];
        }

        public void Append(string text)
        {
            this.Append(text, this.text.Length);
        }

        public void Append(string text, int index)
        {
            this.text = this.text.Insert(index, text);
            this.InvokePropertyChangedEvent(nameof(Text));
        }

        public void Remove(int startIndex, int length)
        {
            this.text.Remove(startIndex, length);
            this.InvokePropertyChangedEvent(nameof(Text));
        }

        public void Focus()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
        }

        public void ScrollToTop()
        {
            if (this.MaximumVisibleIndex > 0)
            {
                this.IsScrolling = true;
                this.VisibleIndex = 0;
                this.IsScrolling = false;
            }
        }

        public void ScrollToBottom()
        {
            if (this.MaximumVisibleIndex > 0)
            {
                this.IsScrolling = true;
                this.VisibleIndex = this.MaximumVisibleIndex;
                this.IsScrolling = false;
            }
        }

        public void PageUp()
        {
            this.Scroll(-this.BufferHeight);
        }

        public void PageDown()
        {
            this.Scroll(this.BufferHeight);
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
                this.IsScrolling = true;
                this.VisibleIndex = visibleIndex;
                this.IsScrolling = false;
            }
        }

        public void Copy()
        {
            if (this.Selections.Any() == true)
            {
                var range = this.Selections.First();
                var p1 = range.BeginPoint;
                var p2 = range.EndPoint;
                var capacity = p1.DistanceOf(p2, this.BufferWidth);
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
            var p1 = new TerminalPoint(0, 0);
            var p2 = new TerminalPoint(this.BufferWidth, this.rows.Count);
            var range = new TerminalRange(p1, p2);
            this.Selections.Clear();
            this.Selections.Add(range);
            this.OnSelectionChanged(EventArgs.Empty);
        }

        public IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.UI.KeyBindings.TerminalGridKeyBindings.GetDefaultBindings();
            set => this.keyBindings = value;
        }

        public IInputHandler InputHandler
        {
            get => this.inputHandler ?? JSSoft.UI.InputHandler.GetDefaultHandler();
            set => this.inputHandler = value;
        }

        public Terminal Terminal => this.terminal;

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
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.text != value)
                {
                    this.text = value;
                    this.InvokePropertyChangedEvent(nameof(Text));
                }
            }
        }

        public TerminalFont Font
        {
            get => this.style != null ? this.style.Font : this.font;
            set
            {
                if (this.font != value)
                {
                    this.font = value;
                    this.UpdateLayout();
                    this.InvokePropertyChangedEvent(nameof(Font));
                }
            }
        }

        public int BufferWidth
        {
            get => this.bufferWidth;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.bufferWidth = value;
                this.InvokePropertyChangedEvent(nameof(BufferWidth));
            }
        }

        public int BufferHeight
        {
            get => this.bufferHeight;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.bufferHeight = value;
                this.InvokePropertyChangedEvent(nameof(BufferHeight));
            }
        }

        public IReadOnlyList<ITerminalRow> Rows => this.rows;

        public IReadOnlyList<TerminalCharacterInfo> CharacterInfos => this.characterInfos;

        public TerminalGridSelection Selections { get; }

        public int VisibleIndex
        {
            get => this.visibleIndex;
            set
            {
                if (value < 0 || value > this.MaximumVisibleIndex)
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

        public int MaximumVisibleIndex
        {
            get
            {
                if (this.visibleIndex < 0 || this.Rows.Count < this.BufferHeight)
                    return 0;
                return this.Rows.Count - this.BufferHeight;
            }
        }

        public Color BackgroundColor
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

        public Color ForegroundColor
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

        public Color SelectionColor
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

        public Color CursorColor
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

        public Color CompositionColor
        {
            get => this.style != null ? this.style.CompositionColor : this.compositionColor;
            set
            {
                if (this.compositionColor != value)
                {
                    this.compositionColor = value;
                    this.InvokePropertyChangedEvent(nameof(CompositionColor));
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

        public TerminalPoint CursorPoint
        {
            get => this.cursorPoint;
            set
            {
                if (this.cursorPoint != value)
                {
                    this.cursorPoint = value;
                    this.UpdateCursorPosition();
                    this.InvokePropertyChangedEvent(nameof(CursorPoint));
                }
                this.Selections.Clear();
            }
        }

        public Rect Rectangle => this.rectangle;

        public TerminalThickness Padding
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

        public TerminalStyle Style
        {
            get => this.style;
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    this.UpdateColor();
                    this.UpdateLayout();
                    this.InvokePropertyChangedEvent(nameof(Style));
                }
            }
        }

        public bool IsCursorVisible
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

        public bool IsScrolling { get; set; }

        public TerminalRange SelectingRange
        {
            get => this.selectingRange;
            set
            {
                if (this.selectingRange != value)
                {
                    this.selectingRange = value;
                    this.OnLayoutChanged(EventArgs.Empty);
                }
            }
        }

        public string CompositionString
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

        public TerminalCursorStyle CursorStyle
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

        public int CursorThickness
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

        public bool IsCursorBlinkable
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

        public float CursorBlinkDelay
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

        public event EventHandler LayoutChanged;

        public event EventHandler SelectionChanged;

        public event EventHandler GotFocus;

        public event EventHandler LostFocus;

        public event EventHandler Validated;

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnLayoutChanged(EventArgs e)
        {
            this.LayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, EventArgs.Empty);
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
            this.UpdateColor();
            this.UpdateLayout();
            this.UpdateVisibleIndex();
            this.UpdateCursorPosition();
            this.OnValidated(EventArgs.Empty);
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.terminal = this.GetComponent<Terminal>();
            this.UpdateLayout();
            this.UpdateVisibleIndex();
            this.UpdateCursorPosition();
            this.OnLayoutChanged(EventArgs.Empty);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.terminal = this.GetComponent<Terminal>();
            this.VisibleIndex = this.MaximumVisibleIndex;
            TerminalGridEvents.Register(this);
            TerminalEvents.Validated += Terminal_Validated;
            TerminalEvents.OutputTextChanged += Terminal_OutputTextChanged;
            TerminalEvents.PromptTextChanged += Terminal_PromptTextChanged;
            TerminalEvents.CursorPositionChanged += Terminal_CursorPositionChanged;
            TerminalValidationEvents.Validated += Object_Validated;
            this.OnEnabled(EventArgs.Empty);
        }

        protected override void OnDisable()
        {
            TerminalEvents.Validated -= Terminal_Validated;
            TerminalEvents.OutputTextChanged -= Terminal_OutputTextChanged;
            TerminalEvents.PromptTextChanged -= Terminal_PromptTextChanged;
            TerminalEvents.CursorPositionChanged -= Terminal_CursorPositionChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
            base.OnDisable();
            this.OnDisabled(EventArgs.Empty);
            TerminalGridEvents.Unregister(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected virtual bool OnPreviewKeyDown(EventModifiers modifiers, KeyCode keyCode)
        {
            if (this.Terminal.ProcessKeyEvent(modifiers, keyCode) == true)
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

        private void UpdateLayout()
        {
            var rect = this.GetComponent<RectTransform>().rect;
            var itemWidth = TerminalGridUtility.GetItemWidth(this);
            var itemHeight = TerminalGridUtility.GetItemHeight(this);
            var bufferWidth = (int)(rect.width / itemWidth);
            var bufferHeight = (int)(rect.height / itemHeight);
            var rectWidth = this.BufferWidth * itemWidth + this.Padding.Left + this.Padding.Right;
            var rectHeight = this.BufferHeight * itemHeight + this.Padding.Top + this.Padding.Bottom;
            this.rectangle.x = 0;
            this.rectangle.y = 0;
            this.rectangle.width = rectWidth;
            this.rectangle.height = rectHeight;
            this.Invoke(nameof(UpdateRectTransform), Time.deltaTime);
        }

        private void UpdateRectTransform()
        {
            this.rectTransform.sizeDelta = this.rectangle.size;
        }

        private void UpdateVisibleIndex()
        {
            if (this.visibleIndex < 0 || this.Rows.Count < this.BufferHeight)
                this.visibleIndex = 0;
            else
                this.visibleIndex = Math.Min(this.visibleIndex, this.Rows.Count - this.BufferHeight);
        }

        private void UpdateCursorPosition()
        {
            var x = this.cursorPoint.X;
            var y = this.cursorPoint.Y;
            var maxBufferHeight = Math.Max(this.BufferHeight, this.rows.Count);
            x = Math.Min(x, this.BufferWidth - 1);
            x = Math.Max(x, 0);
            y = Math.Min(y, maxBufferHeight - 1);
            y = Math.Max(y, 0);
            this.cursorPoint = new TerminalPoint(x, y);
        }

        private void Terminal_Validated(object sender, EventArgs e)
        {
            if (sender is Terminal terminal == this.terminal)
            {
                var index = this.terminal.CursorPosition + this.terminal.OutputText.Length + this.terminal.Prompt.Length;
                this.text = this.terminal.Text;
                this.characterInfos.Update();
                this.rows.Update();
                this.cursorPoint = this.IndexToPoint(index);
                this.InvokePropertyChangedEvent(nameof(CursorPoint));
            }
        }

        private void Terminal_OutputTextChanged(object sender, EventArgs e)
        {
            if (sender is Terminal terminal == this.terminal)
            {
                this.Text = this.Terminal.Text;
            }
        }

        private void Terminal_PromptTextChanged(object sender, EventArgs e)
        {
            if (sender is Terminal terminal == this.terminal)
            {
                this.Text = this.Terminal.Text;
                this.VisibleIndex = this.MaximumVisibleIndex;
            }
        }

        private void Terminal_CursorPositionChanged(object sender, EventArgs e)
        {
            if (sender is Terminal terminal == this.terminal)
            {
                var index = this.Terminal.CursorPosition + this.Terminal.OutputText.Length + this.Terminal.Prompt.Length;
                this.CursorPoint = this.IndexToPoint(index);
                this.VisibleIndex = this.MaximumVisibleIndex;
                Debug.Log(this.CursorPoint);
            }
        }

        private void Object_Validated(object sender, EventArgs e)
        {
            switch (sender)
            {
                case TerminalFont font when font == this.font:
                    this.UpdateLayout();
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPosition();
                    break;
                case TerminalFontDescriptor descriptor when this.Font is TerminalFont font && font.Descriptors.Contains(descriptor) == true:
                    this.UpdateLayout();
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPosition();
                    break;
                case TerminalStyle style when style == this.style:
                    this.UpdateColor();
                    this.UpdateLayout();
                    this.UpdateVisibleIndex();
                    this.UpdateCursorPosition();
                    break;
            }
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

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
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
            if (this.InputHandler.BeginDrag(this, eventData) == true)
                return;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (this.InputHandler.Drag(this, eventData) == true)
                return;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (this.InputHandler.EndDrag(this, eventData) == true)
                return;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (this.InputHandler.PointerClick(this, eventData) == true)
                return;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (this.InputHandler.PointerDown(this, eventData) == true)
                return;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (this.InputHandler.PointerUp(this, eventData) == true)
                return;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (this.InputHandler.PointerEnter(this, eventData) == true)
                return;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (this.InputHandler.PointerExit(this, eventData) == true)
                return;
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (this.MaximumVisibleIndex > 0)
            {
                this.scrollPos -= eventData.scrollDelta.y;
                this.scrollPos = Math.Max((int)this.scrollPos, 0);
                this.scrollPos = Math.Min((int)this.scrollPos, this.MaximumVisibleIndex);
                this.visibleIndex = (int)this.scrollPos;
                this.IsScrolling = true;
                this.UpdateVisibleIndex();
                this.InvokePropertyChangedEvent(nameof(VisibleIndex));
                this.IsScrolling = false;
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
                    this.CompositionString = this.InputSystem != null ? this.InputSystem.compositionString : Input.compositionString;
                }
            }

            eventData.Use();
        }

        ITerminal ITerminalGrid.Terminal => this.terminal;

        IList<TerminalRange> ITerminalGrid.Selections => this.Selections;

        GameObject ITerminalGrid.GameObject => this.gameObject;

        #endregion
    }
}
