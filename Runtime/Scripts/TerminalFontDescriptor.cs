﻿////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JSSoft.Unity.Terminal.Fonts;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public class TerminalFontDescriptor : ScriptableObject, INotifyValidated, IPropertyChangedNotifyable
    {
        [SerializeField]
        internal BaseInfo baseInfo;
        [SerializeField]
        internal CommonInfo commonInfo;
        [SerializeField]
        internal CharInfo[] charInfos = new CharInfo[] { };
        [SerializeField]
        internal Texture2D[] textures = new Texture2D[] { };
        [SerializeField]
        internal int width;

        private Dictionary<char, CharInfo> charInfoByID = new Dictionary<char, CharInfo>();

        public bool Contains(char character)
        {
            return this.Characters.ContainsKey(character);
        }

        public static TerminalFontDescriptor Create(TextAsset textAsset, TerminalFontResolver resolver)
        {
            var descriptor = new TerminalFontDescriptor();
            descriptor.Refresh(textAsset, resolver);
            return descriptor;
        }

        public void Refresh(TextAsset textAsset, TerminalFontResolver resolver)
        {
            if (textAsset == null)
                throw new ArgumentException(nameof(textAsset));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            using (var sb = new StringReader(textAsset.text))
            using (var reader = XmlReader.Create(sb))
            {
                var serializer = new XmlSerializer(typeof(Fonts.Serializations.FontSerializationInfo));
                var obj = (Fonts.Serializations.FontSerializationInfo)serializer.Deserialize(reader);
                var charInfos = obj.CharInfo.Items;
                var pages = obj.Pages;
                this.baseInfo = (BaseInfo)obj.Info;
                this.commonInfo = (CommonInfo)obj.Common;
                this.textures = new Texture2D[pages.Length];
                for (var i = 0; i < pages.Length; i++)
                {
                    var item = pages[i];
                    this.textures[i] = resolver.GetTexture(textAsset, item.File);
                }
                this.charInfos = new CharInfo[charInfos.Length];
                for (var i = 0; i < charInfos.Length; i++)
                {
                    var item = charInfos[i];
                    var charInfo = (CharInfo)item;
                    charInfo.Texture = this.textures[item.Page];
                    this.charInfos[i] = charInfo;
                }
                this.UpdateWidth();
                this.UpdateProperty();
            }
        }

        public CharInfo this[char character] => this.Characters[character];

        [FieldName(nameof(baseInfo))]
        public BaseInfo BaseInfo
        {
            get => this.baseInfo;
            set
            {
                this.baseInfo = value;
                this.InvokePropertyChangedEvent(nameof(BaseInfo));
            }
        }

        [FieldName(nameof(commonInfo))]
        public CommonInfo CommonInfo
        {
            get => this.commonInfo;
            set
            {
                this.commonInfo = value;
                this.InvokePropertyChangedEvent(nameof(CommonInfo));
            }
        }

        [FieldName(nameof(charInfos))]
        public CharInfo[] CharInfos
        {
            get => this.charInfos ?? new CharInfo[] { };
            set
            {
                this.charInfos = value;
                this.InvokePropertyChangedEvent(nameof(CharInfos));
            }
        }

        [FieldName(nameof(textures))]
        public Texture2D[] Textures
        {
            get => this.textures ?? new Texture2D[] { };
            set
            {
                this.textures = value;
                this.InvokePropertyChangedEvent(nameof(Textures));
            }
        }

        public int Height => this.commonInfo.LineHeight;

        public int Width => this.width;

        public IReadOnlyDictionary<char, CharInfo> Characters
        {
            get
            {
                if (this.charInfoByID.Count != this.charInfos.Length)
                {
                    this.charInfoByID = this.charInfos.ToDictionary(item => (char)item.ID);
                }
                return this.charInfoByID;
            }
        }

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnValidate()
        {
            this.UpdateProperty();
            this.OnValidated(EventArgs.Empty);
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
            this.UpdateProperty();
            TerminalValidationEvents.Register(this);
            this.OnEnabled(EventArgs.Empty);
        }

        protected virtual void OnDisable()
        {
            this.OnDisabled(EventArgs.Empty);
            TerminalValidationEvents.Unregister(this);
        }

        protected virtual void OnEnabled(EventArgs e)
        {
            this.Enabled?.Invoke(this, e);
        }

        protected virtual void OnDisabled(EventArgs e)
        {
            this.Disabled?.Invoke(this, e);
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 프린트 가능한 문자들의 폭만을 계산
        /// https://theasciicode.com.ar
        /// </summary>
        internal void UpdateWidth()
        {
            var width = 0;
            for (var i = 0; i < this.charInfos.Length; i++)
            {
                var item = this.charInfos[i];
                if (item.ID >= 32 && item.ID < 126)
                {
                    width = Math.Max(width, item.XAdvance);
                }
            }
            if (width == 0)
                throw new InvalidOperationException("invalid font");
            this.width = width;
        }

        internal void UpdateProperty()
        {
            this.charInfoByID = this.charInfos.ToDictionary(item => (char)item.ID);
        }

        #region IPropertyChangedNotifyable

        void IPropertyChangedNotifyable.InvokePropertyChangedEvent(string propertyName)
        {
            this.InvokePropertyChangedEvent(propertyName);
        }

        #endregion
    }
}
