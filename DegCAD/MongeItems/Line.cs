using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    public class Line : IMongeItem
    {
        private ParametricLine2 _bottomProjectionLine;
        private ParametricLine2 _topProjectionLine;
        int botSign;
        int topSign;

        public ParametricLine2 BottomProjectionLine
        {
            get => _bottomProjectionLine;
            set
            {
                _bottomProjectionLine = value;

                //Calculates the infinity sign for drawing the line
                botSign = 1;
                if (value.DirectionVector.Y * value.DirectionVector.X < 0)
                {
                    botSign = -1;
                }
            }
        }
        public ParametricLine2 TopProjectionLine
        {
            get => _topProjectionLine;
            set
            {
                _topProjectionLine = value;

                //Calculates the infinity sign for drawing the line
                topSign = -1;
                if (value.DirectionVector.Y * value.DirectionVector.X < 0)
                {
                    topSign = 1;
                }
            }
        }

        public Style Style = Style.Default;


        public void Draw(GeometryDrawer gd)
        {
            gd.DrawLine(BottomProjectionLine, double.PositiveInfinity * botSign, BottomProjectionLine.GetParamFromY(0), Style);
            gd.DrawLine(TopProjectionLine, double.PositiveInfinity * topSign, TopProjectionLine.GetParamFromY(0), Style);
        }
    }
}
