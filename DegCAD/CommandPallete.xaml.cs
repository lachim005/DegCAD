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

            AddCommand(new("Bod", "Umístí do projektu pojmenovaný bod", Key.None, ModifierKeys.None, (_, _) => executeCommand(new Point2D()), "cmdPointIcon"));
            AddCommand(new("Průměty bodu", "Umístí do půdorysny a nárysny průměty bodu", Key.B, ModifierKeys.Control, (_, _) => executeCommand(new Point3D()), "cmdPointProjectionsIcon"));
            AddCommand(new("Doplnit průmět", "Doplní k průmětu bodu průmět v druhé průmětně", Key.None, ModifierKeys.None, (_, _) => executeCommand(new AddPointProjection()), "cmdPointAddProjectionIcon"));
            AddCommand(new("Střed", "Střed mezi dvěma body", Key.S, ModifierKeys.Control, (_, _) => executeCommand(new Middle()), "cmdMiddleIcon"));
            AddCommand(new("Vynést body", "Umístí body podle zadaných souřadnic", Key.B, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new PointCoordInput()), "cmdPointInputIcon"));
            AddSeparator();
            AddCommand(new("Průmět přímky", "Umístí do půdorysny nebo do nárysny průmět přímky", Key.None, ModifierKeys.None, (_, _) => executeCommand(new Line2D()), "cmdLineProjectionIcon"));
            AddCommand(new("Průměty přímky", "Umístí do půdorysny a nárysny průměty přímky", Key.P, ModifierKeys.Control, (_, _) => executeCommand(new Line3D()), "cmdLineProjectionsIcon"));
            AddCommand(new("Rovnoběžka", "Rovnoběžka na danou přímku", Key.R, ModifierKeys.Control, (_, _) => executeCommand(new ParallelLine()), "cmdParallelLineIcon"));
            AddCommand(new("Kolmice", "Kolmice na danou přímku", Key.K, ModifierKeys.Control, (_, _) => executeCommand(new PerpendicularLine()), "cmdPerpendicularLineIcon"));
            AddSeparator();
            AddCommand(new("Úsečka", "Úsečka mezi dvěma body", Key.U, ModifierKeys.Control, (_, _) => executeCommand(new LineSegment()), "cmdSegmentIcon"));
            AddCommand(new("Kolmá úsečka", "Úsečka mezi dvěma body kolmá na jinou přímku", Key.None, ModifierKeys.None, (_, _) => executeCommand(new PerpendicularSegment()), "cmdPerpendicularSegmentIcon"));
            AddCommand(new("Rovnoběžná úsečka", "Úsečka mezi dvěma body rovnoběžná s jinou přímkou", Key.None, ModifierKeys.None, (_, _) => executeCommand(new ParallelSegment()), "cmdParallelSegmentIcon"));
            AddCommand(new("Úsečka na přímce", "Úsečka mezi dvěma body, které leží na přímce", Key.None, ModifierKeys.None, (_, _) => executeCommand(new SegmentOnLine()), "cmdSegmentOnLineIcon"));
            AddSeparator();
            AddCommand(new("Kružnice", "Kružnice dána středem a bodem na ní", Key.O, ModifierKeys.Control, (_, _) => executeCommand(new Circle()), "cmdCircleIcon"));
            AddCommand(new("Oblouk", "Oblouk dán středem, bodem na kružnici a dvěma úhly", Key.O, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new Arc()), "cmdArcIcon"));
            AddCommand(new("Elipsa", "Elipsa dána středem hlavním a vedlejším vrcholem", Key.E, ModifierKeys.Control, (_, _) => executeCommand(new GeometryCommands.Ellipse()), "cmdEllipseIcon"));
            AddCommand(new("Parabola", "Parabola dána ohniskem a vrcholem", Key.Q, ModifierKeys.Control, (_, _) => executeCommand(new GeometryCommands.Parabola()), "cmdParabolaIcon"));
            AddCommand(new("Hyperbola", "Větev hyperboly dána středem, vrcholem a koncovým bodem na hyperbole. Zobrazení hyperboly je pouze přibližné.", Key.H, ModifierKeys.Control, (_, _) => executeCommand(new Hyperbola()), "cmdHyperbolaIcon"));
            AddSeparator();
            AddCommand(new("Přenést vzdálenost", "Přenese vzdálenost mezi dvěma body", Key.V, ModifierKeys.Control, (_, _) => executeCommand(new TranslateSize()), "cmdTranslateSizeIcon"));
            AddCommand(new("Sklopit bod", "Sklopí bod pomocí jeho druhého průmětu", Key.S, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new CastPoint()), "cmdCastPointIcon"));

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
                Style = FindResource("cmdPaletteBtnStyle") as System.Windows.Style 
            };
            cmdButtons.Children.Add(cmdButton);
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

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer) return;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.ContentHorizontalOffset - e.Delta);
        }
    }
}
