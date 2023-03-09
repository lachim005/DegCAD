using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private void OpenSaveFileDialog()
        {
            if (ActiveEditor is null) return;

            SaveFileDialog sfd = new();
            sfd.Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*";
            sfd.FileName = ActiveEditor.FileName + ".dgproj";

            if (sfd.ShowDialog() != true) return;

            ActiveEditor.FolderPath = Path.GetDirectoryName(sfd.FileName);
            ActiveEditor.FileName = Path.GetFileNameWithoutExtension(sfd.FileName);
        }
        private void SaveEditor()
        {
            try
            {
                ActiveEditor?.SaveEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při ukládání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OpenFile(string path)
        {
            Editor ed;
            try
            {
                ed = EditorLoader.CreateFromFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            openEditors.Add(new(ed));
            editorTabs.SelectedIndex = openEditors.Count - 1;
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
            Editor ed = new($"Bez názvu {editorCounter}");
            ed.AddAxis();
            ed.styleSelector.AddDefaultColors();
            openEditors.Add(new(ed));
            editorTabs.SelectedIndex = openEditors.Count - 1;
            editorCounter++;
        }
        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*";
            if (ofd.ShowDialog() != true) return;

            OpenFile(ofd.FileName);
        }
        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            if (ActiveEditor.FolderPath is null) OpenSaveFileDialog();

            //User has canceled the save file dialog
            if (ActiveEditor.FolderPath is null) return;

            SaveEditor();
        }
        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            OpenSaveFileDialog();

            //User has canceled the save file dialog
            if (ActiveEditor.FolderPath is null) return;

            SaveEditor();
        }
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            openEditors.Remove(new(ActiveEditor));
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
