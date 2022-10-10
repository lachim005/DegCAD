using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for CommandPallete.xaml
    /// </summary>
    public partial class CommandPallete : UserControl
    {
        public CommandPallete()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Generates commands from the command list and 
        /// </summary>
        public void GenerateCommands(MainWindow mw)
        {
            List<GeometryCommandData> commands = GeometryCommandsList.GetCommands((c) => mw.ActiveEditor?.ExecuteCommand(c));

            foreach (var cmd in commands)
            {
                //Creates the RoutedUICommand to work with keyboard shortcuts
                RoutedCommand rcmd = new RoutedUICommand(
                    cmd.Name,
                    cmd.Description,
                    typeof(MainWindow),
                    new InputGestureCollection() { new KeyGesture(cmd.Key, cmd.ModifierKey)
                });

                //Binds the new command to the main window. Can execute if active editor isn't null
                mw.CommandBindings.Add(new(rcmd, cmd.ExecuteHandler, (s, e) => e.CanExecute = mw.ActiveEditor is not null));

                //Adds a button for every command to the toolbar
                cmdButtons.Children.Add(new Button() { Content = cmd.Name, ToolTip = cmd.Description, Command=rcmd });
            }

        }
    }
}
