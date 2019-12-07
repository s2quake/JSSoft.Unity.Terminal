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
using System.ComponentModel;
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
    public static class TerminalFontDescriptorEvents
    {
        private static readonly HashSet<TerminalFontDescriptor> descriptors = new HashSet<TerminalFontDescriptor>();

        public static void Register(TerminalFontDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (descriptors.Contains(descriptor) == true)
                throw new ArgumentException($"{nameof(descriptor)} is already exists.");
            descriptors.Add(descriptor);
            descriptor.Validated += Descriptor_Validated;
            descriptor.PropertyChanged += Descriptor_PropertyChanged;
        }

        public static void Unregister(TerminalFontDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (descriptors.Contains(descriptor) == false)
                throw new ArgumentException($"{nameof(descriptor)} does not exists.");
            descriptor.Validated -= Descriptor_Validated;
            descriptor.PropertyChanged -= Descriptor_PropertyChanged;
            descriptors.Remove(descriptor);
        }

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Descriptor_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

#if UNITY_EDITOR
        internal static void InvokeValidatedEvent(TerminalFontDescriptor fontDescriptor, EventArgs e)
        {
            Validated?.Invoke(fontDescriptor, e);
        }
#endif
    }
}
