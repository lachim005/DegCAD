using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for AxonometrySetup.xaml
    /// </summary>
    public partial class AxonometrySetup : Window
    {
        public bool Canceled { get; set; } = true;

        public TimelineItem? Axis;

        public AxonometrySetup()
        {
            InitializeComponent();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            switch (axoTab.SelectedIndex)
            {
                case 0:
                    //Axonometry given by the axonometry triangle lenghts
                    if (!double.TryParse(xyLen.Text, out double xy) ||xy < .1) { MessageBox.Show("Zadány neplatné hodnoty"); return; }
                    if (!double.TryParse(yzLen.Text, out double yz) ||yz < .1) { MessageBox.Show("Zadány neplatné hodnoty"); return; }
                    if (!double.TryParse(zxLen.Text, out double zx) || zx < .1) { MessageBox.Show("Zadány neplatné hodnoty"); return; }

                    double biggest = 0;
                    double squareSum = xy*xy + yz*yz + zx*zx;
                    if (xy > biggest) biggest = xy;
                    if (yz > biggest) biggest = yz;
                    if (zx > biggest) biggest = zx;
                    squareSum -= biggest * biggest;
                    if (biggest*biggest >= squareSum) { MessageBox.Show("Nejedná se o ostroúhlý trojúhelník"); return; }

                    Vector2 xPoint = (0, 0);
                    Vector2 yPoint = (xy, 0);

                    Circle2 c1 = new((0, 0), zx);
                    Circle2 c2 = new((xy, 0), yz);
                    var intersections = c1.FindIntersections(c2);
                    if (intersections is null) { MessageBox.Show("Nejedná se o trojúhelník"); return; }
                    Vector2 zPoint = intersections.Value.Item1;


                    var yzSegment = zPoint - yPoint;
                    var zxSegment = xPoint - zPoint;

                    ParametricLine2 zLine = new((zPoint.X, Math.Abs(zPoint.Y)), (0, 1));
                    ParametricLine2 xLine = new(xPoint, (-yzSegment.Y, yzSegment.X));
                    ParametricLine2 yLine = new(yPoint, (-zxSegment.Y, zxSegment.X));

                    var centerOffset = zLine.FindIntersection(yLine);

                    xPoint -= centerOffset;
                    yPoint -= centerOffset;
                    zPoint -= centerOffset;

                    MongeItems.Point originPt = new(0, 0);
                    MongeItems.Point xMPt = new(xPoint.X, xPoint.Y, DegCAD.Style.Default);
                    MongeItems.Point yMPt = new(yPoint.X, yPoint.Y, DegCAD.Style.Default);
                    MongeItems.Point zMPt = new(zPoint.X, zPoint.Y, DegCAD.Style.Default);

                    var xMLn = new HalfLine((0, 0), xLine.DirectionVector, DegCAD.Style.Default);
                    var yMLn = new HalfLine((0, 0), yLine.DirectionVector, DegCAD.Style.Default);
                    var zMLn = new HalfLine((0, 0), zLine.DirectionVector, DegCAD.Style.Default);

                    Axis = new(new IMongeItem[] {
                        new Axis(),
                        xMLn,
                        yMLn,
                        zMLn,
                        new MongeItems.LineSegment(xPoint, yPoint, DegCAD.Style.Default),
                        new MongeItems.LineSegment(yPoint, zPoint, DegCAD.Style.Default),
                        new MongeItems.LineSegment(zPoint, xPoint, DegCAD.Style.Default),
                        originPt,
                        xMPt,
                        yMPt,
                        zMPt,
                        new MongeItems.Label("0", "", "", originPt.Coords, DegCAD.Style.Default, originPt.Clone()),
                        new MongeItems.Label("X", "", "", xMPt.Coords, DegCAD.Style.Default, xMPt.Clone()),
                        new MongeItems.Label("Y", "", "", yMPt.Coords, DegCAD.Style.Default, yMPt.Clone()),
                        new MongeItems.Label("Z", "", "", zMPt.Coords, DegCAD.Style.Default, zMPt.Clone()),
                        new MongeItems.Label("x", "", "", xLine.DirectionVector.ChangeLength(-5), DegCAD.Style.Default, xMLn.Clone()),
                        new MongeItems.Label("y", "", "", yLine.DirectionVector.ChangeLength(-5), DegCAD.Style.Default, yMLn.Clone()),
                        new MongeItems.Label("z", "", "", zLine.DirectionVector.ChangeLength(-5), DegCAD.Style.Default, zMLn.Clone()),
                    }); ;


                    Canceled = false;
                    Close();
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
