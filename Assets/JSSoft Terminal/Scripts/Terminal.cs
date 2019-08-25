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
    public class Terminal : MonoBehaviour
    {
        // Start is called before the first frame update

        private TerminalPro textBox;
        public void Start()
        {
            this.textBox = this.gameObject.GetComponent<TerminalPro>();

            if (this.textBox != null)
            {

                //this.textBox.Prompt = "c:www>";
                // this.textBox.onValueChanged = new TMP_InputField.OnChangeEvent();
                // this.textBox.onValueChanged.AddListener(TextBox_TextChanged);
                // this.textBox.onEndEdit = new TMP_InputField.SubmitEvent();
                // this.textBox.onEndEdit.AddListener(OnEndEdit);
                // this.textBox.onEndTextSelection = new TMP_InputField.TextSelectionEvent();
                // this.textBox.onEndTextSelection.AddListener(OnEndTextSelection);
                // this.textBox.onValidateInput = OnTextValidateInput;
                // this.textBox.caretPosition = this.textBox.text.Length;
                //this.textBox.ActivateInputField();
            }
        }

    }
}
