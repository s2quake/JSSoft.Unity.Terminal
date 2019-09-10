
using JSSoft.Communication.Shells;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI.Editor
{
    static class TerminalMeniItems
    {
        // [MenuItem("Terminal/Test")]
        // private static void Test2()
        // {
        //     var selectedObject = Selection.activeGameObject;
        //     if (selectedObject != null)
        //     {
        //         var selectedObjectRect = selectedObject.GetComponent<RectTransform>();
        //         if (selectedObjectRect != null)
        //         {
        //             Debug.Log($"offsetMin: {selectedObjectRect.offsetMin}");
        //             Debug.Log($"offsetMax: {selectedObjectRect.offsetMax}");
        //         }
        //     }
        // }

        [MenuItem("GameObject/UI/Terminal")]
        private static void CreateTerminalUI()
        {
            CreateTerminal();
        }
        
        private static void CreateTerminal()
        {
            var width = 800.0f;
            var height = 600.0f;
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                canvas = GameObject.FindObjectOfType<Canvas>();
            }

            var fontAsset = (TMP_FontAsset)AssetDatabase.LoadAssetAtPath("Assets/JSSoft Terminal/Fonts/SFMono-Regular SDF.asset", typeof(TMP_FontAsset));
            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");

            var terminalObj = new GameObject("Terminal") { layer = canvas.gameObject.layer };
            var terminalRect = terminalObj.AddComponent<RectTransform>();
            terminalObj.AddComponent<CanvasRenderer>();
            var terminalImage = terminalObj.AddComponent<Image>();
            terminalImage.sprite = backgroundSprite;
            terminalImage.type = Image.Type.Sliced;

            var terminal = terminalObj.AddComponent<Terminal>();
            terminal.targetGraphic = terminalImage;
            terminal.lineType = TMP_InputField.LineType.MultiLineSubmit;
            terminal.onFocusSelectAll = false;
            terminal.resetOnDeActivation = false;
            terminal.restoreOriginalTextOnEscape = false;
            terminal.richText = false;

            var textAreaObj = new GameObject("Text Area") { layer = canvas.gameObject.layer };
            var textAreaRect = textAreaObj.AddComponent<RectTransform>();
            textAreaObj.AddComponent<RectMask2D>();

            // var placeholderObj = new GameObject("Placeholder") { layer = canvas.gameObject.layer };
            // var placeholderRect = placeholderObj.AddComponent<RectTransform>();
            // placeholderObj.AddComponent<CanvasRenderer>();
            // var placeholderMesh = placeholderObj.AddComponent<TextMeshProUGUI>();
            // placeholderMesh.enabled = false;
            // placeholderMesh.extraPadding = false;
            // placeholderMesh.font = fontAsset;

            var textObj = new GameObject("Text") { layer = canvas.gameObject.layer };
            var textRect = textObj.AddComponent<RectTransform>();
            var textMesh = textObj.AddComponent<TerminalText>();
            textMesh.terminal = terminal;
            textMesh.richText = false;
            // textMesh.geometrySortingOrder = VertexSortingOrder.Reverse;
            textMesh.overflowMode = TextOverflowModes.ScrollRect;

            terminal.textViewport = textAreaRect;
            terminal.textComponent = textMesh;
            terminal.fontAsset = fontAsset;
            terminal.pointSize = 24;
            var colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.normalColor = Color.black;
            colorBlock.highlightedColor = Color.black;
            colorBlock.selectedColor = Color.black;
            colorBlock.pressedColor = Color.black;
            terminal.colors = colorBlock;
            terminal.caretBlinkRate = 0;
            terminal.customCaretColor = true;
            terminal.caretColor = new Color(0.56862745098f, 0.56862745098f, 0.56862745098f);
            terminal.caretWidth = (int)(terminal.pointSize * 0.7f) - 1;

            terminalRect.SetParent(canvas.GetComponent<RectTransform>());
            terminalRect.position = Vector3.zero;
            terminalRect.sizeDelta = new Vector2(width, height);
            terminalRect.anchoredPosition = new Vector2(0, 0);

            textAreaRect.SetParent(terminalRect);
            textAreaRect.anchorMin = Vector3.zero;
            textAreaRect.anchorMax = Vector3.one;
            textAreaRect.offsetMin = new Vector2(10, 6);
            textAreaRect.offsetMax = new Vector2(-10, -7);

            // placeholderRect.SetParent(textAreaRect);
            // placeholderRect.anchorMin = Vector2.zero;
            // placeholderRect.anchorMax = Vector2.one;
            // placeholderRect.offsetMin = Vector2.zero;
            // placeholderRect.offsetMax = Vector2.zero;

            textRect.SetParent(textAreaRect);
            textRect.anchorMin = Vector3.zero;
            textRect.anchorMax = Vector3.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Selection.activeObject = terminalObj;

            EditorApplication.ExecuteMenuItem("GameObject/UI/Scrollbar");

            var scrollbarObj = Selection.activeGameObject;
            var scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
            var scrollbar = scrollbarObj.GetComponent<Scrollbar>();
            var navigation = scrollbar.navigation;
            navigation.mode = Navigation.Mode.Vertical;
            scrollbar.direction = Scrollbar.Direction.TopToBottom;
            scrollbar.size = 1.0f;
            scrollbar.navigation = navigation;

            scrollbarRect.SetParent(terminalRect);
            scrollbarRect.anchorMin = new Vector2(1, 0.5f);
            scrollbarRect.anchorMax = new Vector2(1, 0.5f);
            scrollbarRect.offsetMax = new Vector2(0, height / 2);
            scrollbarRect.offsetMin = new Vector2(-20, -height / 2);

            terminal.verticalScrollbar = scrollbar;
            Selection.activeGameObject = terminalObj;
        }
    }
}