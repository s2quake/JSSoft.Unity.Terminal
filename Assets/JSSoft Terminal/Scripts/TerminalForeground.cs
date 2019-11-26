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
using System.Collections;
using System.Threading.Tasks;

namespace JSSoft.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class TerminalForeground : UIBehaviour
    {
        [SerializeField]
        private TerminalGrid grid = null;

        public TerminalForeground()
        {

        }

        protected override void Awake()
        {
        }

        protected override void Start()
        {
            base.Start();
            TerminalGridEvents.TextChanged += TerminalGrid_TextChanged;
            TerminalGridEvents.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TerminalGridEvents.TextChanged -= TerminalGrid_TextChanged;
            TerminalGridEvents.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.Validated += TerminalGrid_Validated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalGridEvents.Validated -= TerminalGrid_Validated;
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid == this.grid)
            {
                // this.SetVerticesDirty();
            }
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid == this.grid)
            {
                // this.SetVerticesDirty();
            }
        }

        private async void TerminalGrid_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid == this.grid)
            {
                await Task.Delay(1);
                this.RefreshChilds();
            }
        }

        private IEnumerable<TerminalForegroundItem> Items
        {
            get
            {
                for (var i = 0; i < this.transform.childCount; i++)
                {
                    var childTransform = this.transform.GetChild(i);
                    if (childTransform.GetComponent<TerminalForegroundItem>() is TerminalForegroundItem component)
                    {
                        yield return component;
                    }
                }
            }
        }

        private void RefreshChilds()
        {
            var font = this.grid.Font;
            var itemByTexture = this.Items.ToDictionary(item => item.Texture);
            var textures = font != null ? font.Textures : new Texture2D[] { };
            for (var i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                if (itemByTexture.ContainsKey(texture) == true)
                {
                    itemByTexture.Remove(texture);
                }
                else
                {
                    var gameObject = new GameObject($"{nameof(TerminalForegroundItem)}{i}", typeof(TerminalForegroundItem));
                    var foregroundItem = gameObject.GetComponent<TerminalForegroundItem>();
                    var transform = foregroundItem.rectTransform;
                    foregroundItem.Texture = texture;
                    foregroundItem.Grid = this.grid;
                    transform.SetParent(this.transform);
                    transform.anchorMin = Vector3.zero;
                    transform.anchorMax = Vector3.one;
                    transform.offsetMin = Vector3.zero;
                    transform.offsetMax = Vector3.zero;
                }
            }

            var items = itemByTexture.Values.ToArray();
            foreach (var item in items)
            {
                GameObject.DestroyImmediate(item.gameObject);
            }
        }
    }
}
