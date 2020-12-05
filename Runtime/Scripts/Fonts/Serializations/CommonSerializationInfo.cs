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

using System.Xml.Serialization;

namespace JSSoft.Unity.Terminal.Fonts.Serializations
{
    public struct CommonSerializationInfo
    {
        [XmlAttribute("lineHeight")]
        public int LineHeight { get; set; }

        [XmlAttribute("base")]
        public int Base { get; set; }

        [XmlAttribute("scaleW")]
        public int ScaleW { get; set; }

        [XmlAttribute("scaleH")]
        public int ScaleH { get; set; }

        [XmlAttribute("pages")]
        public int Pages { get; set; }

        [XmlAttribute("packed")]
        public int Packed { get; set; }

        [XmlAttribute("alphaChnl")]
        public int AlphaChnl { get; set; }

        [XmlAttribute("redChnl")]
        public int RedChnl { get; set; }

        [XmlAttribute("greenChnl")]
        public int GreenChnl { get; set; }

        [XmlAttribute("blueChnl")]
        public int BlueChnl { get; set; }

        public static explicit operator CommonInfo(CommonSerializationInfo info)
        {
            return new CommonInfo()
            {
                LineHeight = info.LineHeight,
                BaseLine = info.Base,
                ScaleW = info.ScaleW,
                ScaleH = info.ScaleH,
                Pages = info.Pages,
                Packed = info.Packed != 0,
                AlphaChannel = info.AlphaChnl != 0,
                RedChannel = info.RedChnl != 0,
                GreenChannel = info.GreenChnl != 0,
                BlueChannel = info.BlueChnl != 0,
            };
        }
    }
}
