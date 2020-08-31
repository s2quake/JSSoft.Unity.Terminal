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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Terminal
{
    [CreateAssetMenu(menuName = "Terminal/Palette")]
    public class TerminalColorPalette : ScriptableObject, INotifyPropertyChanged, IPropertyChangedNotifyable
    {
        [SerializeField]
        private Color black = TerminalColors.Black;
        [SerializeField]
        private Color darkBlue = TerminalColors.DarkBlue;
        [SerializeField]
        private Color darkGreen = TerminalColors.DarkGreen;
        [SerializeField]
        private Color darkCyan = TerminalColors.DarkCyan;
        [SerializeField]
        private Color darkRed = TerminalColors.DarkRed;
        [SerializeField]
        private Color darkMagenta = TerminalColors.DarkMagenta;
        [SerializeField]
        private Color darkYellow = TerminalColors.DarkYellow;
        [SerializeField]
        private Color gray = TerminalColors.Gray;
        [SerializeField]
        private Color darkGray = TerminalColors.DarkGray;
        [SerializeField]
        private Color blue = TerminalColors.Blue;
        [SerializeField]
        private Color green = TerminalColors.Green;
        [SerializeField]
        private Color cyan = TerminalColors.Cyan;
        [SerializeField]
        private Color red = TerminalColors.Red;
        [SerializeField]
        private Color magenta = TerminalColors.Magenta;
        [SerializeField]
        private Color yellow = TerminalColors.Yellow;
        [SerializeField]
        private Color white = TerminalColors.White;

        public Color GetColor(TerminalColor color)
        {
            switch (color)
            {
                case TerminalColor.Black:
                    return this.Black;
                case TerminalColor.DarkBlue:
                    return this.DarkBlue;
                case TerminalColor.DarkGreen:
                    return this.DarkGreen;
                case TerminalColor.DarkCyan:
                    return this.DarkCyan;
                case TerminalColor.DarkRed:
                    return this.DarkRed;
                case TerminalColor.DarkMagenta:
                    return this.DarkMagenta;
                case TerminalColor.DarkYellow:
                    return this.DarkYellow;
                case TerminalColor.Gray:
                    return this.Gray;
                case TerminalColor.DarkGray:
                    return this.DarkGray;
                case TerminalColor.Blue:
                    return this.Blue;
                case TerminalColor.Green:
                    return this.Green;
                case TerminalColor.Cyan:
                    return this.Cyan;
                case TerminalColor.Red:
                    return this.Red;
                case TerminalColor.Magenta:
                    return this.Magenta;
                case TerminalColor.Yellow:
                    return this.Yellow;
                case TerminalColor.White:
                    return this.White;
            }
            throw new NotImplementedException();
        }

        [FieldName(nameof(black))]
        public Color Black
        {
            get => this.black;
            set
            {
                if (this.black != value)
                {
                    this.black = value;
                    this.InvokePropertyChangedEvent(nameof(Black));
                }
            }
        }

        [FieldName(nameof(darkBlue))]
        public Color DarkBlue
        {
            get => this.darkBlue;
            set
            {
                if (this.darkBlue != value)
                {
                    this.darkBlue = value;
                    this.InvokePropertyChangedEvent(nameof(DarkBlue));
                }
            }
        }

        [FieldName(nameof(darkGreen))]
        public Color DarkGreen
        {
            get => this.darkGreen;
            set
            {
                if (this.darkGreen != value)
                {
                    this.darkGreen = value;
                    this.InvokePropertyChangedEvent(nameof(DarkGreen));
                }
            }
        }

        [FieldName(nameof(darkCyan))]
        public Color DarkCyan
        {
            get => this.darkCyan;
            set
            {
                if (this.darkCyan != value)
                {
                    this.darkCyan = value;
                    this.InvokePropertyChangedEvent(nameof(DarkCyan));
                }
            }
        }

        [FieldName(nameof(darkRed))]
        public Color DarkRed
        {
            get => this.darkRed;
            set
            {
                if (this.darkRed != value)
                {
                    this.darkRed = value;
                    this.InvokePropertyChangedEvent(nameof(DarkRed));
                }
            }
        }

        [FieldName(nameof(darkMagenta))]
        public Color DarkMagenta
        {
            get => this.darkMagenta;
            set
            {
                if (this.darkMagenta != value)
                {
                    this.darkMagenta = value;
                    this.InvokePropertyChangedEvent(nameof(DarkMagenta));
                }
            }
        }

        [FieldName(nameof(darkYellow))]
        public Color DarkYellow
        {
            get => this.darkYellow;
            set
            {
                if (this.darkYellow != value)
                {
                    this.darkYellow = value;
                    this.InvokePropertyChangedEvent(nameof(DarkYellow));
                }
            }
        }

        [FieldName(nameof(gray))]
        public Color Gray
        {
            get => this.gray;
            set
            {
                if (this.gray != value)
                {
                    this.gray = value;
                    this.InvokePropertyChangedEvent(nameof(Gray));
                }
            }
        }

        [FieldName(nameof(darkGray))]
        public Color DarkGray
        {
            get => this.darkGray;
            set
            {
                if (this.darkGray != value)
                {
                    this.darkGray = value;
                    this.InvokePropertyChangedEvent(nameof(DarkGray));
                }
            }
        }

        [FieldName(nameof(blue))]
        public Color Blue
        {
            get => this.blue;
            set
            {
                if (this.blue != value)
                {
                    this.blue = value;
                    this.InvokePropertyChangedEvent(nameof(Blue));
                }
            }
        }

        [FieldName(nameof(green))]
        public Color Green
        {
            get => this.green;
            set
            {
                if (this.green != value)
                {
                    this.green = value;
                    this.InvokePropertyChangedEvent(nameof(Green));
                }
            }
        }

        [FieldName(nameof(cyan))]
        public Color Cyan
        {
            get => this.cyan;
            set
            {
                if (this.cyan != value)
                {
                    this.cyan = value;
                    this.InvokePropertyChangedEvent(nameof(Cyan));
                }
            }
        }

        [FieldName(nameof(red))]
        public Color Red
        {
            get => this.red;
            set
            {
                if (this.red != value)
                {
                    this.red = value;
                    this.InvokePropertyChangedEvent(nameof(Red));
                }
            }
        }

        [FieldName(nameof(magenta))]
        public Color Magenta
        {
            get => this.magenta;
            set
            {
                if (this.magenta != value)
                {
                    this.magenta = value;
                    this.InvokePropertyChangedEvent(nameof(Magenta));
                }
            }
        }

        [FieldName(nameof(yellow))]
        public Color Yellow
        {
            get => this.yellow;
            set
            {
                if (this.yellow != value)
                {
                    this.yellow = value;
                    this.InvokePropertyChangedEvent(nameof(Yellow));
                }
            }
        }

        [FieldName(nameof(white))]
        public Color White
        {
            get => this.white;
            set
            {
                if (this.white != value)
                {
                    this.white = value;
                    this.InvokePropertyChangedEvent(nameof(White));
                }
            }
        }

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnValidate()
        {
            this.OnValidated(EventArgs.Empty);
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