using DegCAD.MultiFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DegCAD
{
    public class MFEditorTab : ITab
    {
        public MFEditor Editor { get; init; }
        public Control Body => Editor;

        public bool CanSave => false;

        public bool CanUndo => false;

        public bool CanRedo => false;

        public bool CanExecuteCommand => false;

        public bool CanPrint => false;

        public bool CanLayout => false;

        public bool CanAddGuide => false;

        public bool CanExport => false;

        public bool HasChanges => false;

        public string Name => "MF";

        public event PropertyChangedEventHandler? PropertyChanged;

        public MFEditorTab(MFEditor editor)
        {
            Editor = editor;
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAs()
        {
            throw new NotImplementedException();
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
    }
}
