﻿using System;
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
        public Control Body { get; init; }

        public bool CanSave => false;

        public bool CanUndo => false;

        public bool CanRedo => false;

        public bool CanExecuteContainerCommand => false;

        public bool CanPaste => false;

        public bool CanExecuteCommand => false;

        public bool CanPrint => false;

        public bool CanLayout => false;

        public bool CanAddGuide => false;

        public bool CanExport => false;

        public bool HasChanges => false;

        public string Name => "Domů";
        public bool ItalicizeName => false;
        public string Icon => "M 6.5 10 L 6.5 15 L 2.5 15 L 2.5 7.5 L 0 7.5 L 7.5 0 15 7.5 L 12.5 7.5 L 12.5 15 L 8.5 15 L 8.5 10 z";


#pragma warning disable CS0067 // The event 'HomeTab.PropertyChanged' is never used
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067 // The event 'HomeTab.PropertyChanged' is never used

        public HomeTab(MainWindow mw)
        {
            Body = new HomeScreen(mw);
        }
        public void Redo()
        {
            
        }

        public void Copy() { }
        public void Cut() { }
        public void Paste() { }
        public void Duplicate() { }
        public void Delete() { }

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
        public void Print()
        {

        }
        public void OnTabClosed()
        {
            TabClosed?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler? TabClosed;
    }
}
