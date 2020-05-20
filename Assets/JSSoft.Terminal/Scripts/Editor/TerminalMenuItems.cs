﻿// MIT License
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
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI.Editor
{
    static class TerminalMeniItems
    {
        [MenuItem("Terminal/Create Font Descriptor")]
        private static void CreateFontDescriptor()
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
                    fontDescriptor = TerminalFontDescriptor.Create(fntAsset);
                    AssetDatabase.CreateAsset(fontDescriptor, fontPath);
                }
                else
                {
                    TerminalFontDescriptor.Update(fontDescriptor, fntAsset);
                    EditorUtility.SetDirty(fontDescriptor);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Terminal/Create Font Descriptor", true)]
        private static bool ValidateCreateFontDescriptor()
        {
            var obj = Selection.activeObject;
            if (obj is TextAsset textAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(textAsset);
                return Path.GetExtension(assetPath) == ".fnt";
            }
            return false;
        }

        [MenuItem("Assets/Create/Terminal/Style Behaviour")]
        public static void CreateStyleBehaviour()
        {
            var action = ScriptableObject.CreateInstance<TerminalStyleBehaviourEndNameEditAction>();
            var icon = AssetPreview.GetMiniTypeThumbnail(typeof(MonoBehaviour));
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, "TerminalStyleBehaviour.cs", icon, string.Empty);
        }

        [MenuItem("Assets/Create/Terminal/Style Behaviour Asset", true)]
        private static bool ValidateCreateStyleBehaviourAsset()
        {
            if (Selection.activeObject is MonoScript monoScript)
            {
                var type = monoScript.GetClass();
                return type.IsSubclassOf(typeof(TerminalBehaviourBase));
            }
            return false;
        }

        [MenuItem("Assets/Create/Terminal/Style Behaviour Asset")]
        public static void CreateStyleBehaviourAsset()
        {
            if (Selection.activeObject is MonoScript monoScript)
            {
                var type = monoScript.GetClass();
                var obj = ScriptableObject.CreateInstance(type);
                var monoPath = AssetDatabase.GetAssetPath(monoScript);
                var assetDirectory = Path.GetDirectoryName(monoPath);
                var assetName = Path.GetFileNameWithoutExtension(monoPath);
                var assetPath = Path.Combine(assetDirectory, assetName + ".asset");
                AssetDatabase.CreateAsset(obj, assetPath);
            }
        }

        [MenuItem("Terminal/Test")]
        public static void Test()
        {
            var obj = Selection.activeObject;
            if (obj is GameObject gameObject)
            {
                var text = gameObject.GetComponent<Text>();
                var path = AssetDatabase.GetAssetPath(text.font);
                Debug.Log(path);
            }
        }

        // [MenuItem("Terminal/Create Font Group")]
        // private static void CreateFontGroup()
        // {
        //     var assetObject = Selection.activeObject;
        //     if (assetObject != null)
        //     {
        //         var assetPath = AssetDatabase.GetAssetPath(assetObject);
        //         var assetDirectory = Path.GetDirectoryName(assetPath);
        //         var fontPath = Path.Combine(assetDirectory, $"FontGroup.asset");
        //         var font = new TerminalFont();
        //         AssetDatabase.CreateAsset(font, fontPath);
        //     }
        // }

        [MenuItem("GameObject/UI/Terminal")]
        private static void CreateTerminalUI()
        {
            CreateTerminal();
        }

        private static void CreateTerminal()
        {
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                canvas = GameObject.FindObjectOfType<Canvas>();
            }
            var canvasTransform = canvas.transform;

            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            var font = AssetDatabase.LoadAssetAtPath("Assets/JSSoft.Terminal/Fonts/NanumGothicCoding.asset", typeof(TerminalFont)) as TerminalFont;
            var controller = AssetDatabase.LoadAssetAtPath("Assets/JSSoft.Terminal/Animations/TerminalScrollbar/TerminalScrollbar.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

            var terminalGridObj = new GameObject("Terminal") { layer = canvas.gameObject.layer };
            var terminalGrid = terminalGridObj.AddComponent<TerminalGrid>();
            var terminal = terminalGrid.Terminal;
            var terminalGridRect = terminalGrid.rectTransform;
            var terminalPadding = terminalGrid.Padding;
            terminalGridObj.AddComponent<TerminalOrientationBehaviour>();
            terminalGrid.material = new Material(Graphic.defaultGraphicMaterial);
            terminalGrid.BackgroundColor = TerminalGrid.DefaultBackgroundColor;
            terminalGridRect.SetParent(canvasTransform);
            terminalGridRect.anchorMin = new Vector2(0.5f, 0.5f);
            terminalGridRect.anchorMax = new Vector2(0.5f, 0.5f);
            terminalGridRect.pivot = new Vector2(0.5f, 0.5f);
            terminalGridRect.localPosition = Vector3.zero;

            var backgroundObj = new GameObject("TerminalBackground") { layer = canvas.gameObject.layer };
            var background = backgroundObj.AddComponent<TerminalBackground>();
            var backgroundRect = background.rectTransform;
            background.material = new Material(Graphic.defaultGraphicMaterial);
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
            cursor.material = new Material(Graphic.defaultGraphicMaterial);
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
            composition.ForegroundColor = TerminalGrid.DefaultForegroundColor;
            compositionRect.SetParent(terminalGridRect);
            compositionRect.anchorMin = Vector3.zero;
            compositionRect.anchorMax = Vector3.one;
            compositionRect.pivot = new Vector2(0.5f, 0.5f);
            compositionRect.offsetMin = Vector2.zero;
            compositionRect.offsetMax = Vector2.zero;

            var scrollbarObj = new GameObject("TerminalScrollbar", typeof(Image), typeof(TerminalScrollbar)) { layer = canvas.gameObject.layer };
            var scrollbar = scrollbarObj.GetComponent<TerminalScrollbar>();
            var animator = scrollbarObj.GetComponent<Animator>();
            var scrollbarImage = scrollbarObj.GetComponent<Image>();
            var scrollbarRect = scrollbarImage.rectTransform;
            scrollbar.Grid = terminalGrid;
            scrollbar.targetGraphic = scrollbarImage;
            scrollbar.direction = Scrollbar.Direction.TopToBottom;
            scrollbarImage.sprite = backgroundSprite;
            scrollbarImage.color = TerminalGrid.DefaultScrollbarColor;
            scrollbarImage.type = Image.Type.Sliced;
            // scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
            scrollbarRect.SetParent(terminalGridRect);
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = Vector3.one;
            scrollbarRect.pivot = new Vector2(0.5f, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(40, 0);
            scrollbarRect.localPosition = new Vector3(0, 0);
            scrollbarRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, terminalPadding.Right, 20);
            animator.runtimeAnimatorController = controller;

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
            handleImage.color = TerminalGrid.DefaultScrollbarColor;
            handleImage.type = Image.Type.Sliced;
            // handleImage.pixelsPerUnitMultiplier = 0.5f;
            handleRect.SetParent(slidingAreaRect);
            handleRect.offsetMin = new Vector2(-10.0f, -10.0f);
            handleRect.offsetMax = new Vector2(10.0f, 10.0f);
            handleRect.anchorMin = Vector3.zero;
            handleRect.anchorMax = Vector3.one;
            handleRect.pivot = new Vector2(0.5f, 0.5f);

            scrollbar.handleRect = handleRect;
            terminalGrid.Font = font;
            terminal.AppendLine("hello world!");
            terminal.Prompt = "Prompt>";
        }
    }
}