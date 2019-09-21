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
    public class TerminalText : TMPro.TextMeshProUGUI
    {
        [SerializeField]
        public Terminal terminal;
        private int oldVertexCount;
        private int newVertexCount;
        private int cursorIndex = -1;

        /// <summary>
        /// 공백인 경우 TMP_Text 에서 isVisible 이 false 이기 때문에 하위 클래스에서 어떠한 처리도 할 수가 없음.
        /// 일단 잘 안쓰는 문자로 표시가 가능하도록 하고 색상을 투명화로 설정해서 안보이게 해서 공백처럼 보이게 하려고 하기 위함.
        /// </summary>
        protected override int SetArraySizes(UnicodeChar[] chars)
        {
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i].unicode == 32)
                {
                    chars[i].unicode = 255;
                }
            }
            var length = this.GetLength(chars);
            chars[length - 2].unicode = 255;
            // if (length == m_TextParsingBuffer.Length)
            // {
            //     Array.Resize(ref m_TextParsingBuffer, length + 1);
            // }
            // m_TextParsingBuffer[length].stringIndex = m_TextParsingBuffer[length-1].stringIndex + 1;
            // m_TextParsingBuffer[length].stringIndex = m_TextParsingBuffer[length-1].length;
            // m_TextParsingBuffer[length].unicode = m_TextParsingBuffer[length-1].unicode;
            // m_TextParsingBuffer[length-1].stringIndex = m_TextParsingBuffer[length-2].stringIndex + 1;
            // m_TextParsingBuffer[length-1].stringIndex = m_TextParsingBuffer[length-2].length;
            // m_TextParsingBuffer[length-1].unicode = m_TextParsingBuffer[length-2].unicode;
            // m_TextParsingBuffer[length - 2].unicode = 255;
            // // m_TextParsingBuffer[length - 2].stringIndex = length;
            // m_TextParsingBuffer[length - 2].length = 1;
            this.cursorIndex = length - 2;
            // Debug.Log($"cursorIndex: {this.cursorIndex}");

            var size = base.SetArraySizes(chars);
            return size;
        }

        private int GetLength(UnicodeChar[] chars)
        {
            for (var i = 0; i < chars.Length; i++)
            {
                // if (chars[i].unicode == 8203)
                // {
                //     Debug.Log($"8203: {i}");
                //     return i;
                // }
                if (chars[i].unicode == 0)
                    return i + 1;
            }
            return 0;
        }

        protected override void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
        {
            if (m_textInfo.characterInfo[m_characterCount].textElement.glyph.metrics.horizontalAdvance > this.terminal.caretWidth)
            {
                var metrics = m_textInfo.characterInfo[m_characterCount].textElement.glyph.metrics;
                metrics.horizontalAdvance = this.terminal.caretWidth * 2;
                m_textInfo.characterInfo[m_characterCount].textElement.glyph.metrics = metrics;
            }
            base.SaveGlyphVertexInfo(padding, style_padding, vertexColor);
            if (this.terminal.GetForegroundColor(m_characterCount) is Color32 foregroundColor)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color = foregroundColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color = foregroundColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color = foregroundColor;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color = foregroundColor;
            }
            else if (m_characterCount == this.cursorIndex)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color = TerminalColors.Transparent;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color = TerminalColors.Transparent;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color = TerminalColors.Transparent;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color = TerminalColors.Transparent;
            }

            if (m_characterCount == this.cursorIndex)
            {
                m_textInfo.characterInfo[m_characterCount].style |= TMPro.FontStyles.Highlight;
                m_textInfo.characterInfo[m_characterCount].highlightColor = this.terminal.caretColor;
            }
            else if (this.IsSelected(m_characterCount) == true)
            {
                m_textInfo.characterInfo[m_characterCount].style |= TMPro.FontStyles.Highlight;
                m_textInfo.characterInfo[m_characterCount].highlightColor = this.terminal.selectionColor;
            }
            else if (this.terminal.GetBackgroundColor(m_characterCount) is Color32 backgroundColor)
            {
                m_textInfo.characterInfo[m_characterCount].style |= TMPro.FontStyles.Highlight;
                m_textInfo.characterInfo[m_characterCount].highlightColor = backgroundColor;
            }
        }
        private int i2;
        protected override void FillCharacterVertexBuffers(int i, int index_X4)
        {
            var materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            if (materialIndex == 0)
            {
                m_textInfo.meshInfo[materialIndex].vertexCount = this.oldVertexCount;
                base.FillCharacterVertexBuffers(i, index_X4);
                this.oldVertexCount = m_textInfo.meshInfo[materialIndex].vertexCount;
                if (this.oldVertexCount < this.newVertexCount)
                {
                    m_textInfo.meshInfo[materialIndex].vertexCount = this.newVertexCount;
                }
            }
            else
            {
                base.FillCharacterVertexBuffers(i, index_X4);
            }
            this.i2 = i;
        }

        protected override void DrawTextHighlight(Vector3 start, Vector3 end, ref int index, Color32 highlightColor)
        {
            var i = index;
            base.DrawTextHighlight(start, end, ref index, highlightColor);
            if (this.i2 == this.cursorIndex)
            {
                ref var meshInfo = ref m_textInfo.meshInfo[0];
                var i3 = m_materialReferences[0].referenceCount * 4;
                var currentCharacter = m_textInfo.characterInfo[this.terminal.CursorPositionInternal];
                var startPosition = new Vector2(currentCharacter.origin, currentCharacter.descender);
                var height = currentCharacter.ascender - currentCharacter.descender;
                var top = startPosition.y + height;
                var bottom = top - height;
                var width = this.terminal.caretWidth;
                // var scale = m_TextComponent.canvas.scaleFactor;
                meshInfo.vertices[i + 0] = meshInfo.vertices[i3 + 0];
                meshInfo.vertices[i + 1] = meshInfo.vertices[i3 + 1];
                meshInfo.vertices[i + 2] = meshInfo.vertices[i3 + 2];
                meshInfo.vertices[i + 3] = meshInfo.vertices[i3 + 3];
                meshInfo.colors32[i + 0] = meshInfo.colors32[i3 + 0];
                meshInfo.colors32[i + 1] = meshInfo.colors32[i3 + 1];
                meshInfo.colors32[i + 2] = meshInfo.colors32[i3 + 2];
                meshInfo.colors32[i + 3] = meshInfo.colors32[i3 + 3];
                meshInfo.vertices[i3 + 0] = new Vector3(startPosition.x, bottom, 0.0f);
                meshInfo.vertices[i3 + 1] = new Vector3(startPosition.x, top, 0.0f);
                meshInfo.vertices[i3 + 2] = new Vector3(startPosition.x + width, top, 0.0f);
                meshInfo.vertices[i3 + 3] = new Vector3(startPosition.x + width, bottom, 0.0f);
                meshInfo.colors32[i3 + 0] = highlightColor;
                meshInfo.colors32[i3 + 1] = highlightColor;
                meshInfo.colors32[i3 + 2] = highlightColor;
                meshInfo.colors32[i3 + 3] = highlightColor;
            }
            m_textInfo.meshInfo[0].vertexCount = this.newVertexCount = index;
        }

        protected override void GenerateTextMesh()
        {
            this.newVertexCount = 0;
            this.oldVertexCount = 0;
            base.GenerateTextMesh();
        }

        private bool IsSelected(int index)
        {
            var s1 = this.terminal.selectionAnchorPosition;
            var s2 = this.terminal.selectionFocusPosition;
            if (s2 < s1 && index >= s2 && index < s1)
            {
                return true;
            }
            else if (index >= s1 && index < s2)
            {
                return true;
            }
            return false;
        }

        public void Refresh()
        {
            this.GenerateTextMesh();
        }
    }
}
