using System;
using UnityEngine;

namespace JSSoft.UI
{
    public class TerminalText : TMPro.TextMeshProUGUI
    {
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

        protected override void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
        {
            base.SaveGlyphVertexInfo(padding, style_padding, vertexColor);
            if (this.terminal != null)
            {
                if (this.terminal.GetForegroundColor(m_characterCount) is Color32 foregroundColor)
                {
                    m_textInfo.characterInfo[m_characterCount].vertex_BL.color = foregroundColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TL.color = foregroundColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TR.color = foregroundColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_BR.color = foregroundColor;
                }
            }
        }
    }
}
