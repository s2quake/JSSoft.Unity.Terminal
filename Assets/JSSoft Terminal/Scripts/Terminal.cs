using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ntreev.Library.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.UI
{
    [AddComponentMenu("UI/Terminal", 15)]
    public class Terminal : TMP_InputField, ITerminal
    {
        private static readonly Dictionary<string, IKeyBinding> bindingByKey = new Dictionary<string, IKeyBinding>();
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();

        private string promptText = string.Empty;
        private string outputText = string.Empty;
        private string prompt = string.Empty;
        private string inputText = string.Empty;
        private string commandText = string.Empty;
        private string completion;
        private int historyIndex;
        private bool isReadOnly;
        private bool isChanged;
        private int cursorPosition;
        private Color32?[] foregroundColors = new Color32?[] { };
        private Color32?[] backgroundColors = new Color32?[] { };
        private Color32?[] promptForegroundColors = new Color32?[] { };
        private Color32?[] promptBackgroundColors = new Color32?[] { };

        private Event processingEvent = new Event();
        private ExecutedEvent executedEvent = new ExecutedEvent();

        public ExecutedEvent onExecuted
        {
            get => executedEvent;
            set => executedEvent = value;
        }

        public OnCompletion onCompletion { get; set; }

        public OnDrawPrompt onDrawPrompt { get; set; }

        static Terminal()
        {
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.UpArrow,
                            (t) => t.PrevHistory(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.DownArrow,
                            (t) => t.NextHistory(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.LeftArrow,
                            (t) => t.CursorPosition--));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.RightArrow,
                            (t) => t.CursorPosition++));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Return,
                            (t) => t.Execute(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.KeypadEnter,
                            (t) => t.Execute(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.Backspace,
                            (t) => true, (t) => t.caretPosition <= t.outputText.Length + t.prompt.Length));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Tab,
                            (t) => t.NextCompletion(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.Shift, KeyCode.Tab,
                            (t) => t.PrevCompletion(), (t) => t.isReadOnly == false));
            if (IsMac == true)
            {
                AddBinding(new KeyBinding(EventModifiers.Control, KeyCode.U,
                            (t) => t.Clear(), (t) => t.isReadOnly == false));
                AddBinding(new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.LeftArrow,
                            (t) => t.MoveToFirst()));
                AddBinding(new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.RightArrow,
                            (t) => t.MoveToLast()));
            }
            if (IsWindows == true)
            {
                AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Escape,
                            (t) => t.Clear(), (t) => t.isReadOnly == false));
                AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.Home,
                            (t) => t.MoveToFirst()));
                AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.End,
                            (t) => t.MoveToLast()));
            }
        }

        public void Execute()
        {
            var commandText = this.commandText;
            var promptText = this.promptText;
            if (this.histories.Contains(commandText) == false)
            {
                this.histories.Add(commandText);
                this.historyIndex = this.histories.Count;
            }
            else
            {
                this.historyIndex = this.histories.LastIndexOf(commandText) + 1;
            }

            this.promptText = string.Empty;
            this.inputText = string.Empty;
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.AppendLine(promptText);

            if (this.executedEvent != null)
            {
                this.readOnly = true;
                this.executedEvent.Invoke(commandText);
            }
            else
            {
                this.InsertPrompt();
            }
        }

        public void Clear()
        {
            this.inputText = string.Empty;
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.prompt;
            this.text = this.outputText + this.promptText;
            this.caretPosition = this.text.Length;
        }

        public void MoveToFirst()
        {
            this.caretPosition = this.outputText.Length + this.prompt.Length;
            this.UpdateLabel();
        }

        public void MoveToLast()
        {
            this.caretPosition = this.text.Length;
            this.UpdateLabel();
        }

        public void ResetOutput()
        {
            this.outputText = string.Empty;
            this.inputText = string.Empty;
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.prompt;
            this.foregroundColors = new Color32?[] { };
            this.backgroundColors = new Color32?[] { };
            this.text = this.outputText + this.promptText;
            this.caretPosition = this.text.Length;
        }

        public new void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        public string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.prompt = value;
                this.promptText = this.prompt + this.commandText;
                this.promptForegroundColors = new Color32?[this.prompt.Length];
                this.promptBackgroundColors = new Color32?[this.prompt.Length];
                if (this.onDrawPrompt != null)
                {
                    this.onDrawPrompt(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
                }
                this.text = this.outputText + this.promptText;
                if (this.isReadOnly == false)
                {
                    this.caretPosition = this.text.Length;
                }
            }
        }

        public void NextCompletion()
        {
            this.CompletionImpl(NextCompletion);
            this.caretPosition = this.text.Length;
        }

        public void PrevCompletion()
        {
            this.CompletionImpl(PrevCompletion);
            this.caretPosition = this.text.Length;
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.commandText = this.histories[this.historyIndex + 1];
                this.promptText = this.prompt + this.inputText;
                this.text = this.outputText + this.promptText;
                this.MoveToLast();
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.commandText = this.histories[this.historyIndex - 1];
                this.promptText = this.prompt + this.inputText;
                this.text = this.outputText + this.promptText;
                this.MoveToLast();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.commandText = this.histories[0];
                this.promptText = this.prompt + this.inputText;
                this.text = this.outputText + this.promptText;
                this.MoveToLast();
                this.historyIndex = 0;
            }
        }

        public void InsertPrompt()
        {
            this.promptText = string.Empty;
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.Prompt;
            this.text = this.outputText + this.prompt;
            this.caretPosition = this.text.Length;
            this.readOnly = false;
        }

        public static Match[] MatchCompletion(string text)
        {
            var matches = Regex.Matches(text, "\\S+");
            var argList = new List<Match>(matches.Count);
            foreach (Match item in matches)
            {
                argList.Add(item);
            }
            return argList.ToArray();
        }

        public static string NextCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i + 1 < completions.Length)
                            return completions[i + 1];
                        else
                            return completions.First();
                    }
                }
            }
            else
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public static string PrevCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i - 1 >= 0)
                            return completions[i - 1];
                        else
                            return completions.Last();
                    }
                }
            }
            else
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!isFocused)
                return;

            var consumedEvent = false;
            var promptText = this.promptText;
            while (Event.PopEvent(this.processingEvent))
            {
                if (this.processingEvent.rawType == EventType.KeyDown)
                {
                    var keyCode = this.processingEvent.keyCode;
                    var modifiers = this.processingEvent.modifiers;
                    var key = $"{modifiers}+{keyCode}";
                    if (this.OnPreviewKeyDown(this.processingEvent) == true)
                    {
                        // Debug.Log($"{key}: true");
                        continue;
                    }
                    consumedEvent = true;
                    // Debug.Log($"{key}: false");
                    var shouldContinue = KeyPressed(this.processingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        //SendOnSubmit();
                        //DeactivateInputField();
                        break;
                    }
                }

                switch (this.processingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (this.processingEvent.commandName)
                        {
                            case "SelectAll":
                                SelectAll();
                                consumedEvent = true;
                                break;
                        }
                        break;
                }
            }

            if (consumedEvent)
            {
                this.promptText = this.text.Substring(this.outputText.Length);
                if (this.promptText != string.Empty)
                {
                    this.inputText = this.commandText = this.promptText.Substring(this.prompt.Length);
                }
                this.completion = string.Empty;
                // Debug.Log($"InputText: \"{inputText}\"");
                // Debug.Log($"update: {this.caretPosition}");
                this.isReadOnly = this.caretPosition < this.outputText.Length + this.prompt.Length;
                this.cursorPosition = base.stringPositionInternal - (this.outputText.Length + this.prompt.Length);
                this.UpdateLabel();
            }
            eventData.Use();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            // var index = this.caretPosition;
            base.OnPointerDown(eventData);
            // if (this.caretPosition != index)
            // {
            //     this.caretPositionInternal = index + 1;
            //     this.caretSelectPositionInternal = index;
            // }

            // int insertionIndex = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, eventData.position, eventData.pressEventCamera, out CaretPosition insertionSide);
            // if (insertionIndex )
        }

        public override void OnDrag(PointerEventData eventData)
        {
            var index = this.caretPosition;
            base.OnDrag(eventData);
            if (this.caretPosition != index)
            {
                this.caretPositionInternal = index + 1;
                this.caretSelectPositionInternal = index;
                Debug.Log(index);
            }
        }

        public void ResetColor()
        {
            this.ForegroundColor = null;
            this.BackgroundColor = null;
        }

        public Color32? ForegroundColor { get; set; }

        public Color32? BackgroundColor { get; set; }


        public int CursorPosition
        {
            get => this.cursorPosition;
            set
            {
                this.cursorPosition = value;
                if (this.cursorPosition < 0)
                    this.cursorPosition = 0;
                if (this.cursorPosition > this.promptText.Length - this.prompt.Length)
                    this.cursorPosition = this.promptText.Length - this.prompt.Length;
                this.caretPosition = this.outputText.Length + this.prompt.Length + this.cursorPosition;
                this.UpdateLabel();
            }
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            if (this.onCompletion != null)
            {
                return this.onCompletion(items, find);
            }
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        private CanvasRenderer cursorRenderer;
        private Mesh cursorMesh;

        private Mesh CursorMesh
        {
            get
            {
                if (this.cursorMesh == null)
                {
                    this.cursorMesh = new Mesh();
                }
                return this.cursorMesh;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                var textAreaRect = m_TextComponent.transform.parent;
                var selectionCaret = textAreaRect.GetComponentInChildren(typeof(TMP_SelectionCaret), true) as TMP_SelectionCaret;
                if (selectionCaret != null)
                {
                    selectionCaret.transform.SetAsLastSibling();
                    //selectionCaret.rectTransform.SetAsLastSibling();
                    Debug.Log("1");
                }
                var cursorObj = new GameObject("TerminalCursor", typeof(RectTransform));
                cursorObj.layer = this.gameObject.layer;
                var cursorRect = cursorObj.GetComponent<RectTransform>();
                cursorObj.transform.parent = m_TextComponent.transform.parent;

                this.cursorRenderer = cursorObj.AddComponent<CanvasRenderer>();
                this.cursorRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);

                cursorRect.localPosition = m_TextComponent.rectTransform.localPosition;
                cursorRect.localRotation = m_TextComponent.rectTransform.localRotation;
                cursorRect.localScale = m_TextComponent.rectTransform.localScale;
                cursorRect.anchorMin = m_TextComponent.rectTransform.anchorMin;
                cursorRect.anchorMax = m_TextComponent.rectTransform.anchorMax;
                cursorRect.anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
                cursorRect.sizeDelta = m_TextComponent.rectTransform.sizeDelta;
                cursorRect.pivot = m_TextComponent.rectTransform.pivot;
            }
            // TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        // private void ON_TEXT_CHANGED(UnityEngine.Object obj)
        // {

        // }

        public override void Rebuild(CanvasUpdate update)
        {
            base.Rebuild(update);
            switch (update)
            {
                case CanvasUpdate.LatePreRender:
                    UpdateCursorGeometry();
                    break;
            }
        }

        private void UpdateCursorGeometry()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif


            var m_CursorVerts = new UIVertex[4];

            for (int i = 0; i < m_CursorVerts.Length; i++)
            {
                m_CursorVerts[i] = UIVertex.simpleVert;
                m_CursorVerts[i].uv0 = Vector2.zero;
            }

            float width = 15;

            using (VertexHelper vbo = new VertexHelper())
            {
                // TODO: Optimize to only update the caret position when needed.

                Vector2 startPosition = Vector2.zero;
                float height = 0;
                TMP_CharacterInfo currentCharacter;

                var index = this.outputText.Length + this.prompt.Length + this.cursorPosition;
                int currentLine = m_TextComponent.textInfo.characterInfo[index].lineNumber;

                // Caret is positioned at the origin for the first character of each lines and at the advance for subsequent characters.
                if (index == m_TextComponent.textInfo.lineInfo[currentLine].firstCharacterIndex)
                {
                    currentCharacter = m_TextComponent.textInfo.characterInfo[index];
                    startPosition = new Vector2(currentCharacter.origin, currentCharacter.descender);
                    height = currentCharacter.ascender - currentCharacter.descender;
                }
                else
                {
                    currentCharacter = m_TextComponent.textInfo.characterInfo[index - 1];
                    startPosition = new Vector2(currentCharacter.xAdvance, currentCharacter.descender);
                    height = currentCharacter.ascender - currentCharacter.descender;
                }

                // if (m_SoftKeyboard != null)
                //     m_SoftKeyboard.selection = new RangeInt(stringPositionInternal, 0);

                // Adjust the position of the RectTransform based on the caret position in the viewport (only if we have focus).
                // if (isFocused && startPosition != m_LastPosition || m_forceRectTransformAdjustment)
                //     AdjustRectTransformRelativeToViewport(startPosition, height, currentCharacter.isVisible);

                // m_LastPosition = startPosition;

                // Clamp Caret height
                float top = startPosition.y + height;
                float bottom = top - height;

                // Minor tweak to address caret potentially being too thin based on canvas scaler values.
                float scale = m_TextComponent.canvas.scaleFactor;

                m_CursorVerts[0].position = new Vector3(startPosition.x, bottom, 0.0f);
                m_CursorVerts[1].position = new Vector3(startPosition.x, top, 0.0f);
                m_CursorVerts[2].position = new Vector3(startPosition.x + (width + 1) / scale, top, 0.0f);
                m_CursorVerts[3].position = new Vector3(startPosition.x + (width + 1) / scale, bottom, 0.0f);

                // Set Vertex Color for the caret color.
                m_CursorVerts[0].color = TerminalColors.Red;
                m_CursorVerts[1].color = TerminalColors.Red;
                m_CursorVerts[2].color = TerminalColors.Red;
                m_CursorVerts[3].color = TerminalColors.Red;

                vbo.AddUIVertexQuad(m_CursorVerts);

                int screenHeight = Screen.height;
                // Removed multiple display support until it supports none native resolutions(case 741751)
                //int displayIndex = m_TextComponent.canvas.targetDisplay;
                //if (Screen.fullScreen && displayIndex < Display.displays.Length)
                //    screenHeight = Display.displays[displayIndex].renderingHeight;

                startPosition.y = screenHeight - startPosition.y;
                vbo.FillMesh(this.CursorMesh);
                this.cursorRenderer.SetMesh(this.CursorMesh);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.onTextSelection = new TMP_InputField.TextSelectionEvent();
            this.onTextSelection.AddListener(Terminal_onTextSelection);
            this.onEndTextSelection = new TMP_InputField.TextSelectionEvent();
            this.onEndTextSelection.AddListener(Terminal_onEndTextSelection);
        }

        private void Terminal_onTextSelection(string a, int i1, int i2)
        {
            // Debug.Log($"text {i1}, {i2}");
        }

        private void Terminal_onEndTextSelection(string a, int i1, int i2)
        {
            // Debug.Log($"end {i1}, {i2}");
        }

        protected virtual bool OnPreviewKeyDown(Event e)
        {
            var keyCode = e.keyCode;
            var modifiers = e.modifiers;
            var key = $"{modifiers}+{keyCode}";
            if (bindingByKey.ContainsKey(key) == true)
            {
                var binding = bindingByKey[key];
                if (binding.Verify(this) == true && binding.Action(this) == true)
                    return true;
            }
            // Debug.Log($"{modifiers}+{keyCode}");
            if (e.character == '\n' || e.character == '\t' || e.character == 25)
            {
                return true;
            }
            return false;
        }

        protected override bool IsValidChar(char c)
        {
            // if (this.isReadOnly == true)
            //     return false;
            // if (base.caretPosition < this.outputText.Length + this.prompt.Length)
            //     return false;
            var r = base.IsValidChar(c);
            if (r == true && c != 0)
            {
                this.caretPosition = this.outputText.Length + this.prompt.Length + this.cursorPosition;
                // Debug.Log(this.caretPosition);
            }
            return r;
        }

        private void CompletionImpl(Func<string[], string, string> func)
        {
            var matches = new List<Match>(CommandStringUtility.MatchCompletion(this.inputText));
            var find = string.Empty;
            var prefix = false;
            var postfix = false;
            var leftText = this.inputText;
            if (matches.Count > 0)
            {
                var match = matches.Last();
                var matchText = match.Value;
                if (matchText.Length > 0 && matchText.First() == '\"')
                {
                    prefix = true;
                    matchText = matchText.Substring(1);
                }
                if (matchText.Length > 1 && matchText.Last() == '\"')
                {
                    postfix = true;
                    matchText = matchText.Remove(matchText.Length - 1);
                }
                if (matchText == string.Empty || matchText.Trim() != string.Empty)
                {
                    find = matchText;
                    matches.RemoveAt(matches.Count - 1);
                    leftText = this.inputText.Remove(match.Index);
                }
            }

            var argList = new List<string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var matchText = CommandStringUtility.TrimQuot(matches[i].Value).Trim();
                if (matchText != string.Empty)
                    argList.Add(matchText);
            }

            var completions = this.GetCompletion(argList.ToArray(), find);
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                var inputText = this.inputText;
                if (prefix == true || postfix == true)
                {
                    this.commandText = leftText + "\"" + this.completion + "\"";
                }
                else
                {
                    this.commandText = leftText + this.completion;
                }
                this.promptText = this.prompt + this.commandText;
                this.inputText = inputText;
                this.text = this.outputText + this.promptText;
                this.caretPosition = this.text.Length;
            }
        }

        private void AppendInternal(string text)
        {
            var index = this.outputText.Length;
            this.outputText += text;
            Array.Resize(ref this.foregroundColors, this.outputText.Length);
            Array.Resize(ref this.backgroundColors, this.outputText.Length);
            for (var i = index; i < this.outputText.Length; i++)
            {
                this.foregroundColors[i] = this.ForegroundColor;
                this.backgroundColors[i] = this.BackgroundColor;
            }
            this.text = this.outputText + this.promptText;
        }

        private static void AddBinding(IKeyBinding binding)
        {
            bindingByKey.Add($"{binding.Modifiers}+{binding.KeyCode}", binding);
        }

        private static bool IsMac => (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);

        private static bool IsWindows => (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);

        internal Color32? GetForegroundColor(int index)
        {
            if (index < this.text.Length && this.text[index] == ' ')
            {
                return TerminalColors.Transparent;
            }
            if (index < this.foregroundColors.Length)
            {
                return this.foregroundColors[index];
            }
            var promptIndex = index - this.outputText.Length;
            if (promptIndex >= 0 && promptIndex < this.promptForegroundColors.Length)
            {
                return this.promptForegroundColors[promptIndex];
            }
            return null;
        }

        internal Color32? GetBackgroundColor(int index)
        {
            // if (index % 2 != 0)
            //     return TerminalColors.Blue;
            if (index < this.backgroundColors.Length)
            {
                return this.backgroundColors[index];
            }
            var promptIndex = index - this.outputText.Length;
            if (promptIndex >= 0 && promptIndex < this.promptBackgroundColors.Length)
            {
                return this.promptBackgroundColors[promptIndex];
            }
            return null;
        }

        public class ExecutedEvent : UnityEvent<string>
        {

        }

        #region ITerminal

        void ITerminal.Reset() => this.ResetOutput();

        string ITerminal.Command => this.commandText;

        string ITerminal.Prompt
        {
            get => this.Prompt;
            set => this.Prompt = value;
        }

        // event EventHandler ITerminal.Executed
        // {
        //     add {}
        //     remove {}
        // }

        #endregion
    }
}
