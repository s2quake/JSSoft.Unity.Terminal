////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(Image))]
    public class TerminalSwiper : Selectable
    {
        private ITerminalGrid grid;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.GetComponent<Image>().enabled = false;
            TerminalKeyboardEvents.Opened += TerminalKeyboard_Opened;
            TerminalKeyboardEvents.Done += TerminalKeyboard_Done;
            TerminalKeyboardEvents.Canceled += TerminalKeyboard_Canceled;
        }

        protected override void OnDisable()
        {
            TerminalKeyboardEvents.Opened -= TerminalKeyboard_Opened;
            TerminalKeyboardEvents.Done -= TerminalKeyboard_Done;
            TerminalKeyboardEvents.Canceled -= TerminalKeyboard_Canceled;
            base.OnDisable();
        }

        private void TerminalKeyboard_Opened(object sender, TerminalKeyboardEventArgs e)
        {
            this.GetComponent<Image>().enabled = true;
            if (sender is TerminalKeyboardBase keyboard)
            {
                this.grid = keyboard.Grid;
            }
        }

        private void TerminalKeyboard_Done(object sender, TerminalKeyboardEventArgs e)
        {
            this.GetComponent<Image>().enabled = false;
            this.grid = null;
        }

        private void TerminalKeyboard_Canceled(object sender, EventArgs e)
        {
            this.GetComponent<Image>().enabled = false;
            this.grid = null;
        }
    }
}
