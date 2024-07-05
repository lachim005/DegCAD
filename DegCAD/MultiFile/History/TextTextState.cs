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

            textObject.Container?.Select();
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new TextTextState(textObject);
        }
    }
}
