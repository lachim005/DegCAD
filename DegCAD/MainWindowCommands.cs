using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DegCAD
{
    public partial class MainWindow
    {
        private void CanExecuteEditorCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            if (ActiveEditor is null)
            {
                e.CanExecute = false;
                return;
            }
            if (ActiveEditor.ExecutingCommand)
            {
                e.CanExecute = false;
                return;
            }
        }

        private void NewCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void UndoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
        private void RedoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
    }
}
