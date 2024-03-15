using DegCAD.Dialogs;
using DegCAD.MultiFile;
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
        public static RoutedCommand newMFCommand = new();
        public static RoutedCommand exportCommand = new();
        public static RoutedCommand pageLayoutCommand = new();
        public static RoutedCommand addGuideCommand = new();
        public static RoutedCommand openDebugMenuCommand = new();

        public static bool OpenEditorSaveFileDialog(Editor editor)
        {
            if (editor is null) return false;

            SaveFileDialog sfd = new();
            sfd.Filter = "DegCAD projekt|*.dgproj|Všechny soubory|*.*";
            sfd.FileName = editor.FileName + ".dgproj";

            if (sfd.ShowDialog() != true) return false;

            editor.FolderPath = Path.GetDirectoryName(sfd.FileName);
            editor.FileName = Path.GetFileNameWithoutExtension(sfd.FileName);
            return true;
        }
        public static async Task<bool> SaveEditorAsync(Editor editor)
        {
            try
            {
                await editor.SaveEditor();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při ukládání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
        private async void OpenFileAsync(string path)
        {
            if (Path.GetExtension(path) == ".dgproj")
            {
                if (await OpenEditorAsync(path) is not Editor ed) return;
                openTabs.Add(new EditorTab(ed));
            } else if (Path.GetExtension(path) == ".dgcomp")
            {
                if (await OpenMFEditorAsync(path) is not MFEditor ed) return;
                var et = new MFEditorTab(ed)
                {
                    FolderPath = Path.GetDirectoryName(path),
                    Name = Path.GetFileNameWithoutExtension(path)
                };
                openTabs.Add(et);
            } else
            {
                MessageBox.Show("Tento formát není podporován", "Chyba při otevírání", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            editorTabs.SelectedIndex = openTabs.Count - 1;
        }
        public static async Task<Editor?> OpenEditorAsync(string path)
        {
            Editor ed;
            try
            {
                ed = await EditorLoader.CreateFromFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            ed.Changed = false;
            return ed;
        }
        public async Task<MFEditor?> OpenMFEditorAsync(string path)
        {
            MFEditor ed;
            try
            {
                ed = await MFLoader.Load(path, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message + "\n\n" + ex.InnerException?.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return ed;
        }

        public void CanSave(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanSave;
        public void CanExport(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanExport;
        public void CanPrint(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanPrint;
        public void CanAddGuide(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanAddGuide;
        public void CanUndo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanUndo;
        public void CanRedo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanRedo;
        public void CanLayout(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanLayout;
        public void CanExecuteEditorCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanExecuteCommand;

        private void NewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (CreateNewEditor() is not Editor ed) return;
            openTabs.Add(new EditorTab(ed));
            editorTabs.SelectedIndex = openTabs.Count - 1;
            editorCounter++;
        }
        public static Editor? CreateNewEditor()
        {
            NewFileDialog nfd = new();
            nfd.ShowDialog();
            if (nfd.ProjectionType is null) return null;

            Editor ed;

            //Opens the axonometry setup dialog if the user chose axonometry
            if (nfd.ProjectionType == ProjectionType.Axonometry)
            {
                AxonometrySetup axo = new();
                axo.ShowDialog();
                if (axo.Canceled || axo.Axis is null) return null;
                ed = new($"Bez názvu {editorCounter}", nfd.ProjectionType.Value);

                foreach (var item in axo.Axis.Items)
                {
                    item.AddToViewportLayer(ed.viewPort.Layers[1]);
                }

                ed.AxonometryAxes = axo.AxonometryAxes;
                ed.Timeline.AddCommand(axo.Axis);
            }
            else
            {
                ed = new($"Bez názvu {editorCounter}", nfd.ProjectionType.Value);
                ed.AddAxis(ed.viewPort.Layers[1]);
            }

            ed.styleSelector.AddDefaultColors();
            ed.Changed = false;
            return ed;
        }
        private void NewMFCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MFEditor ed;

            ed = new(this);
            
            openTabs.Add(new MFEditorTab(ed));
            editorTabs.SelectedIndex = openTabs.Count - 1;
            editorCounter++;
        }
        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (OpenEditorOpenDialog() is not string path) return;

            OpenFileAsync(path);
        }
        public static string? OpenEditorOpenDialog()
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "Všechny formáty|*.dgproj;*.dgcomp|DegCAD projekt|*.dgproj|DegCAD kompozice|*.dgcomp|Všechny soubory|*.*";
            if (ofd.ShowDialog() != true) return null;
            return ofd.FileName;
        }
        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Save();
        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.SaveAs();
        private void ExportCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            ExportDialog ed = new(ActiveEditor.viewPort);
            ed.ShowDialog();
        }
        private void PrintCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Print();
        private async void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (await CanCloseTab(ActiveTab))
                openTabs.Remove(ActiveTab);
        }
        private void UndoCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Undo();
        private void RedoCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Redo();
        private void AboutClick(object sender, RoutedEventArgs e) => AboutDialog.Open();
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
