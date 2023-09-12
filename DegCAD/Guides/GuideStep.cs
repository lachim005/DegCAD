using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.Guides
{
    public class GuideStep : INotifyPropertyChanged
    {
        string _description = "";
        int _items = 1;
        int _position = -1;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                PropertyChanged?.Invoke(this, new(nameof(Description)));
            }
        }
        public int Items
        {
            get => _items;
            set
            {
                _items = value;
                PropertyChanged?.Invoke(this, new(nameof(Items)));
            }
        }
        public int Position
        {
            get => _position;
            set
            {
                _position = value;
                PropertyChanged?.Invoke(this, new(nameof(Position)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
