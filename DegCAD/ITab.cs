using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DegCAD
{
    public interface ITab : INotifyPropertyChanged
    {
        Control Body { get; }
        bool CanSave { get; }
        bool CanUndo { get; }
        bool CanRedo { get; }
        bool CanExecuteCommand { get; }
        bool CanPrint { get; }
        bool CanLayout { get; }
        bool CanAddGuide { get; }
        bool CanExport { get; }
        bool HasChanges { get; }
        string Name { get; }
        bool ItalicizeName { get; }
        string Icon { get; }

        Task<bool> Save();
        Task<bool> SaveAs();
        void Undo();
        void Redo();
        void TabSelected();
        void ExecuteCommand(ICommand c);
        void SwapWhiteAndBlack();
        void Print();
        void OnTabClosed(); 
        event EventHandler? TabClosed;
    }
}
