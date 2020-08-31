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

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace JSSoft.Terminal.InputHandlers
{
    class EditorKeyboard : TerminalKeyboardBase
    {
        private bool? result;
        private RectTransform panelRect;
        private Button doneButton;
        private Button cancelButton;

        public override string Text
        {
            get => this.Terminal.Command;
            set
            {
            }
        }

        public override RangeInt Selection
        {
            get => new RangeInt(this.Terminal.CursorPosition, 0);
            set
            {
            }
        }

        public override Rect Area
        {
            get
            {
                if (this.panelRect != null)
                    return new Rect(0, Screen.height - this.panelRect.sizeDelta.y, Screen.width, this.panelRect.sizeDelta.y);
                return default(Rect);
            }
        }

        protected override void OnOpen(string text)
        {
            var gameObject = this.Grid.GameObject;
            var canvas = gameObject.GetComponentInParent<Canvas>();
            var canvasRect = canvas.GetComponent<RectTransform>();

            var panelRect = CreatePanel(canvasRect);
            var doneRect = CreateButton(panelRect, "Button_Done", "Done", new Vector2(-150, 0), new Vector2(250, 60));
            var cancelRect = CreateButton(panelRect, "Button_Cancel", "Cancel", new Vector2(150, 0), new Vector2(250, 60));
            var doneButton = doneRect.GetComponent<Button>();
            var cancelButton = cancelRect.GetComponent<Button>();
            doneButton.onClick = new Button.ButtonClickedEvent();
            doneButton.onClick.AddListener(this.OnDoneClicked);
            cancelButton.onClick = new Button.ButtonClickedEvent();
            cancelButton.onClick.AddListener(this.OnCancelClicked);

            this.doneButton = doneButton;
            this.cancelButton = cancelButton;
            this.panelRect = panelRect;
            this.result = null;
        }

        protected override void OnClose()
        {
            this.result = null;
            this.Release();
        }

        protected override bool? OnUpdate()
        {
            if (Input.GetKey(KeyCode.Return) == true || Input.GetKey(KeyCode.Escape) == true)
            {
                this.result = false;
                this.Release();
                this.Grid.Focus();
            }
            return this.result;
        }

        private void OnDoneClicked()
        {
            this.result = true;
            this.Release();
            this.Grid.Focus();
        }

        private void OnCancelClicked()
        {
            this.result = false;
            this.Release();
            this.Grid.Focus();
        }

        private void Release()
        {
            this.doneButton.onClick.RemoveAllListeners();
            this.cancelButton.onClick.RemoveAllListeners();
            GameObject.Destroy(this.panelRect.gameObject);
            this.panelRect = null;
            this.doneButton = null;
            this.cancelButton = null;
        }

        private static RectTransform CreatePanel(RectTransform canvasRect)
        {
            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            var panelObject = new GameObject("Keyboard Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var panelRect = panelObject.GetComponent<RectTransform>();
            var panelImage = panelObject.GetComponent<Image>();
            panelImage.sprite = backgroundSprite;
            panelImage.type = Image.Type.Sliced;
            panelRect.SetParent(canvasRect);
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = new Vector2(1, 0);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = new Vector2(0, 205);
            panelRect.sizeDelta = new Vector2(0, 410);
            return panelRect;
        }

        private static RectTransform CreateButton(RectTransform panelRect, string name, string text, Vector2 pos, Vector2 size)
        {
            var backgroundSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            var buttonRect = buttonObject.GetComponent<RectTransform>();
            var buttonImage = buttonObject.GetComponent<Image>();
            var button = buttonObject.GetComponent<Button>();
            var textRect = CreateText(buttonRect, text);
            buttonImage.sprite = backgroundSprite;
            buttonImage.type = Image.Type.Sliced;
            button.targetGraphic = buttonImage;
            buttonRect.SetParent(panelRect);
            buttonRect.anchoredPosition = pos;
            buttonRect.sizeDelta = size;
            return buttonRect;
        }

        private static RectTransform CreateText(RectTransform buttonRect, string text)
        {
            var font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            var textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            var textRect = textObject.GetComponent<RectTransform>();
            var textComponent = textObject.GetComponent<Text>();
            textComponent.font = font;
            textComponent.text = text;
            textComponent.color = new Color32(50, 50, 50, 255);
            textComponent.fontSize = 20;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textRect.SetParent(buttonRect);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            return textRect;
        }
    }
}
#endif // UNITY_EDITOR
