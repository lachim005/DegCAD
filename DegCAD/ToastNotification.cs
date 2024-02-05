using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class ToastNotification
    {
        public string Text { get; set; }
        public string Header { get; set; }
        public string ButtonText { get; set; }
        public Action ButtonAction { get; set; }

        public ToastNotification(string text, string header, string buttonText, Action buttonAction)
        {
            ButtonAction = buttonAction;
            ButtonText = buttonText;
            Header = header;
            Text = text;
        }
    }
}
