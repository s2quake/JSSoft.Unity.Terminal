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
using System.Threading.Tasks;
using System.ComponentModel;

namespace JSSoft.Terminal
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    class TerminalForeground : UIBehaviour
    {
        [SerializeField]
        private TerminalGrid grid = null;

        public TerminalForeground()
        {

        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.Validated += Grid_Validated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.Validated -= Grid_Validated;
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                switch (e.PropertyName)
                {
                    case nameof(ITerminalGrid.Font):
                    case nameof(ITerminalGrid.Style):
                        {
                            this.RefreshChilds();
                        }
                        break;
                }
            }
        }

        private async void Grid_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
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
            if (this.IsDestroyed() == true)
                return;
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
                    foregroundItem.material = new Material(Shader.Find("UI/Default"));
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
