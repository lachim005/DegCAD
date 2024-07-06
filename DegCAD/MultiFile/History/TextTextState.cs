using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class TextTextState : IMFHistoryState
    {
        MFText textObject;

        public string text;

        public TextTextState(MFText textObject)
        {
            this.textObject = textObject;
            text = textObject.Text;
        }

        public void ApplyState()
        {
            textObject.Text = text;

            if (textObject.Container is not MFContainer cont) return;

            cont.Page.Editor?.SelectPage(cont.Page);
            cont.Select();
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new TextTextState(textObject);
        }
    }
}
