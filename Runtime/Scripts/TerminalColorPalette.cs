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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    [CreateAssetMenu(menuName = "Terminal/Palette")]
    public class TerminalColorPalette : ScriptableObject, INotifyPropertyChanged, IPropertyChangedNotifyable
    {
        [SerializeField]
        private Color black = TerminalColors.Black;
        [SerializeField]
        private Color red = TerminalColors.Red;
        [SerializeField]
        private Color green = TerminalColors.Green;
        [SerializeField]
        private Color yellow = TerminalColors.Yellow;
        [SerializeField]
        private Color blue = TerminalColors.Blue;
        [SerializeField]
        private Color magenta = TerminalColors.Magenta;
        [SerializeField]
        private Color cyan = TerminalColors.Cyan;
        [SerializeField]
        private Color white = TerminalColors.White;
        [SerializeField]
        private Color brightBlack = TerminalColors.BrightBlack;
        [SerializeField]
        private Color brightRed = TerminalColors.BrightRed;
        [SerializeField]
        private Color brightBlue = TerminalColors.BrightBlue;
        [SerializeField]
        private Color brightGreen = TerminalColors.BrightGreen;
        [SerializeField]
        private Color brightYellow = TerminalColors.BrightYellow;
        [SerializeField]
        private Color brightMagenta = TerminalColors.BrightMagenta;
        [SerializeField]
        private Color brightCyan = TerminalColors.BrightCyan;
        [SerializeField]
        private Color brightWhite = TerminalColors.BrightWhite;

        public Color GetColor(TerminalColor color)
        {
            switch (color)
            {
                case TerminalColor.Black:
                    return this.Black;
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
                case TerminalColor.BrightBlack:
                    return this.BrightBlack;
                case TerminalColor.White:
                    return this.White;
                case TerminalColor.BrightBlue:
                    return this.BrightBlue;
                case TerminalColor.BrightGreen:
                    return this.BrightGreen;
                case TerminalColor.BrightCyan:
                    return this.BrightCyan;
                case TerminalColor.BrightRed:
                    return this.BrightRed;
                case TerminalColor.BrightMagenta:
                    return this.BrightMagenta;
                case TerminalColor.BrightYellow:
                    return this.BrightYellow;
                case TerminalColor.BrightWhite:
                    return this.BrightWhite;
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

        [FieldName(nameof(brightBlack))]
        public Color BrightBlack
        {
            get => this.brightBlack;
            set
            {
                if (this.brightBlack != value)
                {
                    this.brightBlack = value;
                    this.InvokePropertyChangedEvent(nameof(BrightBlack));
                }
            }
        }

        [FieldName(nameof(brightRed))]
        public Color BrightRed
        {
            get => this.brightRed;
            set
            {
                if (this.brightRed != value)
                {
                    this.brightRed = value;
                    this.InvokePropertyChangedEvent(nameof(BrightRed));
                }
            }
        }

        [FieldName(nameof(brightGreen))]
        public Color BrightGreen
        {
            get => this.brightGreen;
            set
            {
                if (this.brightGreen != value)
                {
                    this.brightGreen = value;
                    this.InvokePropertyChangedEvent(nameof(BrightGreen));
                }
            }
        }

        [FieldName(nameof(brightYellow))]
        public Color BrightYellow
        {
            get => this.brightYellow;
            set
            {
                if (this.brightYellow != value)
                {
                    this.brightYellow = value;
                    this.InvokePropertyChangedEvent(nameof(BrightYellow));
                }
            }
        }

        [FieldName(nameof(brightBlue))]
        public Color BrightBlue
        {
            get => this.brightBlue;
            set
            {
                if (this.brightBlue != value)
                {
                    this.brightBlue = value;
                    this.InvokePropertyChangedEvent(nameof(BrightBlue));
                }
            }
        }

        [FieldName(nameof(brightMagenta))]
        public Color BrightMagenta
        {
            get => this.brightMagenta;
            set
            {
                if (this.brightMagenta != value)
                {
                    this.brightMagenta = value;
                    this.InvokePropertyChangedEvent(nameof(BrightMagenta));
                }
            }
        }

        [FieldName(nameof(brightCyan))]
        public Color BrightCyan
        {
            get => this.brightCyan;
            set
            {
                if (this.brightCyan != value)
                {
                    this.brightCyan = value;
                    this.InvokePropertyChangedEvent(nameof(BrightCyan));
                }
            }
        }

        [FieldName(nameof(brightWhite))]
        public Color BrightWhite
        {
            get => this.brightWhite;
            set
            {
                if (this.brightWhite != value)
                {
                    this.brightWhite = value;
                    this.InvokePropertyChangedEvent(nameof(BrightWhite));
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