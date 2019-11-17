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
        private CharInfo[] charInfos;
        [SerializeField]
        private Texture2D[] textures;
        private readonly Dictionary<char, CharInfo> charInfoByID = new Dictionary<char, CharInfo>();
        private readonly Dictionary<int, Texture2D> textureByID = new Dictionary<int, Texture2D>();

        public BaseInfo BaseInfo => this.baseInfo;

        public CommonInfo CommonInfo => this.commonInfo;

        public IReadOnlyDictionary<char, CharInfo> CharInfos => this.charInfoByID;

        public IReadOnlyDictionary<int, Texture2D> Textures => this.textureByID;

        protected virtual void Awake()
        {
            this.textureByID.Clear();
            if (this.textures != null)
            {
                for (var i = 0; i < this.textures.Length; i++)
                {
                    this.textureByID.Add(i, this.textures[i]);
                }
            }
            this.charInfoByID.Clear();
            if (this.charInfos != null)
            {
                foreach (var item in this.charInfos)
                {
                    this.charInfoByID.Add((char)item.ID, item);
                }
                Debug.Log(this.charInfoByID.Count);
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
                var font = new TerminalFont();
                font.baseInfo = (BaseInfo)obj.Info;
                font.commonInfo = (CommonInfo)obj.Common;
                foreach (var item in obj.Pages)
                {
                    var texturePath = Path.Combine(assetDirectory, item.File);
                    var texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
                    font.textureByID.Add(item.ID, texture);
                }
                font.textures = font.textureByID.Values.ToArray();
                foreach (var item in obj.CharInfo.Items)
                {
                    var charInfo = (CharInfo)item;
                    font.charInfoByID.Add((char)charInfo.ID, charInfo);
                }
                font.charInfos = font.charInfoByID.Values.ToArray();
                return font;
            }
        }
#endif
    }
}
