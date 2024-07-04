using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DegCAD.MultiFile.History
{
    public class TextStyleState : IMFHistoryState
    {
        MFText textObject;
        public bool bold;
        public bool italic;
        public bool underline;
        public bool strikethrough;
        public VerticalAlignment vAlign;
        public TextAlignment hAlign;
        public Color color;
        public int fontSize;

        public TextStyleState(MFText text)
        {
            textObject = text;

            bold = text.Bold;
            italic = text.Italic;
            underline = text.Underline;
            strikethrough = text.Strikethrough;
            vAlign = text.VAlign;
            hAlign = text.HAlign;
            color = text.Color;
            fontSize = text.TextFontSize;
        }

        public void ApplyState()
        {
            textObject.Bold = bold;
            textObject.Italic = italic;
            textObject.Underline = underline;
            textObject.Strikethrough = strikethrough;
            textObject.VAlign = vAlign;
            textObject.HAlign = hAlign;
            textObject.Color = color;
            textObject.TextFontSize = fontSize;

            textObject.Container?.Select();
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new TextStyleState(textObject);
        }
    }
}
