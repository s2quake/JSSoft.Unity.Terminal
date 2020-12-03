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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System.Xml.Serialization;

namespace JSSoft.Unity.Terminal.Fonts.Serializations
{
    public struct InfoSerializationInfo
    {
        [XmlAttribute("face")]
        public string Face { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("bold")]
        public int Bold { get; set; }

        [XmlAttribute("italic")]
        public int Italic { get; set; }

        [XmlAttribute("charset")]
        public string Charset { get; set; }

        [XmlAttribute("unicode")]
        public int Unicode { get; set; }

        [XmlAttribute("stretchH")]
        public int StretchH { get; set; }

        [XmlAttribute("smooth")]
        public int Smooth { get; set; }

        [XmlAttribute("aa")]
        public int Aa { get; set; }

        [XmlAttribute("padding")]
        public string Padding { get; set; }

        [XmlAttribute("spacing")]
        public string Spacing { get; set; }

        [XmlAttribute("outline")]
        public int Outline { get; set; }

        public static explicit operator BaseInfo(InfoSerializationInfo info)
        {
            return new BaseInfo()
            {
                Face = info.Face,
                Size = info.Size,
                Bold = info.Bold != 0,
                Italic = info.Italic != 0,
                Charset = info.Charset,
                Unicode = info.Unicode != 0,
                StretchH = info.StretchH,
                Smooth = info.Smooth != 0,
                Aa = info.Aa != 0,
                Padding = info.PaddingValue,
                Spacing = info.SpacingValue,
                Outline = info.Outline != 0,
            };
        }

        public (int Top, int Right, int Bottom, int Left) PaddingValue
        {
            get
            {
                var items = this.Padding.Split(',');
                return (int.Parse(items[0]), int.Parse(items[1]), int.Parse(items[2]), int.Parse(items[3]));
            }
            set
            {
                this.Padding = $"{value.Top},{value.Right},{value.Bottom},{value.Left}";
            }
        }

        public (int Vertical, int Horizontal) SpacingValue
        {
            get
            {
                var items = this.Padding.Split(',');
                return (int.Parse(items[0]), int.Parse(items[1]));
            }
            set
            {
                this.Spacing = $"{value.Vertical},{value.Horizontal}";
            }
        }
    }
}
