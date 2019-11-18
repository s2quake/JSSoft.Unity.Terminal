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

namespace JSSoft.UI
{
    public class TerminalForeground : UIBehaviour
    {
        [SerializeField]
        private TerminalGrid grid = null;

        public TerminalForeground()
        {

        }

        // public override Texture mainTexture => this.font?.atlasTexture;

        // public TerminalForeground Parent => this.GetComponentInParent<TerminalForeground>();

        // public TerminalFont Font => this.grid?.Font;

        protected override void Awake()
        {
            
        }

        protected override void Start()
        {
            if (this.grid != null)
            {
                this.grid.TextChanged += TerminalGrid_TextChanged;
                this.grid.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
            }
        }

        protected override void OnDestroy()
        {
            if (this.grid != null)
            {
                this.grid.TextChanged -= TerminalGrid_TextChanged;
                this.grid.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
            }
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            // this.SetVerticesDirty();
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            // this.SetVerticesDirty();
        }

        // private IEnumerable<TerminalFont> FallbackFontAssets
        // {
        //     get
        //     {
        //         if (this.font != null && this.font.fallbackFontAssetTable != null)
        //         {
        //             foreach (var item in this.font.fallbackFontAssetTable)
        //             {
        //                 yield return item;
        //             }
        //         }
        //     }
        // }

        // private IEnumerable<TerminalForeground> FallbackComponents
        // {
        //     get
        //     {
        //         for (var i = 0; i < this.rectTransform.childCount; i++)
        //         {
        //             var childTransform = this.rectTransform.GetChild(i);
        //             if (childTransform.GetComponent<TerminalForeground>() is TerminalForeground component)
        //             {
        //                 yield return component;
        //             }
        //         }
        //     }
        // }
    }
}
