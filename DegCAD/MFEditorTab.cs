using DegCAD.MultiFile;
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

namespace DegCAD
{
    public class MFEditorTab : ITab
    {
        private string _name = "MF";

        public MFEditor Editor { get; init; }
        public Control Body => Editor;

        public bool CanSave => true;

        public bool CanUndo => false;

        public bool CanRedo => false;

        public bool CanExecuteCommand => true;

        public bool CanPrint => false;

        public bool CanLayout => false;

        public bool CanAddGuide => false;

        public bool CanExport => false;

        public bool HasChanges => true;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }
        public string? FolderPath { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MFEditorTab(MFEditor editor)
        {
            Editor = editor;
        }

        public void Redo()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}
