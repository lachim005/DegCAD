using DegCAD.TimelineElements;
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
        //Last values
        private static int lastSelectedInputType = 0;
        private static string lastXYLength = "";
        private static string lastYZLength = "";
        private static string lastZXLength = "";
        private static string lastXYAngle = "";
        private static string lastYZAngle = "";
        private static string lastZXAngle = "";
        private static int lastDisabledAngle = 2;

        public bool Canceled { get; set; } = true;

        public TimelineItem? Axis;
        public AxonometryAxes? AxonometryAxes { get; set; }

        private int disabledAngle = 2;

        public AxonometrySetup()
        {
            InitializeComponent();
            LoadLastValues();
        }

        private void LoadLastValues()
        {
            axoTab.SelectedIndex = lastSelectedInputType;
            xyLen.Text = lastXYLength;
            yzLen.Text = lastYZLength;
            zxLen.Text = lastZXLength;
            xyAngle.Text = lastXYAngle;
            yzAngle.Text = lastYZAngle;
            zxAngle.Text = lastZXAngle;
            switch (lastDisabledAngle)
            {
                case 0: 
                    computeXYAngle.IsChecked = true;
                    xyAngle.IsEnabled = false;
                    break;
                case 1: 
                    computeYZAngle.IsChecked = true; 
                    yzAngle.IsEnabled = false;
                    break;
                default: 
                    computeZXAngle.IsChecked = true; 
                    zxAngle.IsEnabled = false;
                    break;
            }
        }
        private void SaveLastValues()
        {
            lastSelectedInputType = axoTab.SelectedIndex;
            lastXYLength = xyLen.Text;
            lastYZLength = yzLen.Text;
            lastZXLength = zxLen.Text;
            lastXYAngle = xyAngle.Text;
            lastYZAngle = yzAngle.Text;
            lastZXAngle = zxAngle.Text;
            lastDisabledAngle = disabledAngle;
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            switch (axoTab.SelectedIndex)
            {
                case 0:
                    {
                        //Axonometry given by the axonometry triangle lenghts
                        if (!double.TryParse(xyLen.Text, out double xy) || xy < .1) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                        if (!double.TryParse(yzLen.Text, out double yz) || yz < .1) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                        if (!double.TryParse(zxLen.Text, out double zx) || zx < .1) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                        double biggest = 0;
                        double squareSum = xy * xy + yz * yz + zx * zx;
                        if (xy > biggest) biggest = xy;
                        if (yz > biggest) biggest = yz;
                        if (zx > biggest) biggest = zx;
                        squareSum -= biggest * biggest;
                        if (biggest * biggest >= squareSum) { MessageBox.Show("Nejedná se o ostroúhlý trojúhelník", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                        Vector2 xPoint = (0, 0);
                        Vector2 yPoint = (xy, 0);

                        Circle2 c1 = new((0, 0), zx);
                        Circle2 c2 = new((xy, 0), yz);
                        var intersections = c1.FindIntersections(c2);
                        if (intersections is null) { MessageBox.Show("Nejedná se o trojúhelník", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
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

                        TimelineElements.Point originPt = new(0, 0);
                        TimelineElements.Point xMPt = new(xPoint.X, xPoint.Y, DegCAD.Style.Default);
                        TimelineElements.Point yMPt = new(yPoint.X, yPoint.Y, DegCAD.Style.Default);
                        TimelineElements.Point zMPt = new(zPoint.X, zPoint.Y, DegCAD.Style.Default);

                        var xMLn = new HalfLine((0, 0), xLine.DirectionVector, DegCAD.Style.Default);
                        var yMLn = new HalfLine((0, 0), yLine.DirectionVector, DegCAD.Style.Default);
                        var zMLn = new HalfLine((0, 0), zLine.DirectionVector, DegCAD.Style.Default);

                        Axis = new([
                            new Axis(),
                            xMLn,
                            yMLn,
                            zMLn,
                            new TimelineElements.LineSegment(xPoint, yPoint, DegCAD.Style.Default),
                            new TimelineElements.LineSegment(yPoint, zPoint, DegCAD.Style.Default),
                            new TimelineElements.LineSegment(zPoint, xPoint, DegCAD.Style.Default),
                            originPt,
                            xMPt,
                            yMPt,
                            zMPt,
                            new TimelineElements.Label("0", "", "", originPt.Coords, DegCAD.Style.Default, originPt.CloneElement()),
                            new TimelineElements.Label("X", "", "", xMPt.Coords, DegCAD.Style.Default, xMPt.CloneElement()),
                            new TimelineElements.Label("Y", "", "", yMPt.Coords, DegCAD.Style.Default, yMPt.CloneElement()),
                            new TimelineElements.Label("Z", "", "", zMPt.Coords, DegCAD.Style.Default, zMPt.CloneElement()),
                            new TimelineElements.Label("x", "", "", xLine.DirectionVector.ChangeLength(-5), DegCAD.Style.Default, xMLn.CloneElement()),
                            new TimelineElements.Label("y", "", "", yLine.DirectionVector.ChangeLength(-5), DegCAD.Style.Default, yMLn.CloneElement()),
                            new TimelineElements.Label("z", "", "", zLine.DirectionVector.ChangeLength(-5), DegCAD.Style.Default, zMLn.CloneElement()),
                        ]); ;

                        AxonometryAxes = new(xLine.DirectionVector, yLine.DirectionVector, zLine.DirectionVector);

                        Canceled = false;
                        Close();
                    }
                    break;
                case 1:
                    {
                        //Axonometry given by the angles of the axonometry axis cross

                        double a1, a2, a3;

                        switch (disabledAngle)
                        {
                            case 0:
                                if (!double.TryParse(yzAngle.Text, out a2) || a2 <= 90 || a2 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                if (!double.TryParse(zxAngle.Text, out a3) || a3 <= 90 || a3 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                a1 = 360 - a2 - a3;
                                if (a1 <= 90 || a1 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                break;
                            case 1:
                                if (!double.TryParse(xyAngle.Text, out a1) || a1 <= 90 || a1 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                if (!double.TryParse(zxAngle.Text, out a3) || a3 <= 90 || a3 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                a2 = 360 - a1 - a3;
                                if (a2 <= 90 || a2 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                break;
                            default:
                                if (!double.TryParse(xyAngle.Text, out a1) || a1 <= 90 || a1 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                if (!double.TryParse(yzAngle.Text, out a2) || a2 <= 90 || a2 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                a3 = 360 - a1 - a2;
                                if (a3 <= 90 || a3 >= 180) { MessageBox.Show("Zadány neplatné hodnoty", "Nastavení axonometrie", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                                break;
                        }

                        Vector2 xLine = Math.SinCos(a3 / 180 * Math.PI);
                        Vector2 yLine = Math.SinCos((a3 + a1) / 180 * Math.PI);
                        Vector2 zLine = (0, 1);

                        TimelineElements.Point originPt = new(0, 0);

                        var xMLn = new HalfLine((0, 0), xLine, DegCAD.Style.Default);
                        var yMLn = new HalfLine((0, 0), yLine, DegCAD.Style.Default);
                        var zMLn = new HalfLine((0, 0), zLine, DegCAD.Style.Default);

                        Axis = new([
                            new Axis(),
                            xMLn,
                            yMLn,
                            zMLn,
                            originPt,
                            new TimelineElements.Label("0", "", "", originPt.Coords, DegCAD.Style.Default, originPt.CloneElement()),
                            new TimelineElements.Label("x", "", "", xLine.ChangeLength(-5), DegCAD.Style.Default, xMLn.CloneElement()),
                            new TimelineElements.Label("y", "", "", yLine.ChangeLength(-5), DegCAD.Style.Default, yMLn.CloneElement()),
                            new TimelineElements.Label("z", "", "", zLine.ChangeLength(-5), DegCAD.Style.Default, zMLn.CloneElement()),
                        ]); ;

                        AxonometryAxes = new(xLine, xLine, zLine);

                        Canceled = false;
                        Close();
                    }
                    break;
                default:
                    break;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComputeAngleChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            if (sender is not RadioButton rb) return;

            xyAngle.IsEnabled = true;
            yzAngle.IsEnabled = true;
            zxAngle.IsEnabled = true;

            switch (rb.Name)
            {
                case "computeXYAngle":
                    xyAngle.IsEnabled = false;
                    disabledAngle = 0;
                    break;
                case "computeYZAngle":
                    yzAngle.IsEnabled = false;
                    disabledAngle = 1;
                    break;
                case "computeZXAngle":
                    zxAngle.IsEnabled = false;
                    disabledAngle = 2;
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveLastValues();
        }

        private static bool ValidateAxoLengths(double xy, double yz, double zx)
        {
            double biggest = 0;
            double squareSum = xy * xy + yz * yz + zx * zx;
            if (xy > biggest) biggest = xy;
            if (yz > biggest) biggest = yz;
            if (zx > biggest) biggest = zx;
            squareSum -= biggest * biggest;
            if (biggest * biggest >= squareSum) { return false; }

            Circle2 c1 = new((0, 0), zx);
            Circle2 c2 = new((xy, 0), yz);
            var intersections = c1.FindIntersections(c2);
            if (intersections is null) { return false; }

            return true;
        }

        private static bool ValidateAxoAngles(double a1, double a2)
        {
            double a3 = 360 - a1 - a2;
            return a1 < 180 && a1 > 90 && a2 < 180 && a2 > 90 && a3 < 180 && a3 > 90;
        }

        private void RandomAxoTriangle(object sender, RoutedEventArgs e)
        {
            Random r = new(DateTime.Now.Millisecond);

            int xy, yz, zx;
            do
            {
                var b = r.Next(6, 9);
                xy = b + 1;
                yz = b + r.Next(0, 5);
                zx = b + r.Next(0, 5);
            } while (!ValidateAxoLengths(xy, yz, zx));

            xyLen.Text = xy.ToString();
            yzLen.Text = yz.ToString();
            zxLen.Text = zx.ToString();
        }

        private void RandomAxoAngles(object sender, RoutedEventArgs e)
        {
            Random r = new(DateTime.Now.Millisecond);

            int a1, a2;
            do
            {
                a1 = r.Next(10, 18) * 10;
                a2 = r.Next(10, 18) * 10;
            } while (!ValidateAxoAngles(a1, a2));

            xyAngle.Text = a1.ToString();
            yzAngle.Text = a2.ToString();
            zxAngle.Text = (360 - a1 - a2).ToString();
        }
    }
}
