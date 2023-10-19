using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for CommandPallete.xaml
    /// </summary>
    public partial class CommandPallete : UserControl
    {
        MainWindow? mw;
        public List<(GeometryCommandData, Button)> Buttons { get; init; } = new();

        public CommandPallete()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Generates commands from the command list and 
        /// </summary>
        public void GenerateCommands(MainWindow mw)
        {
            Action<IGeometryCommand> executeCommand = (c) => mw.ActiveEditor?.ExecuteCommand(c);
            this.mw = mw;

            AddCommand(new("Bod", "Umístí do projektu pojmenovaný bod", Key.None, ModifierKeys.None, (_, _) => executeCommand(new Point2D()), "cmdPointIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Průměty bodu", "Umístí do půdorysny a nárysny průměty bodu", Key.B, ModifierKeys.Control, (_, _) => executeCommand(new MongePoint3D()), "cmdPointProjectionsIcon", ProjectionType.Monge));
            AddCommand(new("Doplnit průmět", "Doplní k průmětu bodu průmět v druhé průmětně", Key.None, ModifierKeys.None, (_, _) => executeCommand(new AddPointProjection()), "cmdPointAddProjectionIcon", ProjectionType.Monge));
            AddCommand(new("Střed", "Střed mezi dvěma body", Key.S, ModifierKeys.Control, (_, _) => executeCommand(new Middle()), "cmdMiddleIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Vynést body", "Umístí body podle zadaných souřadnic", Key.B, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new MongePointCoordInput()), "cmdPointInputIcon", ProjectionType.Monge));
            AddCommand(new("Vynést body", "Umístí body podle zadaných souřadnic", Key.B, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new PointCoordInput2D()), "cmdPointInputIcon2D", ProjectionType.Plane));
            AddSeparator();
            AddCommand(new("Průmět přímky", "Umístí do půdorysny nebo do nárysny průmět přímky", Key.None, ModifierKeys.None, (_, _) => executeCommand(new MongeLine2D()), "cmdLineProjectionIcon", ProjectionType.Monge));
            AddCommand(new("Průměty přímky", "Umístí do půdorysny a nárysny průměty přímky", Key.P, ModifierKeys.Control, (_, _) => executeCommand(new MongeLine3D()), "cmdLineProjectionsIcon", ProjectionType.Monge));
            AddCommand(new("Kolmice", "Kolmice na danou přímku", Key.K, ModifierKeys.Control, (_, _) => executeCommand(new PerpendicularLine()), "cmdPerpendicularLineIcon", ProjectionType.Monge));
            AddCommand(new("Rovnoběžka", "Rovnoběžka na danou přímku", Key.R, ModifierKeys.Control, (_, _) => executeCommand(new ParallelLine()), "cmdParallelLineIcon", ProjectionType.Monge));
            AddCommand(new("Polopřímka", "Polopřímka dána počátkem a směrem", Key.R, ModifierKeys.Control, (_, _) => executeCommand(new HalfLine()), "cmdHalfLineIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddSeparator();
            AddCommand(new("Úsečka", "Úsečka mezi dvěma body", Key.U, ModifierKeys.Control, (_, _) => executeCommand(new LineSegment()), "cmdSegmentIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Kolmá úsečka", "Úsečka mezi dvěma body kolmá na jinou přímku", Key.None, ModifierKeys.None, (_, _) => executeCommand(new PerpendicularSegment()), "cmdPerpendicularSegmentIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Rovnoběžná úsečka", "Úsečka mezi dvěma body rovnoběžná s jinou přímkou", Key.None, ModifierKeys.None, (_, _) => executeCommand(new ParallelSegment()), "cmdParallelSegmentIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Úsečka na přímce", "Úsečka mezi dvěma body, které leží na přímce", Key.None, ModifierKeys.None, (_, _) => executeCommand(new SegmentOnLine()), "cmdSegmentOnLineIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddSeparator();
            AddCommand(new("Kružnice", "Kružnice dána středem a bodem na ní", Key.O, ModifierKeys.Control, (_, _) => executeCommand(new Circle()), "cmdCircleIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Oblouk", "Oblouk dán středem, bodem na kružnici a dvěma úhly", Key.O, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new Arc()), "cmdArcIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Elipsa", "Elipsa dána středem hlavním a vedlejším vrcholem", Key.E, ModifierKeys.Control, (_, _) => executeCommand(new GeometryCommands.Ellipse()), "cmdEllipseIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Parabola", "Parabola dána ohniskem a vrcholem", Key.Q, ModifierKeys.Control, (_, _) => executeCommand(new GeometryCommands.Parabola()), "cmdParabolaIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Hyperbola", "Větev hyperboly dána středem, vrcholem a koncovým bodem na hyperbole. Zobrazení hyperboly je pouze přibližné.", Key.H, ModifierKeys.Control, (_, _) => executeCommand(new Hyperbola()), "cmdHyperbolaIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddSeparator();
            AddCommand(new("Přenést vzdálenost", "Přenese vzdálenost mezi dvěma body", Key.V, ModifierKeys.Control, (_, _) => executeCommand(new TranslateSize()), "cmdTranslateSizeIcon", ProjectionType.Plane | ProjectionType.Monge));
            AddCommand(new("Sklopit bod", "Sklopí bod pomocí jeho druhého průmětu", Key.S, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new CastPoint()), "cmdCastPointIcon", ProjectionType.Monge));

        }

        public void AddCommand(GeometryCommandData cmd)
        {
            //Creates the RoutedUICommand to work with keyboard shortcuts
            RoutedCommand rcmd = new RoutedUICommand(
                cmd.Name,
                cmd.Description,
                typeof(MainWindow),
                new InputGestureCollection() { new KeyGesture(cmd.Key, cmd.ModifierKey)
            });

            //Binds the new command to the main window. Can execute if active editor isn't null
            mw?.CommandBindings.Add(new(rcmd, cmd.ExecuteHandler, mw.CanExecuteEditorCommand));

            //Adds a button for every command to the toolbar
            StackPanel stp = new();
            if (cmd.IconName is not null)
                stp.Children.Add(new ContentControl()
                {
                    Content = FindResource(cmd.IconName),
                    Width = 60,
                    Height = 60
                });
            stp.Children.Add(new TextBlock() { 
                Text = cmd.Name, 
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
            });
            Button cmdButton = new()
            {
                Width = 70, Content = stp,
                Command = rcmd,
                ToolTip = cmd.Description,
                Style = FindResource("cmdPaletteBtnStyle") as System.Windows.Style 
            };
            cmdButtons.Children.Add(cmdButton);
            Buttons.Add((cmd, cmdButton));
        }

        public void AddSeparator()
        {
            Rectangle r = new()
            {
                Width = 2,
                Height = cmdButtons.Height,
                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 240, 240))
            };
            cmdButtons.Children.Add(r);
        }

        public void ShowButtons(ProjectionType projType)
        {
            foreach (var btn in Buttons)
            {
                if (btn.Item1.ProjectionTypes.HasFlag(projType))
                {
                    btn.Item2.Visibility = Visibility.Visible;
                } else
                {
                    btn.Item2.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer) return;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.ContentHorizontalOffset - e.Delta);
        }
    }
}
