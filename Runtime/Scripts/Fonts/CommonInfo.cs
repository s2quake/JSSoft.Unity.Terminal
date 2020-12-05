////////////////////////////////////////////////////////////////////////////////
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
using UnityEngine;

namespace JSSoft.Unity.Terminal.Fonts
{
    [Serializable]
    public struct CommonInfo
    {
        [SerializeField]
        private int lineHeight;
        [SerializeField]
        private int baseLine;
        [SerializeField]
        private int scaleW;
        [SerializeField]
        private int scaleH;
        [SerializeField]
        private int pages;
        [SerializeField]
        private bool packed;
        [SerializeField]
        private bool alphaChannel;
        [SerializeField]
        private bool redChannel;
        [SerializeField]
        private bool greenChannel;
        [SerializeField]
        private bool blueChannel;

        public int LineHeight { get => this.lineHeight; set => this.lineHeight = value; }

        public int BaseLine { get => this.baseLine; set => this.baseLine = value; }

        public int ScaleW { get => this.scaleW; set => this.scaleW = value; }

        public int ScaleH { get => this.scaleH; set => this.scaleH = value; }

        public int Pages { get => this.pages; set => this.pages = value; }

        public bool Packed { get => this.packed; set => this.packed = value; }

        public bool AlphaChannel { get => this.alphaChannel; set => this.alphaChannel = value; }

        public bool RedChannel { get => this.redChannel; set => this.redChannel = value; }

        public bool GreenChannel { get => this.greenChannel; set => this.greenChannel = value; }

        public bool BlueChannel { get => this.blueChannel; set => this.blueChannel = value; }

        public static readonly CommonInfo Empty = new CommonInfo();
    }
}
