﻿// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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
using JSSoft.Unity.Terminal.Commands;
using JSSoft.Unity.Terminal.Fonts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal.Editor
{
    public static class TerminalMeniItems
    {
        private static Func<Type, Type> typeResolver;

        public static Func<Type, Type> TypeResolver
        {
            get => typeResolver ?? defaultTypeResolver;
            set => typeResolver = value;
        }

        private static readonly Func<Type, Type> defaultTypeResolver = new Func<Type, Type>((type) => type);

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
                    fontDescriptor = CreateFontDescriptor(fntAsset);
                    AssetDatabase.CreateAsset(fontDescriptor, fontPath);
                }
                else
                {
                    UpdateFontDescriptor(fontDescriptor, fntAsset);
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

        // [MenuItem("Terminal/Test")]
        // public static void Test()
        // {
        //     var obj = Selection.activeObject;
        //     if (obj is GameObject gameObject)
        //     {
        //         var text = gameObject.GetComponent<Text>();
        //         var path = AssetDatabase.GetAssetPath(text.font);
        //         Debug.Log(path);
        //     }
        // }

        [MenuItem("GameObject/UI/Terminals/Terminal")]
        private static void CreateTerminalUI()
        {
            var canvas = PrepareCanvas();
            CreateTerminal(canvas);
        }

        [MenuItem("GameObject/UI/Terminals/Terminal - Commands")]
        private static void CreateTerminalUICommands()
        {
            var canvas = PrepareCanvas();
            var terminalGridObj = CreateTerminal(canvas);
            terminalGridObj.AddComponent<CommandContextHost>();
        }

        [MenuItem("GameObject/UI/Terminals/Terminal Layout")]
        private static void CreateTerminalUILayout()
        {
            var canvas = PrepareCanvas();
            CreateTerminalLayout(canvas);
        }

        [MenuItem("GameObject/UI/Terminals/Terminal Full")]
        private static void CreateTerminalUIFull()
        {
            var canvas = PrepareCanvas();
            var terminalLayoutObj = CreateTerminalLayout(canvas);
            var rectVisibleController = terminalLayoutObj.GetComponentInParent<RectVisibleController>();
            var terminalGridObj = CreateTerminal(canvas);
            rectVisibleController.Grid = terminalGridObj.GetComponent<TerminalGridBase>();
        }

        [MenuItem("GameObject/UI/Terminals/Terminal Full - Commands")]
        private static void CreateTerminalUIFullCommands()
        {
            var canvas = PrepareCanvas();
            var terminalLayoutObj = CreateTerminalLayout(canvas);
            var rectVisibleController = terminalLayoutObj.GetComponentInParent<RectVisibleController>();
            var terminalGridObj = CreateTerminal(canvas);
            rectVisibleController.Grid = terminalGridObj.GetComponent<TerminalGridBase>();
            terminalGridObj.AddComponent<CommandContextHost>();
            PrepareStyles(canvas);
        }

        private static GameObject CreateTerminal(Canvas canvas)
        {
            var parentRect = PrepareParent();
            var dispatcher = PrepareDispatcher(canvas);
            var canvasTransform = canvas.transform;
            var pixelRect = canvas.pixelRect;

            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            var font = AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Fonts/NanumGothicCoding.asset", typeof(TerminalFont)) as TerminalFont;
            var controller = AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Animations/TerminalScrollbar/TerminalScrollbar.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
            var types = new List<Type>()
            {
                { typeof(Terminal) },
                { typeof(TerminalGrid) },
                { typeof(TerminalBackground) },
                { typeof(TerminalForeground) },
                { typeof(TerminalForegroundItem) },
                { typeof(TerminalComposition) },
                { typeof(TerminalCompositionBackground) },
                { typeof(TerminalCompositionForeground) },
                { typeof(TerminalCursor) },
                { typeof(TerminalScrollbar) },
            };
            var typeByType = CollectTypes(types);

            var terminalGridObj = new GameObject(nameof(Terminal), typeByType[typeof(Terminal)]) { layer = canvas.gameObject.layer };
            var terminalGrid = terminalGridObj.AddComponent(typeByType[typeof(TerminalGrid)]) as TerminalGrid;
            var terminal = terminalGrid.GetComponent<Terminal>();
            var terminalGridRect = terminalGrid.rectTransform;
            var terminalPadding = terminalGrid.Padding;
            terminal.SetDispatcher(dispatcher);
            terminalGrid.material = new Material(Graphic.defaultGraphicMaterial);
            terminalGrid.BackgroundColor = TerminalGrid.DefaultBackgroundColor;
            terminalGridRect.SetParent(parentRect);
            terminalGridRect.anchorMin = Vector3.zero;
            terminalGridRect.anchorMax = Vector3.one;
            terminalGridRect.pivot = new Vector2(0.5f, 0.5f);
            terminalGridRect.offsetMin = Vector2.zero;
            terminalGridRect.offsetMax = Vector2.zero;

            var backgroundObj = new GameObject(nameof(TerminalBackground)) { layer = canvas.gameObject.layer };
            var background = backgroundObj.AddComponent(typeByType[typeof(TerminalBackground)]) as TerminalBackground;
            var backgroundRect = background.rectTransform;
            background.material = new Material(Graphic.defaultGraphicMaterial);
            background.Grid = terminalGrid;
            backgroundRect.SetParent(terminalGridRect);
            backgroundRect.anchorMin = Vector3.zero;
            backgroundRect.anchorMax = Vector3.one;
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            var cursorObj = new GameObject(nameof(TerminalCursor)) { layer = canvas.gameObject.layer };
            var cursor = cursorObj.AddComponent(typeByType[typeof(TerminalCursor)]) as TerminalCursor;
            var cursorRect = cursor.rectTransform;
            cursor.material = new Material(Graphic.defaultGraphicMaterial);
            cursor.Grid = terminalGrid;
            cursorRect.SetParent(terminalGridRect);
            cursorRect.anchorMin = Vector3.zero;
            cursorRect.anchorMax = Vector3.one;
            cursorRect.pivot = new Vector2(0.5f, 0.5f);
            cursorRect.offsetMin = Vector2.zero;
            cursorRect.offsetMax = Vector2.zero;

            var foregroundObj = new GameObject(nameof(TerminalForeground)) { layer = canvas.gameObject.layer };
            var foreground = foregroundObj.AddComponent(typeByType[typeof(TerminalForeground)]) as TerminalForeground;
            var foregroundRect = foregroundObj.GetComponent<RectTransform>();
            foreground.Grid = terminalGrid;
            foreground.ItemType = typeByType[typeof(TerminalForegroundItem)].AssemblyQualifiedName;
            foregroundRect.SetParent(terminalGridRect);
            foregroundRect.anchorMin = Vector3.zero;
            foregroundRect.anchorMax = Vector3.one;
            foregroundRect.pivot = new Vector2(0.5f, 0.5f);
            foregroundRect.offsetMin = Vector2.zero;
            foregroundRect.offsetMax = Vector2.zero;

            var compositionObj = new GameObject(nameof(TerminalComposition)) { layer = canvas.gameObject.layer };
            var composition = compositionObj.AddComponent(typeByType[typeof(TerminalComposition)]) as TerminalComposition;
            var compositionRect = composition.GetComponent<RectTransform>();
            composition.Grid = terminalGrid;
            composition.ForegroundColor = terminalGrid.ForegroundColor;
            composition.BackgroundColor = terminalGrid.BackgroundColor;
            compositionRect.SetParent(terminalGridRect);
            compositionRect.anchorMin = Vector3.zero;
            compositionRect.anchorMax = Vector3.one;
            compositionRect.pivot = new Vector2(0.5f, 0.5f);
            compositionRect.offsetMin = Vector2.zero;
            compositionRect.offsetMax = Vector2.zero;

            var compositionBackgroundObj = new GameObject("Background") { layer = canvas.gameObject.layer };
            var compositionBackground = compositionBackgroundObj.AddComponent(typeByType[typeof(TerminalCompositionBackground)]) as TerminalCompositionBackground;
            var compositionBackgroundRect = compositionBackground.rectTransform;
            compositionBackground.Composition = composition;
            compositionBackgroundRect.SetParent(compositionRect);
            compositionBackgroundRect.anchorMin = Vector3.zero;
            compositionBackgroundRect.anchorMax = Vector3.one;
            compositionBackgroundRect.pivot = new Vector2(0.5f, 0.5f);
            compositionBackgroundRect.offsetMin = Vector2.zero;
            compositionBackgroundRect.offsetMax = Vector2.zero;

            var compositionForegroundObj = new GameObject("Foreground") { layer = canvas.gameObject.layer };
            var compositionForeground = compositionForegroundObj.AddComponent(typeByType[typeof(TerminalCompositionForeground)]) as TerminalCompositionForeground;
            var compositionForegroundRect = compositionForeground.rectTransform;
            compositionForeground.Composition = composition;
            compositionForegroundRect.SetParent(compositionRect);
            compositionForegroundRect.anchorMin = Vector3.zero;
            compositionForegroundRect.anchorMax = Vector3.one;
            compositionForegroundRect.pivot = new Vector2(0.5f, 0.5f);
            compositionForegroundRect.offsetMin = Vector2.zero;
            compositionForegroundRect.offsetMax = Vector2.zero;

            var scrollbarObj = new GameObject(nameof(TerminalScrollbar), typeof(Image), typeByType[typeof(TerminalScrollbar)]) { layer = canvas.gameObject.layer };
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
#if UNITY_2019_3_OR_NEWER
            scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
#endif
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
#if UNITY_2019_3_OR_NEWER            
            handleImage.pixelsPerUnitMultiplier = 0.5f;
#endif
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

            Selection.activeGameObject = terminalGridObj;
            return Selection.activeGameObject;
        }

        private static GameObject CreateTerminalLayout(Canvas canvas)
        {
            var parentRect = PrepareParent();

            var types = new List<Type>()
            {
                { typeof(VerticalLayoutGroup) },
                { typeof(TerminalKeyboardLayoutGroup) },
                { typeof(TerminalRectVisibleController) },
            };

            var controllerAsset = AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Animations/RectVisibleController/RectVisibleController.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

            var terminalRootObj = new GameObject("TerminalRoot", types.ToArray()) { layer = canvas.gameObject.layer };
            var terminalRootRect = terminalRootObj.GetComponent<RectTransform>();
            var terminalVertLayout = terminalRootObj.GetComponent<VerticalLayoutGroup>();
            var terminalKeyboardLayout = terminalRootObj.GetComponent<TerminalKeyboardLayoutGroup>();
            var temrinalAnimator = terminalRootObj.GetComponent<Animator>();
            temrinalAnimator.runtimeAnimatorController = controllerAsset;
            terminalVertLayout.padding = new RectOffset(2, 2, 2, 2);
            terminalVertLayout.childControlWidth = true;
            terminalVertLayout.childControlHeight = true;
            terminalVertLayout.childForceExpandHeight = true;
            terminalVertLayout.childForceExpandWidth = true;
            terminalRootRect.SetParent(parentRect);
            terminalRootRect.anchorMin = Vector3.zero;
            terminalRootRect.anchorMax = Vector3.one;
            terminalRootRect.pivot = new Vector2(0.5f, 0.5f);
            terminalRootRect.offsetMin = Vector2.zero;
            terminalRootRect.offsetMax = Vector2.zero;

            var terminalHostObj = new GameObject("TerminalLayout", typeof(VerticalLayoutGroup), typeof(LayoutElement)) { layer = canvas.gameObject.layer };
            var terminalHostVertLayout = terminalHostObj.GetComponent<VerticalLayoutGroup>();
            var terminalHostRect = terminalHostObj.GetComponent<RectTransform>();
            var terminalHostLayoutElement = terminalHostObj.GetComponent<LayoutElement>();
            terminalHostVertLayout.spacing = 2;
            terminalHostVertLayout.childControlWidth = true;
            terminalHostVertLayout.childControlHeight = true;
            terminalHostVertLayout.childForceExpandHeight = true;
            terminalHostVertLayout.childForceExpandWidth = true;
            terminalHostLayoutElement.flexibleHeight = 1;
            terminalHostRect.SetParent(terminalRootRect);

            var keyboardObj = new GameObject("KeyboardLayout", typeof(LayoutElement)) { layer = canvas.gameObject.layer };
            var keyboardRect = keyboardObj.GetComponent<RectTransform>();
            var keyboardLayoutElement = keyboardObj.GetComponent<LayoutElement>();
            keyboardLayoutElement.ignoreLayout = true;
            keyboardRect.SetParent(terminalRootRect);

            terminalKeyboardLayout.TerminalLayout = terminalHostLayoutElement;
            terminalKeyboardLayout.KeyboardLayout = keyboardLayoutElement;

            Selection.activeGameObject = terminalHostObj;
            return Selection.activeGameObject;
        }

        private static Canvas PrepareCanvas()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponentInParent<Canvas>() is Canvas canvas)
                {
                    return canvas;
                }
                return null;
            }
            else
            {
                var canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                    canvas = GameObject.FindObjectOfType<Canvas>();
                }
                return canvas;
            }
        }

        private static RectTransform PrepareParent()
        {
            if (Selection.activeTransform is RectTransform rectTransform)
            {
                return rectTransform;
            }
            return GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        }

        private static TerminalDispatcher PrepareDispatcher(Canvas canvas)
        {
            var dispatcher = canvas.GetComponentInChildren<TerminalDispatcher>();
            var canvasRect = canvas.GetComponent<RectTransform>();
            if (dispatcher == null)
            {
                var dispatcherObject = new GameObject(nameof(TerminalDispatcher), typeof(TerminalDispatcher), typeof(RectTransform));
                var dispatcherRect = dispatcherObject.GetComponent<RectTransform>();
                dispatcher = dispatcherObject.GetComponent<TerminalDispatcher>();
                dispatcherRect.SetParent(canvasRect);
            }
            return dispatcher;
        }

        private static TerminalStyles PrepareStyles(Canvas canvas)
        {
            var styles = canvas.GetComponentInChildren<TerminalStyles>();
            var canvasRect = canvas.GetComponent<RectTransform>();
            if (styles == null)
            {
                var stylesObject = new GameObject(nameof(TerminalStyles), typeof(TerminalStyles), typeof(RectTransform));
                var stylesRect = stylesObject.GetComponent<RectTransform>();
                styles = stylesObject.GetComponent<TerminalStyles>();
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Basic.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Grass.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/HomeBrew.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Man Page.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Novel.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Ocean.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Pro.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Red Sands.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Silver Aerogel.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Solid Colors.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/Console.asset", typeof(TerminalStyle)) as TerminalStyle);
                styles.Styles.Add(AssetDatabase.LoadAssetAtPath("Assets/Plugins/JSSoft.Unity.Terminal/Styles/PowerShell.asset", typeof(TerminalStyle)) as TerminalStyle);
                stylesRect.SetParent(canvasRect);
            }
            return styles;
        }

        private static IDictionary<Type, Type> CollectTypes(IEnumerable<Type> types)
        {
            var typeByType = new Dictionary<Type, Type>(types.Count());
            foreach (var item in types)
            {
                var type = TypeResolver(item);
                if (type != item && type.IsSubclassOf(item) == false)
                    throw new InvalidOperationException($"'{type}' is not subclass of '{item}'");
                typeByType.Add(item, type);
            }
            return typeByType;
        }

        public static TerminalFontDescriptor CreateFontDescriptor(TextAsset fntAsset)
        {
            var font = new TerminalFontDescriptor();
            UpdateFontDescriptor(font, fntAsset);
            return font;
        }

        private static void UpdateFontDescriptor(TerminalFontDescriptor font, TextAsset fntAsset)
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
                    var item = charInfos[i];
                    var charInfo = (CharInfo)item;
                    charInfo.Texture = font.textures[item.Page];
                    font.charInfos[i] = charInfo;
                }
                font.UpdateWidth();
                font.UpdateProperty();
            }
        }
    }
}