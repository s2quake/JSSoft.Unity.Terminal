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

namespace JSSoft.UI
{
    public static class FontUtility
    {
        private static readonly char defaultCharacter = 'a';
        private static readonly int defaultItemWidth = 14;

        public static TMP_FontAsset GetFontAsset(TMP_FontAsset fontAsset, char character)
        {
            if (fontAsset == null)
                throw new ArgumentNullException(nameof(fontAsset));
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

        public static TMP_Character GetCharacter(TMP_FontAsset fontAsset, char character)
        {
            if (fontAsset == null)
                throw new ArgumentNullException(nameof(fontAsset));
            if (FontUtility.GetFontAsset(fontAsset, character) is TMP_FontAsset fontAsset1)
                return fontAsset1.characterLookupTable[character];
            return null;
        }

        public static int GetCharacterVolume(TMP_FontAsset fontAsset, char character)
        {
            if (fontAsset == null)
                throw new ArgumentNullException(nameof(fontAsset));
            if (GetCharacter(fontAsset, character) is TMP_Character characterInfo)
            {
                var defaultWidth = GetItemWidth(fontAsset);
                var horizontalAdvance = characterInfo.glyph.metrics.horizontalAdvance;
                var volume = (int)Math.Ceiling(horizontalAdvance / defaultWidth);
                return Math.Max(volume, 1);
            }
            return 1;
        }

        public static (Vector2, Vector2) GetUV(TMP_FontAsset fontAsset, char character)
        {
            if (fontAsset == null)
                throw new ArgumentNullException(nameof(fontAsset));
            if (fontAsset.characterLookupTable.ContainsKey(character) == false)
                throw new ArgumentException($"'{character}' does not exits.", nameof(character));
            var characterInfo = fontAsset.characterLookupTable[character];
            var texture = fontAsset.atlasTexture;
            var glyph = characterInfo.glyph;
            var glyphRect = glyph.glyphRect;
            var textWidth = (float)texture.width;
            var textHeight = (float)texture.height;
            var uv0 = new Vector2(glyphRect.x / textWidth, glyphRect.y / textHeight);
            var uv1 = new Vector2((glyphRect.x + glyphRect.width) / textWidth, (glyphRect.y + glyphRect.height) / textHeight);
            return (uv0, uv1);
        }

        public static Rect GetForegroundRect(TMP_FontAsset fontAsset, char character)
        {
            return GetForegroundRect(fontAsset, character, 0, 0);
        }

        public static Rect GetForegroundRect(TMP_FontAsset fontAsset, char character, int x, int y)
        {
            if (fontAsset == null)
                throw new ArgumentNullException(nameof(fontAsset));
            if (fontAsset.characterLookupTable.ContainsKey(character) == false)
                throw new ArgumentException($"'{character}' does not exits.", nameof(character));
            var characterInfo = fontAsset.characterLookupTable[character];
            var glyph = characterInfo.glyph;
            var topBorder = (float)Math.Ceiling(fontAsset.faceInfo.lineHeight) - fontAsset.faceInfo.lineHeight;
            var glyphRect = glyph.glyphRect;
            var fx = x + glyph.metrics.horizontalBearingX;
            var fy = y + fontAsset.faceInfo.ascentLine - glyph.metrics.horizontalBearingY - topBorder;
            return new Rect(fx, fy, glyph.metrics.width, glyph.metrics.height);
        }

        public static int GetItemWidth(TMP_FontAsset originAsset)
        {
            if (originAsset == null)
                throw new ArgumentNullException(nameof(originAsset));
            if (GetCharacter(originAsset, defaultCharacter) is TMP_Character characterInfo)
                return (int)characterInfo.glyph.metrics.horizontalAdvance;
            return defaultItemWidth;
        }

        public static int GetItemWidth(TMP_FontAsset originAsset, char character)
        {
            var itemWidth = GetItemWidth(originAsset);
            if (GetCharacter(originAsset, character) is TMP_Character characterInfo)
            {
                var characterWidth = (int)characterInfo.glyph.metrics.horizontalAdvance;
                var n = Math.Ceiling(characterWidth / (float)itemWidth);
                return (int)(itemWidth * n);
            }
            return itemWidth;
        }

        public static int GetItemHeight(TMP_FontAsset originAsset)
        {
            if (originAsset == null)
                throw new ArgumentNullException(nameof(originAsset));
            return (int)Math.Ceiling(originAsset.faceInfo.lineHeight);
        }
    }
}
