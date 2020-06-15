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
using JSSoft.Terminal.Fonts;
using UnityEngine;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSSoft.Terminal
{
    [CreateAssetMenu(menuName = "Terminal/Font")]
    public class TerminalFont : ScriptableObject, INotifyValidated
    {
        // private static readonly IReadOnlyList<TerminalFontDescriptor> emptyList = new List<TerminalFontDescriptor>();
        [SerializeField]
        private List<TerminalFontDescriptor> descriptorList = new List<TerminalFontDescriptor>();
        [SerializeField]
        private int width = FontUtility.DefaultItemWidth;
        [SerializeField]
        private int height = FontUtility.DefaultItemHeight;

        // private ObservableCollection<TerminalFontDescriptor> descriptors = new ObservableCollection<TerminalFontDescriptor>();
        // private Texture2D[] textures = new Texture2D[] { };

        public TerminalFont()
        {
            // this.descriptors.CollectionChanged += Descriptors_CollectionChanged;
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

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnValidate()
        {
            // this.UpdateDescriptors();
            // this.UpdateTextures();
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
            // this.UpdateDescriptors();
            // this.UpdateTextures();
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

        private void UpdateSize()
        {
            var mainFont = this.DescriptorList.FirstOrDefault();
            this.width = mainFont != null ? mainFont.Width : FontUtility.DefaultItemWidth;
            this.height = mainFont != null ? mainFont.Height : FontUtility.DefaultItemHeight;
        }

        private void Object_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalFontDescriptor descriptor && this.DescriptorList.Contains(descriptor))
            {
                this.UpdateSize();
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        // private void Descriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        // {
        //     this.descriptorList.Clear();
        //     this.descriptorList.AddRange(this.descriptors);
        // }

        // private void UpdateDescriptors()
        // {
        //     this.descriptors.CollectionChanged -= Descriptors_CollectionChanged;
        //     this.descriptors.Clear();
        //     foreach (var item in this.descriptorList)
        //     {
        //         this.descriptors.Add(item);
        //     }
        //     this.descriptors.CollectionChanged += Descriptors_CollectionChanged;
        // }

        // private void UpdateTextures()
        // {
        //     var textureList = new List<Texture2D>();
        //     foreach (var item in this.descriptorList)
        //     {
        //         if (item is TerminalFontDescriptor descriptor)
        //         {
        //             textureList.AddRange(descriptor.Textures);
        //         }
        //     }
        //     this.textures = textureList.ToArray();
        // }
    }
}
