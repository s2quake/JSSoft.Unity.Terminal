// // MIT License
// // 
// // Copyright (c) 2019 Jeesu Choi
// // 
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// // 
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

// using JSSoft.Communication.Shells;
// using TMPro;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;

// namespace JSSoft.UI.Editor
// {
//     static class TerminalMeniItems
//     {
//         [MenuItem("GameObject/UI/Terminal")]
//         private static void CreateTerminalUI()
//         {
//             CreateTerminal();
//         }

//         private static void CreateTerminal()
//         {
//             // var width = 800.0f;
//             // var height = 600.0f;
//             var canvas = GameObject.FindObjectOfType<Canvas>();
//             if (canvas == null)
//             {
//                 EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
//                 canvas = GameObject.FindObjectOfType<Canvas>();
//             }

//             var fontAsset = (TMP_FontAsset)AssetDatabase.LoadAssetAtPath("Assets/JSSoft Terminal/Fonts/SFMono-Regular.asset", typeof(TMP_FontAsset));
//             var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");

//             var terminalObj = new GameObject("Terminal") { layer = canvas.gameObject.layer };
//             var terminalRect = terminalObj.AddComponent<RectTransform>();
//             terminalObj.AddComponent<CanvasRenderer>();
//             var terminalImage = terminalObj.AddComponent<Image>();
//             // terminalImage.sprite = backgroundSprite;
//             terminalImage.type = Image.Type.Sliced;

//             var terminal = terminalObj.AddComponent<Terminal>();
//             terminal.targetGraphic = terminalImage;
//             terminal.lineType = TMP_InputField.LineType.MultiLineSubmit;
//             terminal.onFocusSelectAll = false;
//             terminal.resetOnDeActivation = false;
//             terminal.restoreOriginalTextOnEscape = false;
//             terminal.richText = false;

//             var textAreaObj = new GameObject("Text Area") { layer = canvas.gameObject.layer };
//             var textAreaRect = textAreaObj.AddComponent<RectTransform>();
//             textAreaObj.AddComponent<RectMask2D>();

//             var textObj = new GameObject("Text") { layer = canvas.gameObject.layer };
//             var textRect = textObj.AddComponent<RectTransform>();
//             var textMesh = textObj.AddComponent<TerminalText>();
//             textMesh.terminal = terminal;
//             textMesh.richText = false;
//             textMesh.geometrySortingOrder = VertexSortingOrder.Reverse;
//             textMesh.overflowMode = TextOverflowModes.ScrollRect;

//             terminal.textViewport = textAreaRect;
//             terminal.textComponent = textMesh;
//             terminal.fontAsset = fontAsset;
//             terminal.pointSize = 24;
//             var colorBlock = ColorBlock.defaultColorBlock;
//             var color = new Color(30.0f / 255.0f, 30.0f / 255.0f, 30.0f / 255.0f);
//             colorBlock.normalColor = color;
//             colorBlock.highlightedColor = color;
//             colorBlock.selectedColor = color;
//             colorBlock.pressedColor = color;
//             terminal.colors = colorBlock;
//             terminal.caretBlinkRate = 0;
//             terminal.customCaretColor = true;
//             terminal.caretColor = new Color(139.0f / 255.0f, 139.0f / 255.0f, 139.0f / 255.0f);
//             // terminal.caretWidth = (int)(terminal.pointSize * 0.7f) - 1;
//             terminal.selectionColor = new Color(49.0f / 255.0f, 79.0f / 255.0f, 120.0f / 255.0f);

//             terminalRect.SetParent(canvas.GetComponent<RectTransform>());
//             terminalRect.anchorMin = Vector3.zero;
//             terminalRect.anchorMax = Vector3.one;
//             terminalRect.offsetMin = Vector3.zero;
//             terminalRect.offsetMax = Vector3.zero;
//             // terminalRect.position = Vector3.zero;
//             terminalRect.pivot = new Vector2(0.5f, 0.5f);

//             textAreaRect.SetParent(terminalRect);
//             textAreaRect.anchorMin = Vector3.zero;
//             textAreaRect.anchorMax = Vector3.one;
//             textAreaRect.offsetMin = new Vector2(10, 6);
//             textAreaRect.offsetMax = new Vector2(-10, -7);

//             textRect.SetParent(textAreaRect);
//             textRect.anchorMin = Vector3.zero;
//             textRect.anchorMax = Vector3.one;
//             textRect.offsetMin = Vector2.zero;
//             textRect.offsetMax = Vector2.zero;

//             Selection.activeObject = terminalObj;

//             EditorApplication.ExecuteMenuItem("GameObject/UI/Scrollbar");

//             var scrollbarObj = Selection.activeGameObject;
//             var scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
//             var scrollbar = scrollbarObj.GetComponent<Scrollbar>();
//             var navigation = scrollbar.navigation;
//             navigation.mode = Navigation.Mode.Vertical;
//             scrollbar.direction = Scrollbar.Direction.TopToBottom;
//             scrollbar.size = 1.0f;
//             scrollbar.navigation = navigation;

//             scrollbarRect.SetParent(terminalRect);
//             scrollbarRect.anchorMin = new Vector2(1, 0);
//             scrollbarRect.anchorMax = new Vector2(1, 1);
//             scrollbarRect.offsetMin = new Vector2(-20, 0);
//             scrollbarRect.offsetMax = new Vector2(0, 0);

//             terminal.verticalScrollbar = scrollbar;
//             Selection.activeGameObject = terminalObj;
//         }
//     }
// }