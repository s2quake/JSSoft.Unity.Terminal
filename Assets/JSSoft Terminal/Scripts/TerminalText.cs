using System;
using UnityEngine;

namespace JSSoft.UI
{
    public class TerminalText : TMPro.TextMeshProUGUI
    {
        [SerializeField]
        public Terminal terminal;

        public override void UpdateVertexData()
        {
            // for (var i = 0; i < m_textInfo.meshInfo.Length; i++)
            // {
            //     var color32 = m_textInfo.meshInfo[i].colors32;
            //     for (var j = 0; j < color32.Length; j++)
            //     {
            //         color32[j].r = 255;
            //         color32[j].g = 0;
            //         color32[j].b = 0;
            //     }
            // }
            base.UpdateVertexData();
        }

        protected override int SetArraySizes(UnicodeChar[] chars)
        {
            for (var i = 0; i < m_TextParsingBuffer.Length; i++)
            {
                // 공백일때 TMP_Text 에서 처리를 해주지 않아서 배경색을 칠할 수가 없음.
                if (m_TextParsingBuffer[i].unicode == 32)
                {
                    m_TextParsingBuffer[i].unicode = 255;
                }
            }
            var result = base.SetArraySizes(chars);
            return result;
        }

        protected override void SaveSpriteVertexInfo(Color32 vertexColor)
        {
            Debug.Log($"SaveSpriteVertexInfo: {m_characterCount}");
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

        protected override void GenerateTextMesh()
        {
            base.GenerateTextMesh();

            // 버텍스 반대로 정렬 기능이 동작하지 않아서 수동으로 처리
            m_mesh.MarkDynamic();
            var vertices = new Vector3[m_mesh.vertices.Length];
            var uv = new Vector2[m_mesh.uv.Length];
            var uv2 = new Vector2[m_mesh.uv2.Length];
            var colors = new Color32[m_mesh.colors32.Length];
            for (var i = 0; i < m_mesh.colors32.Length; i++)
            {
                vertices[i] = m_mesh.vertices[m_mesh.vertices.Length - (i + 1)];
                uv[i] = m_mesh.uv[m_mesh.uv.Length - (i + 1)];
                uv2[i] = m_mesh.uv2[m_mesh.uv2.Length - (i + 1)];
                colors[i] = m_mesh.colors[m_mesh.colors32.Length - (i + 1)];
            }
            m_mesh.vertices = vertices;
            m_mesh.uv = uv;
            m_mesh.uv2 = uv2;
            m_mesh.colors32 = colors;

            m_mesh.RecalculateBounds();
            this.gameObject.GetComponent<CanvasRenderer>().SetMesh(m_mesh);
        }
    }
}
