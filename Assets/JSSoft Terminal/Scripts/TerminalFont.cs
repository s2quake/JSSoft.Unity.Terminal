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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JSSoft.UI.Fonts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.UI
{
    public class TerminalFont : ScriptableObject
    {
        [SerializeField]
        private TerminalFontDescriptor[] fonts;
        [SerializeField]
        [Range(0, 1000)]
        private int width;
        [SerializeField]
        [Range(0, 1000)]
        private int height;

        public bool Contains(char character)
        {
            foreach (var item in this.Fonts)
            {
                if (item.Contains(character) == true)
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
                    if (item.Contains(character) == true)
                        return item[character];
                }
                return new CharInfo()
                {
                    XAdvance = this.width,
                    ID = (int)character,
                };
            }
        }

        public TerminalFontDescriptor[] Fonts
        {
            get => this.fonts ?? new TerminalFontDescriptor[] { };
            set => this.fonts = value;
        }

        public Texture2D[] Textures
        {
            get
            {
                var textureList = new List<Texture2D>();
                foreach (var item in this.Fonts)
                {
                    textureList.AddRange(item.Textures);
                }
                return textureList.ToArray();
            }
        }

        public int Height => this.height;

        public int Width => this.width;

        public void OnValidate()
        {
            Debug.Log(Time.time);
        }
    }
}
