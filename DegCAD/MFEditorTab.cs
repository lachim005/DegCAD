using DegCAD.MultiFile;
using DegCAD.MultiFile.History;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace DegCAD
{
    public class MFEditorTab : ITab
    {
        private string _name = "MF";

        public MFEditor Editor { get; init; }
        public Control Body => Editor;

        public bool CanSave => true;

        public bool CanUndo => Editor.Timeline.CanUndo;

        public bool CanRedo => Editor.Timeline.CanRedo;

        public bool CanExecuteContainerCommand => Editor.SelectedContainer is not null;

        public bool CanPaste => MFClipboard.CopiedContainer is not null;

        public bool CanExecuteCommand => true;

        public bool CanPrint => true;

        public bool CanLayout => false;

        public bool CanAddGuide => false;

        public bool CanExport => false;

        public bool HasChanges => Editor.Changed;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }
        public bool ItalicizeName => false;
        public string Icon => "M .5 0.5 L 15 0.5 L 15 5.5 L .5 5.5 z M .5 9 L 6.5 9 L 6.5 15 L .5 15 z M 9 9 L 15 9 L 15 15 L 9 15 z";

        public string? FolderPath { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void EditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Editor.Changed): PropertyChanged?.Invoke(this, new(nameof(HasChanges))); break;
            }
        }

        public MFEditorTab(MFEditor editor)
        {
            Editor = editor;
            Editor.Tab = this;
            Editor.PropertyChanged += EditorPropertyChanged;
        }

        public void Redo()
        {
            Editor.Timeline.Redo();
        }

        public async Task<bool> Save()
        {
            if (FolderPath is null)
            {
                if (OpenSaveFileDialog() is not string path)
                {
                    return false;
                }
                FolderPath = Path.GetDirectoryName(path) ?? "";
                Name = Path.GetFileNameWithoutExtension(path);
            }
            try
            {
                await Editor.Save(Path.Combine(FolderPath, $"{Name}.dgcomp"));
                Settings.RecentFiles.AddFile(Path.Combine(FolderPath, $"{Name}.dgcomp"), FileType.MultiFile);

                Editor.Changed = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při ukládání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        public async Task<bool> SaveAs()
        {
            if (OpenSaveFileDialog() is not string path)
            {
                return false;
            }
            FolderPath = Path.GetDirectoryName(path) ?? "";
            Name = Path.GetFileNameWithoutExtension(path);

            try
            {
                await Editor.Save(Path.Combine(FolderPath, $"{Name}.dgcomp"));
                Settings.RecentFiles.AddFile(Path.Combine(FolderPath, $"{Name}.dgcomp"), FileType.MultiFile);

                Editor.Changed = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message, "Chyba při ukládání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        public void Undo()
        {
            Editor.Timeline.Undo();
        }

        public void Copy()
        {
            if (Editor.SelectedContainer is null) return;
            MFClipboard.CopiedContainer = Editor.SelectedContainer;
        }
        public void Cut()
        {
            if (Editor.SelectedContainer is null) return;
            MFClipboard.CopiedContainer = Editor.SelectedContainer;
            Editor.DeleteSelected();
        }
        public void Paste()
        {
            if (MFClipboard.CopiedContainer is null) return;
            var clone = MFClipboard.CopiedContainer.Clone(Editor.ActivePage);
            clone.CX += MFClipboard.Offset;
            clone.CY += MFClipboard.Offset;
            MFClipboard.Offset += 5;
            Editor.ActivePage.AddItem(clone);
            Editor.ActivePage.Redraw();
            Editor.Timeline.AddState(new ContainerAddedState(clone));
        }
        public void Duplicate()
        {
            Editor.DuplicateSelected();
        }
        public void Delete()
        {
            Editor.DeleteSelected();
        }

        public void TabSelected()
        {
            Editor.TabSelected();
        }

        public void ExecuteCommand(ICommand c) 
        {
            Editor.ExecuteCommand(c);
        }
        public void SwapWhiteAndBlack()
        {
            Editor.SwapWhiteAndBlack();
        }

        public string? OpenSaveFileDialog()
        {
            SaveFileDialog sfd = new();
            sfd.Filter = "DegCAD kompozice|*.dgcomp|Všechny soubory|*.*";
            sfd.FileName = Name + ".dgcomp";

            if (sfd.ShowDialog() != true) return null;
            return sfd.FileName;
        }
        public void Print()
        {
            Editor.Print();
        }
        public void OnTabClosed()
        {
            TabClosed?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler? TabClosed;
    }
}
