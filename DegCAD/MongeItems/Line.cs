using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    public class Line : IMongeItem
    {
        int botSign;
        int topSign;

        public ParametricLine2 BottomProjectionLine { get; init; }
        public ParametricLine2 TopProjectionLine { get; init; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];
        public ParametricLine2[] SnapableLines { get; init; }

        public Style Style = Style.Default;

        public Line(ParametricLine2 botLine, ParametricLine2 topLine, Style style)
        {
            BottomProjectionLine = botLine;
            TopProjectionLine = topLine;

            //Calculates the infinity sign for drawing the line
            botSign = 1;
            if (botLine.DirectionVector.Y * botLine.DirectionVector.X < 0)
            {
                botSign = -1;
            }
            //Calculates the infinity sign for drawing the line
            topSign = -1;
            if (topLine.DirectionVector.Y * topLine.DirectionVector.X < 0)
            {
                topSign = 1;
            }

            SnapableLines = new ParametricLine2[2] { botLine, topLine };
            Style = style;
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawLine(BottomProjectionLine, double.PositiveInfinity * botSign, BottomProjectionLine.GetParamFromY(0), Style);
            gd.DrawLine(TopProjectionLine, double.PositiveInfinity * topSign, TopProjectionLine.GetParamFromY(0), Style);
        }
    }
}
