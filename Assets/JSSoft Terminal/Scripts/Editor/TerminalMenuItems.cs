
using JSSoft.Communication.Shells;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI.Editor
{
    static class TerminalMeniItems
    {
        [MenuItem("Terminal/Test")]
        private static void Test2()
        {
            var rect = Selection.activeGameObject.GetComponent<RectTransform>();
            rect.offsetMax = new Vector2(rect.offsetMax.x + 1.0f, rect.offsetMax.y);
        }

        [MenuItem("GameObject/UI/Terminal")]
        private static void CreateTerminal()
        {
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            var fontAsset = (TMP_FontAsset)AssetDatabase.LoadAssetAtPath("Assets/JSSoft Terminal/Fonts/SFMono-Regular SDF.asset", typeof(TMP_FontAsset));
            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");

            var terminalObj = new GameObject("Terminal");
            var terminalRect = terminalObj.AddComponent<RectTransform>();            
            terminalObj.AddComponent<CanvasRenderer>();
            var terminalImage = terminalObj.AddComponent<Image>();
            terminalImage.sprite = backgroundSprite;
            terminalImage.type = Image.Type.Sliced;

            var terminalScript = terminalObj.AddComponent<Terminal>();
            terminalScript.targetGraphic = terminalImage;
            terminalScript.lineType = TMP_InputField.LineType.MultiLineSubmit;
            terminalScript.onFocusSelectAll = false;
            terminalScript.resetOnDeActivation = false;
            terminalScript.restoreOriginalTextOnEscape = false;
            terminalScript.richText = false;

            var textAreaObj = new GameObject("Text Area");
            var textAreaRect = textAreaObj.AddComponent<RectTransform>();
            textAreaObj.AddComponent<RectMask2D>();

            var placeholderObj = new GameObject("Placeholder");
            var placeholderRect = placeholderObj.AddComponent<RectTransform>();
            placeholderObj.AddComponent<CanvasRenderer>();
            var placeholderMesh = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderMesh.enabled = false;
            placeholderMesh.extraPadding = false;
            placeholderMesh.font = fontAsset;

            var textObj = new GameObject("Text");
            var textRect = textObj.AddComponent<RectTransform>();            
            var textMesh = textObj.AddComponent<TextMeshProUGUI>();

            terminalScript.textViewport = textAreaRect;
            terminalScript.textComponent = textMesh;
            terminalScript.fontAsset = fontAsset;
            terminalScript.pointSize = 24;
            var colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.normalColor = Color.black;
            colorBlock.highlightedColor = Color.black;
            colorBlock.selectedColor = Color.black;
            colorBlock.pressedColor = Color.black;
            terminalScript.colors = colorBlock;

            terminalRect.SetParent(canvas.GetComponent<RectTransform>());
            terminalRect.position = Vector3.zero;
            terminalRect.sizeDelta = new Vector2(800, 600);
            terminalRect.anchoredPosition = new Vector2(0, 0);
            
            textAreaRect.SetParent(terminalRect);
            textAreaRect.anchorMin = Vector3.zero;
            textAreaRect.anchorMax = Vector3.one;
            textAreaRect.offsetMin = new Vector2(10, 6);
            textAreaRect.offsetMax = new Vector2(-10, -7);

            placeholderRect.SetParent(textAreaRect);
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            textRect.SetParent(textAreaRect);
            textRect.anchorMin = Vector3.zero;
            textRect.anchorMax = Vector3.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Selection.activeGameObject = terminalObj;
        }
    }
}