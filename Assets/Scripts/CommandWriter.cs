using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using JSSoft.UI;

namespace JSSoft.Communication.Shells
{
    class CommandWriter : StringWriter
    {
        private readonly TerminalPro control;
        private StringBuilder stringBuilder = new StringBuilder();

        public CommandWriter(TerminalPro control)
        {
            this.control = control;
            // TerminalColor.ForegroundColorChanged += TerminalColor_ForegroundColorChanged;
            // TerminalColor.BackgroundColorChanged += TerminalColor_BackgroundColorChanged;
        }

        // public IEnumerator UpdateMessage(CancellationToken cancellation)
        // {
        //     while (!cancellation.IsCancellationRequested)
        //     {
        //         lock (this.control)
        //         {
        //             if (this.actionList.Any())
        //             {
        //                 foreach (var item in this.actionList)
        //                 {
        //                     item();
        //                 }
        //                 this.actionList.Clear();
        //             }
        //             else
        //             {
        //                 yield return null;
        //             }
        //         }

        //     }
        // }

        // private void TerminalColor_ForegroundColorChanged(object sender, EventArgs e)
        // {
        //     var foregroundColor = TerminalColor.ForegroundColor;

        //     this.control.Dispatcher.InvokeAsync(() =>
        //     {
        //         if (foregroundColor == null)
        //             this.control.OutputForeground = null;
        //         else
        //             this.control.OutputForeground = (Brush)this.control.FindResource(TerminalColors.FindForegroundKey(foregroundColor));
        //     });
        // }

        // private void TerminalColor_BackgroundColorChanged(object sender, EventArgs e)
        // {
        //     var backgroundColor = TerminalColor.BackgroundColor;
        //     this.control.Dispatcher.InvokeAsync(() =>
        //     {
        //         if (backgroundColor == null)
        //             this.control.OutputBackground = null;
        //         else
        //             this.control.OutputBackground = (Brush)this.control.FindResource(TerminalColors.FindBackgroundKey(backgroundColor));
        //     });
        // }

        public string GetString()
        {
            lock (this.control)
            {
                if (this.stringBuilder.Length == 0)
                    return null;
                var text = this.stringBuilder.ToString();
                this.stringBuilder.Clear();
                return text;
            }
        }

        public override void Write(char value)
        {
            base.Write(value);
            lock (this.control)
            {
                this.stringBuilder.Append(value.ToString());
            }
        }

        public override void WriteLine()
        {
            base.WriteLine();
            lock (this.control)
            {
                this.stringBuilder.AppendLine(string.Empty);
            }
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            lock (this.control)
            {
                this.stringBuilder.AppendLine(value);
            }
        }

        public override void Write(string value)
        {
            base.Write(value);
            lock (this.control)
            {
                this.stringBuilder.Append(value);
            }
        }
    }
}