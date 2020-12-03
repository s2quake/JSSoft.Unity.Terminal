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
using System.Linq;
using System.Collections.Generic;
using JSSoft.Unity.Terminal.Fonts;
using UnityEngine;
using System.ComponentModel;

namespace JSSoft.Unity.Terminal
{
    [CreateAssetMenu(menuName = "Terminal/Font")]
    [DefaultExecutionOrder(-200)]
    public class TerminalFont : ScriptableObject, INotifyValidated, IPropertyChangedNotifyable
    {
        [SerializeField]
        private List<TerminalFontDescriptor> descriptorList = new List<TerminalFontDescriptor>();
        [SerializeField]
        private int width = FontUtility.DefaultItemWidth;
        [SerializeField]
        private int height = FontUtility.DefaultItemHeight;
        [SerializeField]
        private int line = FontUtility.DefaultItemHeight;

        public TerminalFont()
        {
        }

        public bool Contains(char character)
        {
            foreach (var item in this.DescriptorList)
            {
                if (item is TerminalFontDescriptor descriptor && descriptor.Contains(character) == true)
                    return true;
            }
            return false;
        }

        public TerminalFontDescriptor GetDescriptor(char character)
        {
            foreach (var item in this.DescriptorList)
            {
                if (item is TerminalFontDescriptor descriptor && descriptor.Contains(character) == true)
                    return descriptor;
            }
            return null;
        }

        public void UpdateSize()
        {
            var mainFont = this.DescriptorList.FirstOrDefault();
            this.width = mainFont != null ? mainFont.Width : FontUtility.DefaultItemWidth;
            this.height = mainFont != null ? mainFont.Height : FontUtility.DefaultItemHeight;
            this.line = mainFont != null ? mainFont.CommonInfo.BaseLine : FontUtility.DefaultItemHeight;
            this.InvokePropertyChangedEvent(nameof(this.Width));
            this.InvokePropertyChangedEvent(nameof(this.Height));
            this.InvokePropertyChangedEvent(nameof(this.Line));
        }

        public CharInfo this[char character]
        {
            get
            {
                foreach (var item in this.DescriptorList)
                {
                    if (item is TerminalFontDescriptor descriptor && descriptor.Contains(character) == true)
                        return descriptor[character];
                }
                return new CharInfo()
                {
                    XAdvance = this.width,
                    ID = (int)character,
                };
            }
        }

        [FieldName(nameof(descriptorList))]
        public IList<TerminalFontDescriptor> DescriptorList => this.descriptorList;

        public IEnumerable<Texture2D> Textures
        {
            get
            {
                foreach (var item in this.descriptorList)
                {
                    if (item is TerminalFontDescriptor descriptor)
                    {
                        foreach (var texture in descriptor.Textures)
                        {
                            yield return texture;
                        }
                    }
                }
            }
        }

        [FieldName(nameof(height))]
        public int Height
        {
            get => this.height;
            set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.InvokePropertyChangedEvent(nameof(Height));
                }
            }
        }

        [FieldName(nameof(width))]
        public int Width
        {
            get => this.width;
            set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.InvokePropertyChangedEvent(nameof(Width));
                }
            }
        }
        
        [FieldName(nameof(line))]
        public int Line
        {
            get => this.line;
            set
            {
                if (this.line != value)
                {
                    this.line = value;
                    this.InvokePropertyChangedEvent(nameof(Line));
                }
            }
        }

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

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
            TerminalValidationEvents.Register(this);
            TerminalValidationEvents.Validated += Object_Validated;
            this.OnEnabled(EventArgs.Empty);
        }

        protected virtual void OnDisable()
        {
            TerminalValidationEvents.Validated -= Object_Validated;
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

        private void Object_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalFontDescriptor descriptor && this.DescriptorList.Contains(descriptor))
            {
                this.UpdateSize();
                SetDirtyCallback?.Invoke(this);
            }
        }

        internal static Action<TerminalFont> SetDirtyCallback { get; set; }

        #region IPropertyChangedNotifyable

        void IPropertyChangedNotifyable.InvokePropertyChangedEvent(string propertyName)
        {
            this.InvokePropertyChangedEvent(propertyName);
        }

        #endregion
    }
}
