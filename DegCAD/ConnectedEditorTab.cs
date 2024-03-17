using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DegCAD
{
    public class ConnectedEditorTab : ITab
    {
        public Editor Editor { get; init; }

        public Control Body => Editor;

        public bool CanSave => false;

        public bool CanUndo => Editor.Timeline.CanUndo && !Editor.ExecutingCommand;

        public bool CanRedo => Editor.Timeline.CanRedo && !Editor.ExecutingCommand;

        public bool CanExecuteCommand => !Editor.ExecutingCommand;

        public bool CanPrint => true;

        public bool CanLayout => true;

        public bool CanAddGuide => Editor.Guide is null;

        public bool CanExport => true;

        public bool HasChanges => false;
        public string Name => "Propojený editor"; 
        public string Icon => Editor.ProjectionType switch
        {
            ProjectionType.Plane => "M 0 7.5 L 15 7.5 M 7.5 0 L 7.5 15",
            ProjectionType.Monge => "M 0 7.5 L 15 7.5 M 12 7.5 L 4 0 M 4 7.5 L 12 15",
            ProjectionType.Axonometry => "M 0 15 L 7.5 10 L 15 15 M 7.5 10 L 7.5 0",
            _ => ""
        };

        public ConnectedEditorTab(Editor editor)
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

        public Task<bool> Save()
        {
            return Task.FromResult(true);
        }
        public Task<bool> SaveAs()
        {
            return Task.FromResult(true);
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
            
        }
        public void Print()
        {
            Dialogs.PrintDialog pd = new(Editor);
            pd.ShowDialog();
        }
    }
}
