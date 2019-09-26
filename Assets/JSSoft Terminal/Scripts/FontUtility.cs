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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public static class FontUtility
    {
        public static TMP_FontAsset GetFontAsset(TMP_FontAsset fontAsset, char character)
        {
            var fontAssets = GetFontAssets(fontAsset);
            foreach (var item in fontAssets)
            {
                if (item.characterLookupTable.ContainsKey(character) == true)
                {
                    return item;
                }
            }
            return null;
        }

        public static IEnumerable<TMP_FontAsset> GetFontAssets(TMP_FontAsset fontAsset)
        {
            if (fontAsset != null)
            {
                yield return fontAsset;
                if (fontAsset.fallbackFontAssetTable != null)
                {
                    foreach (var item in fontAsset.fallbackFontAssetTable)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
