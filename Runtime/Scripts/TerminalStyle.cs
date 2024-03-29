﻿////////////////////////////////////////////////////////////////////////////////
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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    [CreateAssetMenu(menuName = "Terminal/Style")]
    public class TerminalStyle : ScriptableObject, INotifyValidated, IPropertyChangedNotifyable
    {
        [SerializeField]
        private string styleName;
        [SerializeField]
        private TerminalFont font;
        [SerializeField]
        private Color backgroundColor = TerminalGrid.DefaultBackgroundColor;
        [SerializeField]
        private Color foregroundColor = TerminalGrid.DefaultForegroundColor;
        [SerializeField]
        private Color selectionColor = TerminalGrid.DefaultSelectionColor;
        [SerializeField]
        private Color selectionTextColor = TerminalGrid.DefaultSelectionTextColor;
        [SerializeField]
        private Color cursorColor = TerminalGrid.DefaultCursorColor;
        [SerializeField]
        private Color cursorTextColor = TerminalGrid.DefaultCursorTextColor;
        [SerializeField]
        private Texture2D fallbackTexture;
        [SerializeField]
        private TerminalColorPalette colorPallete;
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

        [FieldName(nameof(styleName))]
        public string StyleName
        {
            get => this.styleName ?? string.Empty;
            set
            {
                if (this.styleName != value)
                {
                    this.styleName = value;
                    this.InvokePropertyChangedEvent(nameof(StyleName));
                }
            }
        }

        [FieldName(nameof(font))]
        public TerminalFont Font
        {
            get => this.font;
            set
            {
                if (this.font != value)
                {
                    this.font = value;
                    this.InvokePropertyChangedEvent(nameof(Font));
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

        [FieldName(nameof(selectionColor))]
        public Color SelectionColor
        {
            get => this.selectionColor;
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
        public Color SelectionTextColor
        {
            get => this.selectionTextColor;
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
        public Color CursorColor
        {
            get => this.cursorColor;
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
        public Color CursorTextColor
        {
            get => this.cursorTextColor;
            set
            {
                if (this.cursorTextColor != value)
                {
                    this.cursorTextColor = value;
                    this.InvokePropertyChangedEvent(nameof(CursorTextColor));
                }
            }
        }

        [FieldName(nameof(fallbackTexture))]
        public Texture2D FallbackTexture
        {
            get => this.fallbackTexture;
            set
            {
                if (this.fallbackTexture != value)
                {
                    this.fallbackTexture = value;
                    this.InvokePropertyChangedEvent(nameof(FallbackTexture));
                }
            }
        }

        [FieldName(nameof(colorPallete))]
        public TerminalColorPalette ColorPalette
        {
            get => this.colorPallete;
            set
            {
                if (this.colorPallete != value)
                {
                    this.colorPallete = value;
                    this.InvokePropertyChangedEvent(nameof(ColorPalette));
                }
            }
        }

        [FieldName(nameof(cursorStyle))]
        public TerminalCursorStyle CursorStyle
        {
            get => this.cursorStyle;
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
        public int CursorThickness
        {
            get => this.cursorThickness;
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
        public bool IsCursorBlinkable
        {
            get => this.isCursorBlinkable;
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
        public float CursorBlinkDelay
        {
            get => this.cursorBlinkDelay;
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
        public bool IsScrollForwardEnabled
        {
            get => this.isScrollForwardEnabled;
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
        public List<TerminalBehaviourBase> BehaviourList => this.behaviourList;

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnValidate()
        {
            this.OnValidated(EventArgs.Empty);
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
            TerminalValidationEvents.Register(this);
            this.OnEnabled(EventArgs.Empty);
        }

        protected virtual void OnDisable()
        {
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

        #region IPropertyChangedNotifyable

        void IPropertyChangedNotifyable.InvokePropertyChangedEvent(string propertyName)
        {
            this.InvokePropertyChangedEvent(propertyName);
        }

        #endregion
    }
}
