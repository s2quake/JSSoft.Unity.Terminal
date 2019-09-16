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

        /// <summary>
        /// 공백인 경우 TMP_Text 에서 isVisible 이 false 이기 때문에 하위 클래스에서 어떠한 처리도 할 수가 없음.
        /// 일단 잘 안쓰는 문자로 표시가 가능하도록 하고 색상을 투명화로 설정해서 안보이게 해서 공백처럼 보이게 하려고 하기 위함.
        /// </summary>
        protected override int SetArraySizes(UnicodeChar[] chars)
        {
            for (var i = 0; i < m_TextParsingBuffer.Length; i++)
            {
                if (m_TextParsingBuffer[i].unicode == 32)
                {
                    m_TextParsingBuffer[i].unicode = 255;
                }
            }
            return base.SetArraySizes(chars);
        }

        protected override void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
        {
            base.SaveGlyphVertexInfo(padding, style_padding, vertexColor);
            if (this.terminal.GetForegroundColor(m_characterCount) is Color32 foregroundColor)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color = foregroundColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color = foregroundColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color = foregroundColor;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color = foregroundColor;
            }
            if (this.terminal.GetBackgroundColor(m_characterCount) is Color32 backgroundColor)
            {
                m_textInfo.characterInfo[m_characterCount].style |= TMPro.FontStyles.Highlight;
                m_textInfo.characterInfo[m_characterCount].highlightColor = backgroundColor;
            }
        }

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
        }

        protected override void DrawTextHighlight(Vector3 start, Vector3 end, ref int index, Color32 highlightColor)
        {
            base.DrawTextHighlight(start, end, ref index, highlightColor);
            m_textInfo.meshInfo[0].vertexCount = this.newVertexCount = index;
        }

        protected override void GenerateTextMesh()
        {
            this.newVertexCount = 0;
            this.oldVertexCount = 0;
            base.GenerateTextMesh();
        }
    }
}
