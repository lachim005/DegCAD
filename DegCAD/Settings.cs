using DegCAD.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    public static class Settings
    {
        private static bool _darkMode;
        private static bool _defaultMongeXDirectionLeft = true;
        private static int _defaultLabelFontSize = 16;

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

        public static bool DefaultMongeXDirectionLeft
        {
            get => _defaultMongeXDirectionLeft;
            set
            {
                _defaultMongeXDirectionLeft = value;
                PointCoordinateInputDialog.lastAxisDirectionLeft = _defaultMongeXDirectionLeft;
            }
        }

        public static int DefaultLabelFontSize
        {
            get => _defaultLabelFontSize;
            set
            {
                _defaultLabelFontSize = value;
                LabelInput.lastFontSize = _defaultLabelFontSize;
            }
        }

        public static bool AlertGuides { get; set; } = true;
        public static bool AlertNewVersions { get; set; } = true;
        public static bool SnapLabels { get; set; } = true;


        public static void LoadSettings()
        {
            try
            {

#if RELEASE_PORTABLE
                string settingsFolder = System.Reflection.Assembly.GetExecutingAssembly().Location;
#else
                string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DegCAD");
#endif
                string settingsFile = Path.Combine(settingsFolder, "Settings.dgconf");

                if (!File.Exists(settingsFile)) return;

                using StreamReader sr = new(settingsFile);

                string? line;
                while((line = sr.ReadLine()) is not null)
                {
                    var pair = line.Split(':', 2);
                    var key = pair[0];
                    var value = pair[1];
                    switch (key)
                    {
                        case "DarkMode":
                            if (!bool.TryParse(value, out bool dm)) continue;
                            DarkMode = dm;
                            break;
                        case "DefaultMongeXDirectionLeft":
                            if (!bool.TryParse(value, out bool dx)) continue;
                            DefaultMongeXDirectionLeft = dx;
                            break;
                        case "DefaultLabelFontSize":
                            if (!int.TryParse(value, out int fs)) continue;
                            DefaultLabelFontSize = fs;
                            break;
                        case "AlertGuides":
                            if (!bool.TryParse(value, out bool ag)) continue;
                            AlertGuides = ag;
                            break;
                        case "AlertNewVersions":
                            if (!bool.TryParse(value, out bool anv)) continue;
                            AlertNewVersions = anv;
                            break;
                        case "SnapLabels":
                            if (!bool.TryParse(value, out bool sl)) continue;
                            SnapLabels = sl;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání nastavení:\n" + ex.Message, img: MessageBoxImage.Error);
            }
        }

        public static void SaveSettings()
        {
            try
            {

#if RELEASE_PORTABLE
                string settingsFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
#else
                string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DegCAD");
#endif
                if (!Directory.Exists(settingsFolder))
                {
                    Directory.CreateDirectory(settingsFolder);
                }

                string settingsFile = Path.Combine(settingsFolder, "Settings.dgconf");

                using StreamWriter sw = new(settingsFile);

                sw.WriteLine("Version:" + (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0,0,0)));

                sw.WriteLine("DarkMode:" + DarkMode);
                sw.WriteLine("DefaultMongeXDirectionLeft:" + DefaultMongeXDirectionLeft);
                sw.WriteLine("DefaultLabelFontSize:" + DefaultLabelFontSize);
                sw.WriteLine("AlertGuides:" + AlertGuides);
                sw.WriteLine("AlertNewVersions:" + AlertNewVersions);
                sw.WriteLine("SnapLabels:" + SnapLabels);
            } catch (Exception ex)
            {
                MessageBox.Show("Chyba při ukládání nastavení:\n" + ex.Message, img: MessageBoxImage.Error);
            }
        }
    }
}
