// MIT License
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

using JSSoft.Unity.Terminal.Commands;
using JSSoft.Unity.Terminal.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal.Editor
{
    public static class TerminalMenuItems
    {
        [MenuItem("GameObject/UI/Terminals/Terminal")]
        public static void CreateTerminalUI()
        {
            var canvas = PrepareCanvas();
            if (canvas == null)
                throw new InvalidOperationException("cannot found Canvas.");
            var terminalGridObj = CreateTerminal(canvas);
            InvokeTerminalCreated(terminalGridObj, TerminalCreation.Terminal);
            Undo.RegisterCreatedObjectUndo(terminalGridObj, "Create Terminal");
        }

        [MenuItem("GameObject/UI/Terminals/Terminal - Commands")]
        public static void CreateTerminalUICommands()
        {
            var canvas = PrepareCanvas();
            if (canvas == null)
                throw new InvalidOperationException("cannot found Canvas.");
            var terminalGridObj = CreateTerminal(canvas);
            terminalGridObj.AddComponent<CommandContextHost>();
            PrepareStyles(canvas);
            InvokeTerminalCreated(terminalGridObj, TerminalCreation.Terminal | TerminalCreation.Commands);
            Undo.RegisterCreatedObjectUndo(terminalGridObj, "Create Terminal - Commands");
        }

        [MenuItem("GameObject/UI/Terminals/Terminal Layout")]
        public static void CreateTerminalUILayout()
        {
            var canvas = PrepareCanvas();
            if (canvas == null)
                throw new InvalidOperationException("cannot found Canvas.");
            var terminalGridObj = CreateTerminalLayout(canvas);
            InvokeTerminalCreated(terminalGridObj, TerminalCreation.Layout);
            Undo.RegisterCreatedObjectUndo(terminalGridObj, "Create Terminal Layout");
        }

        [MenuItem("GameObject/UI/Terminals/Terminal Full")]
        public static void CreateTerminalUIFull()
        {
            var canvas = PrepareCanvas();
            if (canvas == null)
                throw new InvalidOperationException("cannot found Canvas.");
            var terminalLayoutObj = CreateTerminalLayout(canvas);
            var rectVisibleController = terminalLayoutObj.GetComponentInParent<SlidingController>();
            var terminalGridObj = CreateTerminal(canvas);
            rectVisibleController.Grid = terminalGridObj.GetComponent<TerminalGridBase>();
            InvokeTerminalCreated(terminalGridObj, TerminalCreation.Terminal | TerminalCreation.Layout);
            Undo.RegisterCreatedObjectUndo(terminalGridObj, "Create Terminal Full");
        }

        [MenuItem("GameObject/UI/Terminals/Terminal Full - Commands")]
        public static void CreateTerminalUIFullCommands()
        {
            var canvas = PrepareCanvas();
            if (canvas == null)
                throw new InvalidOperationException("cannot found Canvas.");
            var terminalLayoutObj = CreateTerminalLayout(canvas);
            var rectVisibleController = terminalLayoutObj.GetComponentInParent<SlidingController>();
            var terminalGridObj = CreateTerminal(canvas);
            rectVisibleController.Grid = terminalGridObj.GetComponent<TerminalGridBase>();
            terminalGridObj.AddComponent<CommandContextHost>();
            PrepareStyles(canvas);
            InvokeTerminalCreated(terminalGridObj, TerminalCreation.Terminal | TerminalCreation.Layout | TerminalCreation.Terminal);
            Undo.RegisterCreatedObjectUndo(terminalGridObj, "Create Terminal Full - Commands");
        }

        [MenuItem("Assets/Create/Terminal/Create Font Descriptor")]
        private static void CreateFontDescriptor()
        {
            var obj = Selection.activeObject;
            if (obj is TextAsset textAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(textAsset);
                var assetName = Path.GetFileNameWithoutExtension(assetPath);
                var assetDirectory = Path.GetDirectoryName(assetPath);
                var fontPath = Path.Combine(assetDirectory, $"{assetName}.asset");
                var fontDescriptor = AssetDatabase.LoadAssetAtPath(fontPath, typeof(TerminalFontDescriptor)) as TerminalFontDescriptor;
                var resolver = new DescriptorResolver();
                if (fontDescriptor == null)
                {
                    fontDescriptor = TerminalFontDescriptor.Create(textAsset, resolver);
                    AssetDatabase.CreateAsset(fontDescriptor, fontPath);
                }
                else
                {
                    fontDescriptor.Refresh(textAsset, resolver);
                    EditorUtility.SetDirty(fontDescriptor);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Assets/Create/Terminal/Create Font Descriptor", true)]
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

        public static EventHandler<TerminalCreatedEventArgs> TerminalCreated;

        internal static T FindAsset<T>(string path) where T : class
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var items = AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}");
            foreach (var item in items)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(item);
                if (assetPath.EndsWith(path) == true)
                {
                    return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
                }
            }
            throw new ArgumentException($"cannot found asset: '{path}'", nameof(path));
        }

        private static void InvokeTerminalCreated(GameObject gameObject, TerminalCreation creation)
        {
            TerminalCreated?.Invoke(null, new TerminalCreatedEventArgs(gameObject, creation));
        }

        private static GameObject CreateTerminal(Canvas canvas)
        {
            var parentRect = PrepareParent();
            var dispatcher = PrepareDispatcher(canvas);
            var canvasTransform = canvas.transform;
            var pixelRect = canvas.pixelRect;

            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            var font = FindAsset<TerminalFont>("Fonts/NanumGothicCoding.asset");
            var controller = FindAsset<RuntimeAnimatorController>("Animations/TerminalScrollbar/TerminalScrollbar.controller");

            var terminalGridObj = new GameObject(nameof(Terminal), typeof(Terminal)) { layer = canvas.gameObject.layer };
            var terminalGrid = terminalGridObj.AddComponent(typeof(TerminalGrid)) as TerminalGrid;
            var terminal = terminalGrid.GetComponent<Terminal>();
            var terminalGridState = terminalGridObj.AddComponent<TerminalGridState>();
            var terminalGridRect = terminalGrid.rectTransform;
            var terminalPadding = terminalGrid.Padding;
            terminal.SetDispatcher(dispatcher);
            terminalGrid.material = new Material(Graphic.defaultGraphicMaterial);
            terminalGridRect.SetParent(parentRect);
            terminalGridRect.anchorMin = Vector3.zero;
            terminalGridRect.anchorMax = Vector3.one;
            terminalGridRect.pivot = new Vector2(0.5f, 0.5f);
            terminalGridRect.offsetMin = Vector2.zero;
            terminalGridRect.offsetMax = Vector2.zero;

            var backgroundObj = new GameObject(nameof(TerminalBackground)) { layer = canvas.gameObject.layer };
            var background = backgroundObj.AddComponent(typeof(TerminalBackground)) as TerminalBackground;
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
            var cursor = cursorObj.AddComponent(typeof(TerminalCursor)) as TerminalCursor;
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
            var foreground = foregroundObj.AddComponent(typeof(TerminalForeground)) as TerminalForeground;
            var foregroundRect = foregroundObj.GetComponent<RectTransform>();
            foreground.Grid = terminalGrid;
            foregroundRect.SetParent(terminalGridRect);
            foregroundRect.anchorMin = Vector3.zero;
            foregroundRect.anchorMax = Vector3.one;
            foregroundRect.pivot = new Vector2(0.5f, 0.5f);
            foregroundRect.offsetMin = Vector2.zero;
            foregroundRect.offsetMax = Vector2.zero;

            var compositionObj = new GameObject(nameof(TerminalComposition)) { layer = canvas.gameObject.layer };
            var composition = compositionObj.AddComponent(typeof(TerminalComposition)) as TerminalComposition;
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
            var compositionBackground = compositionBackgroundObj.AddComponent(typeof(TerminalCompositionBackground)) as TerminalCompositionBackground;
            var compositionBackgroundRect = compositionBackground.rectTransform;
            compositionBackground.Composition = composition;
            compositionBackgroundRect.SetParent(compositionRect);
            compositionBackgroundRect.anchorMin = Vector3.zero;
            compositionBackgroundRect.anchorMax = Vector3.one;
            compositionBackgroundRect.pivot = new Vector2(0.5f, 0.5f);
            compositionBackgroundRect.offsetMin = Vector2.zero;
            compositionBackgroundRect.offsetMax = Vector2.zero;

            var compositionForegroundObj = new GameObject("Foreground") { layer = canvas.gameObject.layer };
            var compositionForeground = compositionForegroundObj.AddComponent(typeof(TerminalCompositionForeground)) as TerminalCompositionForeground;
            var compositionForegroundRect = compositionForeground.rectTransform;
            compositionForeground.Composition = composition;
            compositionForegroundRect.SetParent(compositionRect);
            compositionForegroundRect.anchorMin = Vector3.zero;
            compositionForegroundRect.anchorMax = Vector3.one;
            compositionForegroundRect.pivot = new Vector2(0.5f, 0.5f);
            compositionForegroundRect.offsetMin = Vector2.zero;
            compositionForegroundRect.offsetMax = Vector2.zero;

            var scrollbarObj = new GameObject(nameof(TerminalScrollbar), typeof(Image), typeof(TerminalScrollbar)) { layer = canvas.gameObject.layer };
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
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
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
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;

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
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER            
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
            terminal.Append("hello world!");
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
                { typeof(TerminalSlidingController) },
            };

            var controllerAsset = FindAsset<RuntimeAnimatorController>("Animations/SlidingController/SlidingController.controller");

            var terminalRootObj = new GameObject("TerminalRoot", types.ToArray()) { layer = canvas.gameObject.layer };
            var terminalRootRect = terminalRootObj.GetComponent<RectTransform>();
            var terminalVertLayout = terminalRootObj.GetComponent<VerticalLayoutGroup>();
            var terminalKeyboardLayout = terminalRootObj.GetComponent<TerminalKeyboardLayoutGroup>();
            var temrinalAnimator = terminalRootObj.GetComponent<Animator>();
            temrinalAnimator.runtimeAnimatorController = controllerAsset;
            temrinalAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
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
            var terminalDockController = terminalHostObj.AddComponent<TerminalDockController>();
            var terminalDockControllerState = terminalHostObj.AddComponent<TerminalDockControllerState>();
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
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                if (Selection.activeGameObject != null)
                {
                    if (Selection.activeGameObject.GetComponentInParent<Canvas>() is Canvas canvas && PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject) == null)
                    {
                        return canvas;
                    }
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
            else
            {
                if (Selection.activeGameObject != null)
                {
                    if (Selection.activeGameObject.GetComponentInParent<Canvas>() is Canvas canvas)
                    {
                        return canvas;
                    }
                }
            }
            return null;
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
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Basic.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Grass.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Homebrew.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Man Page.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Novel.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Ocean.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Pro.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Red Sands.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Silver Aerogel.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Solid Colors.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/Console.asset"));
                styles.Styles.Add(FindAsset<TerminalStyle>("Styles/PowerShell.asset"));
                stylesRect.SetParent(canvasRect);
            }
            return styles;
        }

        #region DescriptorResolver

        class DescriptorResolver : TerminalFontResolver
        {
            public override Texture2D GetTexture(TextAsset textAsset, string path)
            {
                var assetPath = AssetDatabase.GetAssetPath(textAsset);
                var assetDirectory = Path.GetDirectoryName(assetPath);
                var texturePath = Path.Combine(assetDirectory, path);
                return AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            }
        }

        #endregion
    }
}