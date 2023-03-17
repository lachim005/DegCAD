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

            AddCommand(new("Průměty bodu", "Umístí do půdorysny a nárysny průměty bodu", Key.B, ModifierKeys.Control, (_, _) => executeCommand(new Point3D())));
            AddCommand(new("Vynést body", "Umístí body podle zadaných souřadnic", Key.B, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new PointCoordInput())));
            AddCommand(new("Průměty přímky", "Umístí do půdorysny a nárysny průměty přímky", Key.P, ModifierKeys.Control, (_, _) => executeCommand(new Line3D())));
            AddCommand(new("Úsečka", "Úsečka mezi dvěma body", Key.U, ModifierKeys.Control, (_, _) => executeCommand(new LineSegment())));
            AddCommand(new("Rovnoběžka", "Rovnoběžka na danou přímku", Key.R, ModifierKeys.Control, (_, _) => executeCommand(new ParallelLine())));
            AddCommand(new("Kolmice", "Kolmice na danou přímku", Key.K, ModifierKeys.Control, (_, _) => executeCommand(new PerpendicularLine())));
            AddCommand(new("Střed", "Střed mezi dvěma body", Key.S, ModifierKeys.Control, (_, _) => executeCommand(new Middle())));
            AddCommand(new("Přenést vzdálenost", "Přenese vzdálenost mezi dvěma body", Key.V, ModifierKeys.Control, (_, _) => executeCommand(new TranslateSize())));
            AddCommand(new("Sklopit bod", "Sklopí bod pomocí jeho druhého průmětu", Key.S, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new CastPoint())));
            AddCommand(new("Kružnice", "Kružnice dána středem a bodem na ní", Key.O, ModifierKeys.Control, (_, _) => executeCommand(new Circle())));
            AddCommand(new("Oblouk", "Oblouk dán středem, bodem na kružnici a dvěma úhly", Key.O, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new Arc())));

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
            cmdButtons.Children.Add(new Button() { Content = cmd.Name, ToolTip = cmd.Description, Command=rcmd });
        }
    }
}
