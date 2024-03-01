using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DegCAD
{
    public class HomeTab : ITab
    {
        public Control Body => new Control();

        public bool CanSave => false;

        public bool CanUndo => false;

        public bool CanRedo => false;

        public bool CanExecuteCommand => false;

        public bool CanPrint => false;

        public bool CanLayout => false;

        public bool CanAddGuide => false;

        public bool CanExport => false;

        public bool HasChanges => false;

        public string Name => "Domů";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Redo()
        {
            
        }

        public Task<bool> Save()
        {
            return Task.FromResult(true);
        }

        public Task<bool> SaveAs()
        {
            return Task.FromResult(true);
        }

        public void Undo()
        {
            
        }

        public void TabSelected()
        {

        }
        public void ExecuteCommand(ICommand c)
        {

        }
        public void SwapWhiteAndBlack()
        {

        }
    }
}
