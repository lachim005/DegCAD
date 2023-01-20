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
        private bool IsActiveEditorIdle()
        {
            if (ActiveEditor is null) return false;
            if (ActiveEditor.ExecutingCommand) return false;
            return true;
        }
        private void CanExecuteEditorCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsActiveEditorIdle();
        }
        private void CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!IsActiveEditorIdle()) return;
            if (ActiveEditor is null) return;
            e.CanExecute = ActiveEditor.Timeline.CanUndo;
        }
        private void CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!IsActiveEditorIdle()) return;
            if (ActiveEditor is null) return;
            e.CanExecute = ActiveEditor.Timeline.CanRedo;
        }

        private void NewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            openEditors.Add(new($"Bez názvu {editorCounter}"));
            editorTabs.SelectedIndex = openEditors.Count;
            editorCounter++;
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
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            openEditors.Remove(ActiveEditor);
        }
        private void UndoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ActiveEditor?.Timeline.Undo();
        }
        private void RedoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ActiveEditor?.Timeline.Redo();
        }
    }
}
