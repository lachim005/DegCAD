using DegCAD.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
                if (_darkMode == value) return;

                _darkMode = value;
                if (Application.Current.MainWindow is not MainWindow mw) return;
                mw.ChangeSkin(value ? Skin.Dark : Skin.Light);
                SwapBlackAndWhite();
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
        public static List<Color> DefaultColors { get; set; } =
        [
            Colors.Black,
            Color.FromRgb(153, 153, 153),
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(255, 128, 0),
            Color.FromRgb(242, 203, 12),
            Color.FromRgb(67, 204, 0),
            Color.FromRgb(40, 204, 204),
            Color.FromRgb(0, 169, 255),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(134, 31, 186),
            Color.FromRgb(229, 68, 229)
        ];
        public static bool RepeatCommands { get; set; } = false;
        public static bool NameNewItems { get; set; } = true;

        public static bool AlertGuides { get; set; } = true;
        public static bool AlertNewVersions { get; set; } = true;
        public static bool SnapLabels { get; set; } = true;

        public static RecentFiles RecentFiles { get; private set; } = new();


        public static void LoadSettings()
        {
            CultureInfo currentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {

#if RELEASE_PORTABLE
                string settingsFolder = Path.GetDirectoryName(AppContext.BaseDirectory) ?? ".";
#else
                string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DegCAD");
#endif
                string settingsFile = Path.Combine(settingsFolder, "Settings.dgconf");

                if (!File.Exists(settingsFile)) return;

                using StreamReader sr = new(settingsFile);

                List<RecentFile> recentFiles = new();

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
                        case "DefaultColors":
                            DefaultColors.Clear();
                            while ((line = sr.ReadLine()) is not null && line != "]")
                            {
                                var vals = line.Split(';', 3);
                                if (!byte.TryParse(vals[0], out byte r)) continue;
                                if (!byte.TryParse(vals[1], out byte g)) continue;
                                if (!byte.TryParse(vals[2], out byte b)) continue;

                                var c = Color.FromRgb(r, g, b);
                                if (DarkMode)
                                {
                                    if (c == Colors.White)
                                        c = Colors.Black;
                                    else if (c == Colors.Black)
                                        c = Colors.White;
                                }

                                DefaultColors.Add(c);
                            }
                            break;
                        case "RepeatCommands":
                            if (!bool.TryParse(value, out bool rc)) continue;
                            RepeatCommands = rc;
                            break;
                        case "NameNewItems":
                            if (!bool.TryParse(value, out bool nni)) continue;
                            NameNewItems = nni;
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
                        case "RecentFiles":
                            while ((line = sr.ReadLine()) is not null && line != "]")
                            {
                                var vals = line.Split(';', 3);
                                if (!int.TryParse(vals[0], out int ft)) continue;
                                if (!long.TryParse(vals[1], out long ticks)) continue;
                                recentFiles.Add(new(vals[2], (FileType)ft, new(ticks)) { Visibility = File.Exists(vals[2]) ? Visibility.Visible : Visibility.Collapsed});
                            }
                            break;
                    }
                }

                RecentFiles = new(recentFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání nastavení:\n" + ex.Message, img: MessageBoxImage.Error);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCI;
            }
            RecentFiles.RecentFilesChanged += RecentFilesChanged;
        }

        private static void RecentFilesChanged(object? sender, EventArgs e)
        {
            SaveSettings();
        }

        public static void SaveSettings()
        {
            CultureInfo currentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {

#if RELEASE_PORTABLE
                string settingsFolder = Path.GetDirectoryName(AppContext.BaseDirectory) ?? ".";
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
                sw.WriteLine("DefaultColors:[");
                foreach (var color in DefaultColors)
                {
                    var c = color;
                    if (DarkMode)
                    {
                        if (color == Colors.White)
                            c = Colors.Black;
                        else if (color == Colors.Black)
                            c = Colors.White;
                    }
                    sw.WriteLine($"{c.R};{c.G};{c.B}");
                }
                sw.WriteLine(']');
                sw.WriteLine("RepeatCommands:" + RepeatCommands);
                sw.WriteLine("NameNewItems:" + NameNewItems);
                sw.WriteLine("AlertGuides:" + AlertGuides);
                sw.WriteLine("AlertNewVersions:" + AlertNewVersions);
                sw.WriteLine("SnapLabels:" + SnapLabels);
                sw.WriteLine("RecentFiles:[");
                foreach (var file in RecentFiles.Files)
                {
                    sw.WriteLine($"{(int)file.FileType};{file.TimeOpen.Ticks};{file.Path}");
                }
                sw.WriteLine(']');
            } catch (Exception ex)
            {
                MessageBox.Show("Chyba při ukládání nastavení:\n" + ex.Message, img: MessageBoxImage.Error);
            } finally
            {
                Thread.CurrentThread.CurrentCulture = currentCI;
            }
        }

        private static void SwapBlackAndWhite()
        {
            for (int i = 0; i < DefaultColors.Count; i++)
            {
                if (DefaultColors[i] == Colors.Black)
                    DefaultColors[i] = Colors.White;
                else if (DefaultColors[i] == Colors.White)
                    DefaultColors[i] = Colors.Black;
            }
        }
    }
}
