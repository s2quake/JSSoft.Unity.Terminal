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

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JSSoft.Communication.Shells;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI.Editor
{
    static class TerminalMeniItems
    {
        [MenuItem("Terminal/Create Font")]
        private static void CreateFont()
        {
            var obj = Selection.activeObject;
            if (obj is TextAsset fntAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(fntAsset);
                var assetName = Path.GetFileNameWithoutExtension(assetPath);
                var assetDirectory = Path.GetDirectoryName(assetPath);
                var fontPath = Path.Combine(assetDirectory, $"{assetName}.asset");
                var fontDescriptor = AssetDatabase.LoadAssetAtPath(fontPath, typeof(TerminalFontDescriptor)) as TerminalFontDescriptor;
                if (fontDescriptor == null)
                {
                    var font = TerminalFontDescriptor.Create(fntAsset);
                    AssetDatabase.CreateAsset(font, fontPath);
                }
                else
                {
                    TerminalFontDescriptor.Update(fontDescriptor, fntAsset);
                    // AssetDatabase.ImportAsset(fontPath);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Terminal/Create Font", true)]
        private static bool ValidateCreateFont()
        {
            var obj = Selection.activeObject;
            if (obj is TextAsset textAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(textAsset);
                return Path.GetExtension(assetPath) == ".fnt";
            }
            return false;
        }

        [MenuItem("Terminal/Create Font Group")]
        private static void CreateFontGroup()
        {
            var assetObject = Selection.activeObject;
            if (assetObject != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(assetObject);
                var assetDirectory = Path.GetDirectoryName(assetPath);
                var fontPath = Path.Combine(assetDirectory, $"FontGroup.asset");
                var font = new TerminalFont();
                AssetDatabase.CreateAsset(font, fontPath);
            }
        }

        [MenuItem("GameObject/UI/Terminal")]
        private static void CreateTerminalUI()
        {
            CreateTerminal();
        }

        private static void CreateTerminal()
        {
            // var width = 800.0f;
            // var height = 600.0f;
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                canvas = GameObject.FindObjectOfType<Canvas>();
            }
            var canvasTransform = canvas.transform;

            var terminalGridObj = new GameObject("Terminal") { layer = canvas.gameObject.layer };
            var terminalGrid = terminalGridObj.AddComponent<TerminalGrid>();
            var terminalGridRect = terminalGrid.rectTransform;
            var terminalPadding = terminalGrid.Padding;
            terminalGridRect.SetParent(canvasTransform);
            terminalGridRect.anchorMin = new Vector2(0.5f, 0.5f);
            terminalGridRect.anchorMax = new Vector2(0.5f, 0.5f);
            terminalGridRect.pivot = new Vector2(0.5f, 0.5f);
            terminalGridRect.localPosition = Vector3.zero;

            var backgroundObj = new GameObject("TerminalBackground") { layer = canvas.gameObject.layer };
            var background = backgroundObj.AddComponent<TerminalBackground>();
            var backgroundRect = background.rectTransform;
            background.Grid = terminalGrid;
            backgroundRect.SetParent(terminalGridRect);
            backgroundRect.anchorMin = Vector3.zero;
            backgroundRect.anchorMax = Vector3.one;
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            var cursorObj = new GameObject("TerminalCursor") { layer = canvas.gameObject.layer };
            var cursor = cursorObj.AddComponent<TerminalCursor>();
            var cursorRect = cursor.rectTransform;
            cursor.Grid = terminalGrid;
            cursorRect.SetParent(terminalGridRect);
            cursorRect.anchorMin = Vector3.zero;
            cursorRect.anchorMax = Vector3.one;
            cursorRect.pivot = new Vector2(0.5f, 0.5f);
            cursorRect.offsetMin = Vector2.zero;
            cursorRect.offsetMax = Vector2.zero;

            var foregroundObj = new GameObject("TerminalForeground") { layer = canvas.gameObject.layer };
            var foreground = foregroundObj.AddComponent<TerminalForeground>();
            var foregroundRect = foregroundObj.GetComponent<RectTransform>();
            foreground.Grid = terminalGrid;
            foregroundRect.SetParent(terminalGridRect);
            foregroundRect.anchorMin = Vector3.zero;
            foregroundRect.anchorMax = Vector3.one;
            foregroundRect.pivot = new Vector2(0.5f, 0.5f);
            foregroundRect.offsetMin = Vector2.zero;
            foregroundRect.offsetMax = Vector2.zero;

            var compositionObj = new GameObject("TerminalComposition") { layer = canvas.gameObject.layer };
            var composition = compositionObj.AddComponent<TerminalComposition>();
            var compositionRect = composition.rectTransform;
            composition.Grid = terminalGrid;
            compositionRect.SetParent(terminalGridRect);
            compositionRect.anchorMin = Vector3.zero;
            compositionRect.anchorMax = Vector3.one;
            compositionRect.pivot = new Vector2(0.5f, 0.5f);
            compositionRect.offsetMin = Vector2.zero;
            compositionRect.offsetMax = Vector2.zero;

            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            var scrollbarObj = new GameObject("TerminalScrollbar", typeof(Image)) { layer = canvas.gameObject.layer };
            var scrollbarHost = scrollbarObj.AddComponent<TerminalScrollbarHost>();
            var scrollbar = scrollbarObj.GetComponent<TerminalScrollbar>();
            var scrollbarImage = scrollbarObj.GetComponent<Image>();
            var scrollbarRect = scrollbarImage.rectTransform;
            scrollbarHost.Grid = terminalGrid;
            scrollbar.targetGraphic = scrollbarImage;
            scrollbar.direction = Scrollbar.Direction.TopToBottom;
            scrollbarImage.sprite = backgroundSprite;
            scrollbarImage.type = Image.Type.Sliced;
            scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
            scrollbarRect.SetParent(terminalGridRect);
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = Vector3.one;
            scrollbarRect.pivot = new Vector2(0.5f, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(40, 0);
            scrollbarRect.localPosition = new Vector3(0, 0);
            scrollbarRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, terminalPadding.Right, 20);

            var slidingAreaObj = new GameObject("Sliding Area", typeof(RectTransform)) { layer = canvas.gameObject.layer };
            var slidingAreaRect = slidingAreaObj.GetComponent<RectTransform>();
            slidingAreaRect.SetParent(scrollbarRect);
            slidingAreaRect.offsetMin = new Vector2(10.0f, 10.0f);
            slidingAreaRect.offsetMax = new Vector2(-10.0f, -10.0f);
            slidingAreaRect.anchorMin = Vector3.zero;
            slidingAreaRect.anchorMax = Vector3.one;
            slidingAreaRect.pivot = new Vector2(0.5f, 0.5f);

            var handleObj = new GameObject("Handle", typeof(RectTransform), typeof(Image)) { layer = canvas.gameObject.layer };
            var handleRect = handleObj.GetComponent<RectTransform>();
            var handleImage = handleObj.GetComponent<Image>();
            handleImage.sprite = uiSprite;
            handleImage.type = Image.Type.Sliced;
            handleImage.pixelsPerUnitMultiplier = 0.5f;
            handleRect.SetParent(slidingAreaRect);
            handleRect.offsetMin = new Vector2(-10.0f, -10.0f);
            handleRect.offsetMax = new Vector2(10.0f, 10.0f);
            handleRect.anchorMin = Vector3.zero;
            handleRect.anchorMax = Vector3.one;
            handleRect.pivot = new Vector2(0.5f, 0.5f);

            scrollbar.handleRect = handleRect;
        }
    }
}