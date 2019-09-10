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
        private readonly Dictionary<int, int> swapIndexes = new Dictionary<int, int>();

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
            //Debug.Log($"SaveGlyphVertexInfo: {m_textInfo.characterInfo.Length}");
            // if (m_textInfo.characterCount - 1 == m_characterCount)
            // {
            //     Debug.Log("end");
            // }
            // Debug.Log($"{m_textInfo.meshInfo[0].vertices.Length}");
        }

        protected override void FillCharacterVertexBuffers(int i, int index_X4)
        {
            // Debug.Log($"vertext: {i}, {index_X4}");
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            if (this.swapIndexes.ContainsKey(i) == true)
            {

            }

            base.FillCharacterVertexBuffers(i, index_X4);

            if (this.terminal.GetForegroundColor(i) is Color32 foregroundColor)
            {
                // m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = foregroundColor;
                // m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = foregroundColor;
                // m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = foregroundColor;
                // m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = foregroundColor;
                // m_textInfo.characterInfo[i].vertex_BL.color = foregroundColor;
                // m_textInfo.characterInfo[i].vertex_TL.color = foregroundColor;
                // m_textInfo.characterInfo[i].vertex_TR.color = foregroundColor;
                // m_textInfo.characterInfo[i].vertex_BR.color = foregroundColor;
            }
            // if (this.terminal.GetBackgroundColor(i) is Color32 backgroundColor)
            // {
            //     m_textInfo.characterInfo[i].style |= TMPro.FontStyles.Highlight;
            //     m_textInfo.characterInfo[i].highlightColor = backgroundColor;
            // }

        }

        protected override void DrawTextHighlight(Vector3 start, Vector3 end, ref int index, Color32 highlightColor)
        {
            var index1 = index / 4;
            var index2 = this.swapIndexes.Count;
            // Debug.Log($"{m_textInfo.meshInfo[0].vertices.Length / 4} to {index1}");
            base.DrawTextHighlight(start, end, ref index, highlightColor);
            // this.SwapTriangle(ref m_textInfo.meshInfo[0], index1, index2);
            // Debug.Log($"highlight: {index1} to {index2}");
            this.swapIndexes.Add(index1, index2);
        }

        private void SwapTriangle(ref TMP_MeshInfo meshInfo, int index1, int index2)
        {
            var buffer = new int[6];
            var begin = index1 * 6;
            var end = index2 * 6;
            var triangles = m_mesh.triangles;
            Array.Copy(triangles, begin, buffer, 0, buffer.Length);
            Array.Copy(triangles, end, triangles, begin, buffer.Length);
            Array.Copy(buffer, 0, triangles, end, buffer.Length);
            m_mesh.triangles = triangles;
            meshInfo.triangles = triangles;
        }

        protected override void GenerateTextMesh()
        {
            this.swapIndexes.Clear();

            {
                // var triangles = m_mesh.triangles;
                // var size = triangles.Length / 6;
                // for (var i = 0; i < size; i++)
                // {
                //     var index_X6 = i * 6;
                //     var index_X4 = i * 4;
                //     triangles[0 + index_X6] = 0 + index_X4;
                //     triangles[1 + index_X6] = 1 + index_X4;
                //     triangles[2 + index_X6] = 2 + index_X4;
                //     triangles[3 + index_X6] = 2 + index_X4;
                //     triangles[4 + index_X6] = 3 + index_X4;
                //     triangles[5 + index_X6] = 0 + index_X4;
                // }
                // m_mesh.triangles = triangles;
                // m_textInfo.meshInfo[0].triangles = triangles;
            }
            // {
            //     var triangles = m_textInfo.meshInfo[0].triangles;
            //     var size = triangles.Length / 6;
            //     for (var i = 0; i < size; i++)
            //     {
            //         var index_X6 = i * 6;
            //         var index_X4 = i * 4;
            //         triangles[0 + index_X6] = 0 + index_X4;
            //         triangles[1 + index_X6] = 1 + index_X4;
            //         triangles[2 + index_X6] = 2 + index_X4;
            //         triangles[3 + index_X6] = 2 + index_X4;
            //         triangles[4 + index_X6] = 3 + index_X4;
            //         triangles[5 + index_X6] = 0 + index_X4;
            //     }
            //     m_textInfo.meshInfo[0].triangles = triangles;
            // }
            base.GenerateTextMesh();
            // {
            //     var triangles = m_textInfo.meshInfo[0].triangles;
            //     var size = triangles.Length / 6;
            //     for (var i = 0; i < size; i++)
            //     {
            //         var index_X6 = i * 6;
            //         Debug.Log($"face: {i}, {triangles[index_X6]}");
            //     }
            // }
        }
    }
}
