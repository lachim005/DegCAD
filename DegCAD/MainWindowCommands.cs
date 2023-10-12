using DegCAD.Dialogs;
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
        public static RoutedCommand pageLayoutCommand = new();
        public static RoutedCommand addGuideCommand = new();
        public static RoutedCommand openDebugMenuCommand = new();

        private bool IsActiveEditorIdle()
        {
            if (ActiveEditor is null) return false;
            if (ActiveEditor.ExecutingCommand) return false;
            return true;
        }
        private void OpenSaveFileDialog(Editor editor)
        {
            if (editor is null) return;

            SaveFileDialog sfd = new();
            sfd.Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*";
            sfd.FileName = editor.FileName + ".dgproj";

            if (sfd.ShowDialog() != true) return;

            editor.FolderPath = Path.GetDirectoryName(sfd.FileName);
            editor.FileName = Path.GetFileNameWithoutExtension(sfd.FileName);
        }
        private async void SaveEditorAsync(Editor editor)
        {
            await Task.Delay(1);
            try
            {
                editor?.SaveEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při ukládání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void OpenFileAsync(string path)
        {
            await Task.Delay(1);
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
            ed.Changed = false;
            openEditors.Add(new(ed));
            editorTabs.SelectedIndex = openEditors.Count - 1;
        }

        public void CanExecuteEditorCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsActiveEditorIdle();
        }
        public void CanAddGuide(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsActiveEditorIdle() && ActiveEditor?.Guide is null;
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
            NewFileDialog nfd = new();
            nfd.ShowDialog();
            if (nfd.ProjectionType is null) return;
            Editor ed = new($"Bez názvu {editorCounter}", nfd.ProjectionType.Value);
            ed.AddAxis(ed.viewPort.Layers[1]);
            ed.styleSelector.AddDefaultColors();
            ed.Changed = false;
            openEditors.Add(new(ed));
            editorTabs.SelectedIndex = openEditors.Count - 1;
            editorCounter++;
        }
        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*";
            if (ofd.ShowDialog() != true) return;

            OpenFileAsync(ofd.FileName);
        }
        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            if (ActiveEditor.FolderPath is null) OpenSaveFileDialog(ActiveEditor);

            //User has canceled the save file dialog
            if (ActiveEditor.FolderPath is null) return;

            SaveEditorAsync(ActiveEditor);
        }
        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            OpenSaveFileDialog(ActiveEditor);

            //User has canceled the save file dialog
            if (ActiveEditor.FolderPath is null) return;

            SaveEditorAsync(ActiveEditor);
        }
        private void PrintCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            PrintDialog pd = new(ActiveEditor);
            pd.ShowDialog();
        }
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;
            if (CanCloseEditor(ActiveEditor))
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
        private void AboutClick(object sender, RoutedEventArgs e)
        {
            AboutDialog.Open();
        }
        private void OpenPageLayoutWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            PageLayoutWindow plw = new(ActiveEditor.viewPort);
            ActiveEditor.ShowView(plw, "Rozložení na papíře", false);
        }
        private void AddGuideCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            ActiveEditor.Guide = new();
        }
        private void OpenDebugMenu(object sender, ExecutedRoutedEventArgs e)
        {
            DebugMenu.DebugWindow dw = new(this);
            dw.Show();
        }
    }
}
