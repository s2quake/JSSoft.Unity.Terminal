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
using JSSoft.UI.Fonts;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSSoft.UI
{
    [CreateAssetMenu(menuName = "Terminal/Create Font")]
    public class TerminalFont : ScriptableObject
    {
        private static readonly IReadOnlyList<TerminalFontDescriptor> emptyList = new List<TerminalFontDescriptor>();
        [SerializeField]
        private List<TerminalFontDescriptor> fontList = new List<TerminalFontDescriptor>();
        [SerializeField]
        private int width = FontUtility.DefaultItemWidth;
        [SerializeField]
        private int height = FontUtility.DefaultItemHeight;

        public bool Contains(char character)
        {
            foreach (var item in this.Fonts)
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
                foreach (var item in this.Fonts)
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

        public IList<TerminalFontDescriptor> FontList => this.fontList;

        public Texture2D[] Textures
        {
            get
            {
                var textureList = new List<Texture2D>();
                foreach (var item in this.fontList)
                {
                    if (item is TerminalFontDescriptor descriptor)
                    {
                        textureList.AddRange(descriptor.Textures);
                    }
                }
                return textureList.ToArray();
            }
        }

        public int Height => this.height;

        public int Width => this.width;

        public event EventHandler Validated;

        protected virtual void OnValidate()
        {
            this.OnValidated(EventArgs.Empty);
        }

        protected virtual void Awake()
        {
            TerminalFontDescriptorEvents.Validated += TerminalFontDescriptor_Validated;
        }

        protected virtual void OnDestroy()
        {
            TerminalFontDescriptorEvents.Validated -= TerminalFontDescriptor_Validated;
        }

        protected virtual void OnEnable()
        {
            TerminalFontEvents.Register(this);
        }

        protected virtual void OnDisable()
        {
            TerminalFontEvents.Unregister(this);
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, e);
        }

        private void UpdateSize()
        {
            var mainFont = this.Fonts.FirstOrDefault();
            this.width = mainFont != null ? mainFont.Width : FontUtility.DefaultItemWidth;
            this.height = mainFont != null ? mainFont.Height : FontUtility.DefaultItemHeight;
        }

        private void TerminalFontDescriptor_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalFontDescriptor descriptor && this.Fonts.Contains(descriptor))
            {
                this.UpdateSize();
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        private IReadOnlyList<TerminalFontDescriptor> Fonts => this.fontList ?? emptyList;
    }
}
