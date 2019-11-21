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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JSSoft.UI.Fonts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.UI
{
    public class TerminalFont : ScriptableObject
    {
        [SerializeField]
        private BaseInfo baseInfo;
        [SerializeField]
        private CommonInfo commonInfo;
        [SerializeField]
        private CharInfo[] charInfos = new CharInfo[] { };
        [SerializeField]
        private Texture2D[] textures = new Texture2D[] { };
        private Dictionary<char, CharInfo> charInfoByID;

        public BaseInfo BaseInfo => this.baseInfo;

        public CommonInfo CommonInfo => this.commonInfo;

        public Texture2D[] Textures => this.textures ?? new Texture2D[] { };

        public IReadOnlyDictionary<char, CharInfo> CharInfos
        {
            get
            {
                if (this.charInfoByID == null)
                {
                    this.charInfoByID = this.charInfos.ToDictionary(item => (char)item.ID);
                }
                return this.charInfoByID;
            }
        }

#if UNITY_EDITOR
        public static TerminalFont Create(TextAsset fntAsset)
        {
            using (var sb = new StringReader(fntAsset.text))
            using (var reader = XmlReader.Create(sb))
            {
                var assetPath = AssetDatabase.GetAssetPath(fntAsset);
                var assetDirectory = Path.GetDirectoryName(assetPath);
                var serializer = new XmlSerializer(typeof(Fonts.Serializations.FontSerializationInfo));
                var obj = (Fonts.Serializations.FontSerializationInfo)serializer.Deserialize(reader);
                var charInfos = obj.CharInfo.Items;
                var pages = obj.Pages;
                var font = new TerminalFont();
                font.baseInfo = (BaseInfo)obj.Info;
                font.commonInfo = (CommonInfo)obj.Common;
                font.textures = new Texture2D[pages.Length];
                for (var i = 0; i < pages.Length; i++)
                {
                    var item = pages[i];
                    var texturePath = Path.Combine(assetDirectory, item.File);
                    font.textures[i] = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
                }
                font.charInfos = new CharInfo[charInfos.Length];
                for (var i = 0; i < charInfos.Length; i++)
                {
                    font.charInfos[i] = (CharInfo)charInfos[i];
                }
                return font;
            }
        }
#endif
    }
}
