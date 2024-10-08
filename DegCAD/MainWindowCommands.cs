﻿using DegCAD.Dialogs;
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
        public static RoutedCommand exportCommand = new();
        public static RoutedCommand duplicateCommand = new();
        public static RoutedCommand settingsCommand = new();
        public static RoutedCommand pageLayoutCommand = new();
        public static RoutedCommand addGuideCommand = new();
        public static RoutedCommand changeGlobalFontSizeCommand = new();
        public static RoutedCommand reorderTimelineCommand = new();
        public static RoutedCommand openDebugMenuCommand = new();
        public static RoutedCommand cancelCommandCommand = new();

        public event EventHandler? CommandCanceled;

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
        public static async Task<bool> SaveEditorAsync(Editor editor, Window owner)
        {
            try
            {
                await editor.SaveEditor();
                if (editor.FolderPath is not null)
                {
                    Settings.RecentFiles.AddFile(
                        Path.Combine(editor.FolderPath, editor.FileName + ".dgproj"),
                        ProjectionTypeToFileType(editor.ProjectionType)
                        );
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při ukládání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
        public async void OpenFileAsync(string path, Window owner, bool switchToTab = true)
        {
            FileType openingFileType = FileType.None;
            if (Path.GetExtension(path) == ".dgproj")
            {
                if (await OpenEditorAsync(path, owner) is not Editor ed) return;
                openTabs.Add(new EditorTab(ed));

                openingFileType = ProjectionTypeToFileType(ed.ProjectionType);
            } else if (Path.GetExtension(path) == ".dgcomp")
            {
                if (await OpenMFEditorAsync(path) is not MFEditor ed) return;
                var et = new MFEditorTab(ed)
                {
                    FolderPath = Path.GetDirectoryName(path),
                    Name = Path.GetFileNameWithoutExtension(path)
                };
                openTabs.Add(et);

                openingFileType = FileType.MultiFile;
            } else
            {
                MessageBox.Show(this, "Tento formát není podporován", "Chyba při otevírání", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (switchToTab)
            {
                editorTabs.SelectedIndex = openTabs.Count - 1;
            }

            Settings.RecentFiles.AddFile(path, openingFileType);
        }
        public static async Task<Editor?> OpenEditorAsync(string path, Window owner)
        {
            Editor ed;
            try
            {
                ed = await EditorLoader.CreateFromFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(this, ex.Message + "\n\n" + ex.InnerException?.Message + "\n\n" + ex.InnerException?.InnerException?.Message, "Chyba při načítání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public void CanExecuteContainerCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanExecuteContainerCommand;
        public void CanPaste(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanPaste;
        public void CanLayout(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanLayout;
        public void CanExecuteEditorCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ActiveTab.CanExecuteCommand;

        private void NewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewFile();
        }
        public void CreateNewFile()
        {
            NewFileDialog nfd = new(this);
            nfd.ShowDialog();
        }
        public static Editor? CreateNewEditor(Window owner, ProjectionType? projection = null)
        {
            ProjectionType proj;
            if (projection is null)
            {
                NewDrawingDialog nfd = new(owner);
                nfd.ShowDialog();
                if (nfd.ProjectionType is null) return null;
                proj = nfd.ProjectionType.Value;
            } else
            {
                proj = projection.Value;
            }

            Editor ed = new($"Bez názvu {editorCounter++}", proj);

            //Opens the axonometry setup dialog if the user chose axonometry
            if (proj == ProjectionType.Axonometry)
            {
                AxonometrySetup axo = new(owner);
                axo.ShowDialog();
                if (axo.Canceled || axo.Axis is null) return null;

                foreach (var item in axo.Axis.Items)
                {
                    if (item is not GeometryElement ge) continue;
                    ge.AddToViewportLayer(ed.viewPort.Layers[1]);
                }

                ed.AxonometryAxes = axo.AxonometryAxes;
                ed.Timeline.AddCommand(axo.Axis);
            }
            else
            {
                ed.AddAxis(ed.viewPort.Layers[1]);
            }

            ed.styleSelector.AddDefaultColors();
            ed.Changed = false;
            return ed;
        }
        public void AddEditor(Editor? ed)
        {
            if (ed is null) return;
            
            openTabs.Add(new EditorTab(ed));
            editorTabs.SelectedIndex = openTabs.Count - 1;
        }
        public void AddComposition(MFEditor ed)
        {
            openTabs.Add(new MFEditorTab(ed) { Name = $"Bez názvu {editorCounter++}" });
            editorTabs.SelectedIndex = openTabs.Count - 1;
        }
        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (OpenEditorOpenDialog() is not string path) return;

            OpenFileAsync(path, this);
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

            ExportDialog ed = new(ActiveEditor.viewPort, this);
            ed.ShowDialog();
        }
        private void PrintCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Print();
        private async void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (await CanCloseTab(ActiveTab))
            {
                ActiveTab.OnTabClosed();
                openTabs.Remove(ActiveTab);
            }
        }
        private void UndoCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Undo();
        private void RedoCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Redo();
        private void CopyCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Copy();
        private void CutCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Cut();
        private void PasteCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Paste();
        private void DuplicateCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Duplicate();
        private void DeleteCommand(object sender, ExecutedRoutedEventArgs e) => ActiveTab.Delete();
        private void SettingsCommand(object sender, ExecutedRoutedEventArgs e) => SettingsWindow.OpenDialog(this);
        private void AboutClick(object sender, RoutedEventArgs e) => AboutDialog.Open(this);
        private void OpenPageLayoutWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            PageLayoutEditorView pl = new(ActiveEditor.viewPort);
            ActiveEditor.ShowView(pl, "Rozložení na papíře", false);
        }
        private void AddGuideCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            ActiveEditor.Guide = new();
            ActiveEditor.OOBEGuide.OpenIfTrue(ref Settings.OOBEState.guide);
        }
        private void ChangeGlobalFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            ChangeGlobalFontSize d = new(ActiveEditor, this);
            d.ShowDialog();
        }
        private void ReorderTimeline(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveEditor is null) return;

            ActiveEditor.ShowView(new ReorderTimelineEditorView(ActiveEditor), "Změnit pořadí prvků", true);
        }
        private void OpenDebugMenu(object sender, ExecutedRoutedEventArgs e)
        {
            DebugMenu.DebugWindow dw = new(this);
            dw.Show();
        }
        private void CancelCommandCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CancelCommand();
        }
        public void CancelCommand()
        {
            CommandCanceled?.Invoke(this, EventArgs.Empty);
        }

        private void OpenHomeTabClick(object sender, RoutedEventArgs e)
        {
            openTabs.Add(new HomeTab(this));
            editorTabs.SelectedIndex = openTabs.Count - 1;
            editorCounter++;
        }

        public static FileType ProjectionTypeToFileType(ProjectionType pt) => pt switch
        {
            ProjectionType.Plane => FileType.Plane,
            ProjectionType.Monge => FileType.Monge,
            ProjectionType.Axonometry => FileType.Axonometry,
            _ => FileType.None
        };
    }
}
