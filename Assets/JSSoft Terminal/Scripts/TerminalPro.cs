using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.UI
{
    [AddComponentMenu("UI/TerminalPro", 15)]
    public class TerminalPro : TMP_InputField
    {
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();
        private int historyIndex;

        private string promptText;
        private string outputText;
        private string prompt = string.Empty;
        private string inputText = string.Empty;
        private int refStack = 0;
        private string completion;
        private bool isReadOnly;
        private string oldText = string.Empty;
        private int oldPosition;

        private bool isChanged;

        public event EventHandler Executed;

        protected override void Start()
        {
            base.Start();
            this.promptText = string.Empty;
            // this.Document.Blocks.Add(this.promptBlock);
            // this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
            // this.CaretPosition = this.promptBlock.ContentEnd;
            if (this.TextInternal.Length != 0 && this.TextInternal.EndsWith(Environment.NewLine) == false)
            {
                this.TextInternal += Environment.NewLine;
            }
            this.outputText = this.TextInternal;

            // this.text = string.Empty;
            // this.Prompt = "c:>";
            // this.onValueChanged = new TMP_InputField.OnChangeEvent();
            // this.onValueChanged.AddListener(TextBox_TextChanged);
            // this.onEndEdit = new TMP_InputField.SubmitEvent();
            // this.onEndEdit.AddListener(OnEndEdit);
            // this.onEndTextSelection = new TMP_InputField.TextSelectionEvent();
            // this.onEndTextSelection.AddListener(OnEndTextSelection);
            // this.onValidateInput = OnTextValidateInput;
            this.caretPosition = this.text.Length;
            // this.ActivateInputField();
        }

        private string TextInternal
        {
            get => this.text;
            set => this.text = value;
        }

        private void OnEndTextSelection(string arg0, int arg1, int arg2)
        {
            this.OnCaretPositionChanged();
        }

        private void OnEndEdit(string text)
        {
            // if (this.isReadOnly == true)
            // {
            //     this.caretPosition = this.TextInternal.Length;
            // }
            // else
            // {
            //     //this.Execute();
            // }
        }

        public char OnTextValidateInput(string text, int charIndex, char addedChar)
        {
            if (addedChar != '\0')
            {
                this.oldText = this.text;
                this.oldPosition = this.caretPosition;
            }
            return addedChar;
        }
        public void Execute()
        {
            this.refStack++;
            try
            {
                var commandText = this.CommandText;
                //var args = new RoutedEventArgs(ExecutedEvent);
                this.Text = commandText;
                if (this.histories.Contains(this.Text) == false)
                {
                    this.histories.Add(this.Text);
                    this.historyIndex = this.histories.Count;
                }
                else
                {
                    this.historyIndex = this.histories.LastIndexOf(this.Text) + 1;
                }

                this.promptText = string.Empty;
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.AppendLine(this.Prompt + commandText);

                this.readOnly = true;

                this.OnExecuted(EventArgs.Empty);
                this.readOnly = false;

                //if (args.Handled == false)
                {
                    this.promptText = this.Prompt;
                    this.text = this.outputText + this.Prompt;
                    this.readOnly = false;
                    this.caretPosition = this.TextInternal.Length;
                    this.oldPosition = this.caretPosition;
                    this.oldText = this.TextInternal;
                }
            }
            finally
            {
                this.refStack--;
            }
        }

        public void Clear()
        {
            var text = this.text ?? string.Empty;
            var promptText = this.prompt + this.inputText;
            text = text.Substring(0, text.Length - promptText.Length);
            //this.promptText = this.Prompt;
            this.text = text + this.prompt;
            this.inputText = string.Empty;
            this.completion = string.Empty;
        }

        public void MoveToFirst()
        {
            this.caretPosition = this.text.Length - this.promptText.Length;
        }

        public void MoveToLast()
        {
            this.caretPosition = this.text.Length;
        }

        // public void Reset()
        // {
        //     this.refStack++;
        //     try
        //     {
        //         // this.output = null;
        //         // this.outputBlock = null;
        //         // this.outputLeftText = string.Empty;
        //         // this.Document.Blocks.Clear();
        //         // this.promptBlock = new Paragraph();
        //         // this.Document.Blocks.Add(this.promptBlock);
        //         // this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
        //         // this.CaretPosition = this.promptBlock.Inlines.LastInline.ContentEnd;
        //     }
        //     finally
        //     {
        //         this.refStack--;
        //     }
        // }

        public new void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        public string Text { get; private set; }

        public string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                var diff = value.Length - this.prompt.Length;
                this.prompt = value;
                this.promptText = this.prompt + this.inputText;
                this.text = this.oldText = this.outputText + this.prompt + this.inputText;
                if (this.isReadOnly == false)
                {
                    this.oldPosition += diff;
                    this.caretPosition += diff;
                }

                // this.OnCaretPositionChanged();
                //this.RefreshPrompt();
            }
        }

        // public Brush OutputForeground
        // {
        //     get { return (Brush)this.GetValue(OutputForegroundProperty); }
        //     set { this.SetValue(OutputForegroundProperty, value); }
        // }

        // public Brush OutputBackground
        // {
        //     get { return (Brush)this.GetValue(OutputBackgroundProperty); }
        //     set { this.SetValue(OutputBackgroundProperty, value); }
        // }

        public void NextCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(NextCompletion);
                this.caretPosition = this.text.Length;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void PrevCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(PrevCompletion);
                this.caretPosition = this.text.Length;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.CommandText = this.histories[this.historyIndex + 1];
                this.MoveToLast();
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.CommandText = this.histories[this.historyIndex - 1];
                this.MoveToLast();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.CommandText = this.histories[0];
                this.MoveToLast();
                this.historyIndex = 0;
            }
        }

        public void InsertPrompt()
        {
            this.refStack++;
            try
            {
                // var textRange = new TextRange(this.promptBlock.ContentStart, this.promptBlock.ContentEnd);
                var isEnd = this.caretPosition == this.text.Length;
                // if (textRange.Text != string.Empty)
                //     this.AppendLine(textRange.Text);
                this.promptText = string.Empty;
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.promptText = this.Prompt;
                if (isEnd == true)
                    this.caretPosition = this.text.Length;
                this.readOnly = false;
            }
            finally
            {
                this.refStack--;
            }
        }

        public static Match[] MatchCompletion(string text)
        {
            var matches = Regex.Matches(text, "\\S+");
            var argList = new List<Match>();

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

        // protected override void OnInitialized(EventArgs e)
        // {
        //     base.OnInitialized(e);
        // }

        // protected override void OnGotFocus(RoutedEventArgs e)
        // {
        //     base.OnGotFocus(e);
        //     this.textBox?.Focus();
        // }

        protected virtual void OnExecuted(EventArgs e)
        {
            this.Executed?.Invoke(this, e);
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        // protected virtual Inline[] GetPrompt(string prompt)
        // {
        //     return new Run[]
        //     {
        //         new Run(){ Text = this.Prompt, },
        //     };
        // }

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
                    // this.promptBlock.Inlines.Clear();
                    // this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                    // this.promptBlock.Inlines.Add(new Run() { Text = leftText + "\"" + this.completion + "\"" });
                }
                else
                {
                    // this.promptBlock.Inlines.Clear();
                    // this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                    // this.promptBlock.Inlines.Add(new Run() { Text = leftText + this.completion, });
                }
                this.inputText = inputText;
            }
        }

        // private static void PromptPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        // {
        //     if (d is TerminalControl self)
        //     {
        //         self.RefreshPrompt();
        //     }
        // }

        // private static object PromptPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        // {
        //     return baseValue ?? string.Empty;
        // }

        // private static void OutputForegroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        // {
        //     if (d is TerminalControl self && self.textBox != null)
        //     {
        //         self.isChanged = true;
        //     }
        // }

        // private static void OutputBackgroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        // {
        //     if (d is TerminalControl self && self.textBox != null)
        //     {
        //         self.isChanged = true;
        //     }
        // }

        private void TextBox_TextChanged(string arg0)
        {
            if (this.refStack > 0)
                return;

            this.promptText = this.text.Substring(this.outputText.Length);
            this.inputText = this.promptText.Substring(this.prompt.Length);
            this.completion = string.Empty;
        }

        private void Update()
        {
            return;
            if (Input.GetKeyDown(KeyCode.Home))
            {
                //if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    //e.Handled = true;
                    this.MoveToFirst();
                }
                // else if (Keyboard.Modifiers == ModifierKeys.Shift)
                // {
                //     e.Handled = true;
                //     var commandStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
                //     this.Selection.Select(commandStart, this.Selection.Start);
                // }
            }
            // else if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            // {
            //     if (this.IsReadOnly == true)
            //     {
            //         this.MoveToFirst();
            //         e.Handled = true;
            //     }
            // }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (this.isReadOnly == false)
                {
                    try
                    {
                        this.Execute();
                    }
                    finally
                    {
                        //e.Handled = true;
                        this.ActivateInputField();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                this.text = this.oldText;
                this.caretPosition = this.oldPosition;

                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    //e.Handled = true;
                    this.NextCompletion();
                }
                else if (Input.GetKey(KeyCode.LeftShift) == true)
                {
                    //e.Handled = true;
                    this.PrevCompletion();
                }
            }
            // else if (e.Key == Key.Back && Keyboard.Modifiers == ModifierKeys.None)
            // {
            //     var commandStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
            //     if (this.CaretPosition.CompareTo(commandStart) <= 0)
            //     {
            //         e.Handled = true;
            //     }
            // }
            // else if (e.Key == Key.Up && Keyboard.Modifiers == ModifierKeys.None)
            // {
            //     if (this.IsReadOnly == false)
            //     {
            //         this.PrevHistory();
            //         e.Handled = true;
            //     }
            // }
            // else if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.None)
            // {
            //     if (this.IsReadOnly == false)
            //     {
            //         this.NextHistory();
            //         e.Handled = true;
            //     }
            // }
            else if (Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.DownArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.OnCaretPositionChanged();
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (this.oldPosition <= this.outputText.Length + this.prompt.Length)
                {
                    this.text = this.oldText;
                    this.caretPosition = this.oldPosition;
                }
                else
                {
                    this.oldText = this.text;
                    this.oldPosition = this.caretPosition;
                    this.inputText = this.text.Substring(this.outputText.Length + this.prompt.Length);
                }
            }

        }

        private void OnCaretPositionChanged()
        {
            var inputPosition = this.outputText.Length + this.prompt.Length;
            this.isReadOnly = this.caretPosition < inputPosition;
            if (this.isReadOnly == true)
            {
                this.caretPosition = inputPosition;
            }
            else
            {
                this.oldPosition = this.caretPosition;
            }
        }

        // private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        // {
        //     if (this.promptBlock.Inlines.FirstInline != null)
        //     {
        //         var commandStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
        //         if (commandStart != null)
        //         {
        //             if (this.Selection.Start.CompareTo(commandStart) < 0 || this.Selection.End.CompareTo(commandStart) < 0)
        //             {
        //                 this.IsReadOnly = true;
        //             }
        //             else
        //             {
        //                 this.IsReadOnly = false;
        //             }
        //         }
        //         else
        //         {
        //             this.IsReadOnly = false;
        //         }
        //     }
        // }

        private void RefreshPrompt()
        {
            this.refStack++;
            this.Clear();
            this.refStack--;
        }

        private void AppendInternal(string text)
        {
            this.refStack++;
            try
            {
                var isEnd = this.caretPosition == this.TextInternal.Length;
                // if (this.output == null || this.isChanged == true)
                // {
                //     var oldOutput = this.output;
                //     this.output = new Run();
                //     if (this.OutputForeground != null)
                //         this.output.Foreground = this.OutputForeground;
                //     if (this.OutputBackground != null)
                //         this.output.Background = this.OutputBackground;
                //     if (oldOutput == null)
                //     {
                //         this.outputBlock = new Paragraph(this.output);
                //         this.Document.Blocks.InsertBefore(this.promptBlock, this.outputBlock);
                //     }
                //     else
                //     {
                //         this.outputBlock.Inlines.InsertAfter(oldOutput, this.output);
                //     }
                // }
                // if (text.EndsWith(Environment.NewLine) == true)
                // {
                //     text = text.Substring(0, text.Length - Environment.NewLine.Length);
                //     this.output.Text += this.outputLeftText + text;
                //     this.outputLeftText = Environment.NewLine;
                // }
                // else
                // {
                //     this.output.Text += this.outputLeftText + text;
                //     this.outputLeftText = string.Empty;
                // }
                this.outputText += text;
                this.text = this.outputText + this.promptText;

                if (isEnd == true)
                    this.caretPosition = this.TextInternal.Length;
                this.oldText = this.text;
                this.oldPosition = this.caretPosition;
            }
            finally
            {
                this.refStack--;
            }
        }

        private string CommandText
        {
            get
            {
                return this.promptText.Substring(this.Prompt.Length);
            }
            set
            {
                this.refStack++;
                try
                {
                    this.promptText = this.Prompt + value;
                }
                finally
                {
                    this.refStack--;
                }
            }
        }

        public class ExecutedEent : UnityEvent<string>
        {

        }

        protected virtual bool OnPreviewKeyDown(KeyCode key, EventModifiers modifiers)
        {
            if (key == KeyCode.Backspace)
            {
                //if (modifiers == EventModifiers.None)
                {
                    var inputPosition = this.outputText.Length + this.prompt.Length;
                    if (this.caretPosition <= inputPosition)
                        return true;
                }
            }
            return false;
        }

        private Event m_ProcessingEvent = new Event();

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!isFocused)
                return;

            bool consumedEvent = false;
            while (Event.PopEvent(m_ProcessingEvent))
            {
                if (m_ProcessingEvent.rawType == EventType.KeyDown)
                {
                    if (this.OnPreviewKeyDown(m_ProcessingEvent.keyCode, m_ProcessingEvent.modifiers) == true)
                        continue;
                    consumedEvent = true;
                    var shouldContinue = KeyPressed(m_ProcessingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        //SendOnSubmit();
                        //DeactivateInputField();
                        break;
                    }
                }

                switch (m_ProcessingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (m_ProcessingEvent.commandName)
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
                UpdateLabel();
                this.promptText = this.text.Substring(this.outputText.Length);
                this.inputText = this.promptText.Substring(this.prompt.Length);
                this.completion = string.Empty;
            }

            eventData.Use();
        }
    }
}
