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
        private static readonly char defaultCharacter = 'a';
        private static readonly int defaultItemWidth = 14;
        private static readonly int defaultItemHeight = 21;

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
            if (GetCharacter(fontAsset, defaultCharacter) is TMP_Character characterInfo)
            {
                var defaultWidth = GetItemWidth(fontAsset);
                var horizontalAdvance = characterInfo.glyph.metrics.horizontalAdvance;
                return (int)Math.Ceiling(horizontalAdvance / defaultWidth);
            }
            return 1;
        }

        public static int GetItemWidth(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null)
                throw new ArgumentNullException(nameof(fontAsset));
            if (GetCharacter(fontAsset, defaultCharacter) is TMP_Character characterInfo)
                return (int)characterInfo.glyph.metrics.horizontalAdvance;
            return defaultItemWidth;
        }

        public static TerminalRow[] GenerateTerminalRows(TMP_FontAsset fontAsset, string text, int columnCount)
        {
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var rowList = new List<TerminalRow>(lines.Length);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                while (line != string.Empty)
                {
                    var row = new TerminalRow(rowList.Count, columnCount);
                    line = FillString(fontAsset, row, line);
                    rowList.Add(row);
                }
            }
            return rowList.ToArray();
        }

        public static string FillString(TMP_FontAsset fontAsset, TerminalRow row, string text)
        {
            var columnIndex = 0;
            var cellCount = row.Cells.Length;
            var i = 0;
            while (columnIndex < cellCount && i < text.Length)
            {
                var character = text[i];
                var cell = row.Cells[columnIndex];
                var volume = FontUtility.GetCharacterVolume(fontAsset, character);
                if (columnIndex + volume > cellCount)
                    break;
                cell.Refresh(fontAsset, character);
                columnIndex += volume;
                i++;
            }
            return text.Substring(i);
        }
    }
}
