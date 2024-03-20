using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    public static class Settings
    {
        private static bool _darkMode;

        public static bool DarkMode
        {
            get => _darkMode;
            set
            {
                _darkMode = value;
                if (Application.Current.MainWindow is not MainWindow mw) return;
                mw.ChangeSkin(value ? Skin.Dark : Skin.Light);
            }
        }
    }
}
