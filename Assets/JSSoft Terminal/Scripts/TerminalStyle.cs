﻿// MIT License
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
using System.Collections.Generic;
using System.ComponentModel;
using JSSoft.UI.Fonts;
using UnityEditor;
using UnityEngine;

namespace JSSoft.UI
{
    [CreateAssetMenu(menuName = "Terminal/Create Style")]
    public class TerminalStyle : ScriptableObject, INotifyPropertyChanged
    {
        [SerializeField]
        private TerminalFont font;
        [SerializeField]
        private Color backgroundColor = TerminalGrid.DefaultBackgroundColor;
        [SerializeField]
        private Color foregroundColor = TerminalGrid.DefaultForegroundColor;
        [SerializeField]
        private Color selectionColor = TerminalGrid.DefaultSelectionColor;
        [SerializeField]
        private Color cursorColor = TerminalGrid.DefaultCursorColor;
        [SerializeField]
        private Color compositionColor = TerminalGrid.DefaultCompositionColor;
        [SerializeField]
        private TerminalThickness padding = new TerminalThickness(2);
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
        private MonoBehaviour behaviour;

        public TerminalFont Font
        {
            get => this.font;
            set
            {
                this.font = value;
                this.InvokePropertyChangedEvent(nameof(Font));
            }
        }

        public Color BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                this.backgroundColor = value;
                this.InvokePropertyChangedEvent(nameof(BackgroundColor));
            }
        }

        public Color ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                this.foregroundColor = value;
                this.InvokePropertyChangedEvent(nameof(ForegroundColor));
            }
        }

        public Color SelectionColor
        {
            get => this.selectionColor;
            set
            {
                this.selectionColor = value;
                this.InvokePropertyChangedEvent(nameof(SelectionColor));
            }
        }

        public Color CursorColor
        {
            get => this.cursorColor;
            set
            {
                this.cursorColor = value;
                this.InvokePropertyChangedEvent(nameof(CursorColor));
            }
        }

        public Color CompositionColor
        {
            get => this.compositionColor;
            set
            {
                this.compositionColor = value;
                this.InvokePropertyChangedEvent(nameof(CompositionColor));
            }
        }

        public TerminalThickness Padding
        {
            get => this.padding;
            set
            {
                this.padding = value;
                this.InvokePropertyChangedEvent(nameof(Padding));
            }
        }

        public TerminalCursorStyle CursorStyle
        {
            get => this.cursorStyle;
            set
            {
                this.cursorStyle = value;
                this.InvokePropertyChangedEvent(nameof(CursorStyle));
            }
        }

        public int CursorThickness
        {
            get => this.cursorThickness;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.cursorThickness = value;
                this.InvokePropertyChangedEvent(nameof(CursorThickness));
            }
        }

        public bool IsCursorBlinkable
        {
            get => this.isCursorBlinkable;
            set
            {
                this.isCursorBlinkable = value;
                this.InvokePropertyChangedEvent(nameof(IsCursorBlinkable));
            }
        }

        public float CursorBlinkDelay
        {
            get => this.cursorBlinkDelay;
            set
            {
                if (value < 0.0f)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.cursorBlinkDelay = value;
                this.InvokePropertyChangedEvent(nameof(CursorBlinkDelay));
            }
        }

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnValidate()
        {
            this.OnValidated(EventArgs.Empty);
        }

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnEnable()
        {
            TerminalStyleEvents.Register(this);
        }

        protected virtual void OnDisable()
        {
            TerminalStyleEvents.Unregister(this);
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
    }
}
