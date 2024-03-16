using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DegCAD
{
    public class EditorTab : ITab
    {
        public Editor Editor { get; init; }

        public Control Body => Editor;

        public bool CanSave => true;

        public bool CanUndo => Editor.Timeline.CanUndo && !Editor.ExecutingCommand;

        public bool CanRedo => Editor.Timeline.CanRedo && !Editor.ExecutingCommand;

        public bool CanExecuteCommand => !Editor.ExecutingCommand;

        public bool CanPrint => true;

        public bool CanLayout => true;

        public bool CanAddGuide => Editor.Guide is null;

        public bool CanExport => true;

        public bool HasChanges => Editor.Changed;
        public string Name => Editor.FileName;

        public EditorTab(Editor editor)
        {
            Editor = editor;
            editor.PropertyChanged += EditorPropertyChanged;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void EditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Editor.Changed): PropertyChanged?.Invoke(this, new(nameof(HasChanges))); break;
                case nameof(Editor.FileName): PropertyChanged?.Invoke(this, new(nameof(Name))); break;
                case nameof(Editor.ExecutingCommand):
                    PropertyChanged?.Invoke(this, new(nameof(CanExecuteCommand)));
                    PropertyChanged?.Invoke(this, new(nameof(CanUndo)));
                    PropertyChanged?.Invoke(this, new(nameof(CanRedo)));
                    break;
            }
        }

        public async Task<bool> Save()
        {
            if (Editor.FolderPath is null)
            {
                if (!MainWindow.OpenEditorSaveFileDialog(Editor))
                {
                    return false;
                }
            }
            return await MainWindow.SaveEditorAsync(Editor);
        }
        public async Task<bool> SaveAs()
        {
            if (!MainWindow.OpenEditorSaveFileDialog(Editor))
            {
                return false;
            }

            return await MainWindow.SaveEditorAsync(Editor);
        }
        public void Undo() => Editor.Timeline.Undo();
        public void Redo() => Editor.Timeline.Redo();

        public void TabSelected()
        {

        }

        public void ExecuteCommand(ICommand c)
        {
            Editor.ExecuteCommand(c);
        }
        public void SwapWhiteAndBlack()
        {
            Editor.SwapWhiteAndBlack();
        }
        public void Print()
        {
            Dialogs.PrintDialog pd = new(Editor);
            pd.ShowDialog();
        }
    }
}
