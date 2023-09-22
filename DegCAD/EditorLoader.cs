using DegCAD.Guides;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LineSegment = DegCAD.MongeItems.LineSegment;

namespace DegCAD
{
    public static class EditorLoader
    {
        public static Editor CreateFromFile(string path)
        {
            var tempDir = Unpack(path);
            var metadata = ReadMetadata(tempDir);

            Version curVer = Assembly.GetExecutingAssembly().GetName().Version ?? new(0, 0, 0);
            if (curVer.Major < metadata.version.Major || curVer.Minor < metadata.version.Minor)
            {
                System.Windows.MessageBox.Show(
                    "Tento soubor byl vytvořen v novější verzi DegCADu. Může dojít k nesprávnému přečtení.",
                    "Varování",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                    );
            }

            Editor res = new(Path.GetFileNameWithoutExtension(path))
            {
                FolderPath = Path.GetDirectoryName(path)
            };

            //Reads and parses the timeline
            try
            {
                ReadTimeline(res, Path.Combine(tempDir, "timeline.txt"), metadata);
            }
            catch (Exception ex)
            {
                Directory.Delete(tempDir, true);
                throw new Exception("Chyba při čtení časové osy", ex);
            }

            //Reads and parses the palette
            try
            {
                if (metadata.hasPalette)
                    ReadPalette(res, Path.Combine(tempDir, "palette.txt"));
                else
                    res.styleSelector.AddDefaultColors();
            }
            catch (Exception ex)
            {
                Directory.Delete(tempDir, true);
                throw new Exception("Chyba při čtení palety", ex);
            }

            //Reads and parses the guide
            if (metadata.hasGuide)
                try
                {
                    ReadGuide(res, Path.Combine(tempDir, "guide.txt"));
                    res.PromptGuide();
                }
                catch (Exception ex)
                {
                    Directory.Delete(tempDir, true);
                    throw new Exception("Chyba při čtení návodu", ex);
                }

            //Removes the temp directory
            Directory.Delete(tempDir, true);
            return res;
        }

        public static Metadata ReadMetadata(string tempDir)
        {
            //Read the metadata
            string[] metaData;
            try
            {
                metaData = File.ReadAllLines(Path.Combine(tempDir, "DEG-CAD-PROJECT.txt"));
            }
            catch (Exception ex)
            {
                Directory.Delete(tempDir, true);
                throw new Exception("Soubor DEG-CAD-PROJECT nemohl být přečten", ex);
            }

            //Parse the metadata
            Metadata md = new();

            foreach (var line in metaData)
            {
                string[] keyVal = line.Split(':');
                switch (keyVal[0])
                {
                    case "DegCAD version":
                        md.version = Version.Parse(keyVal[1]);
                        break;
                    case "Palette":
                        md.hasPalette = bool.Parse(keyVal[1]);
                        break;
                    case "Guide":
                        md.hasGuide = bool.Parse(keyVal[1]);
                        break;
                }
            }

            return md;
        }

        public static string Unpack(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Soubror {path} nebyl nalezen");
            }

            string tempDir = EditorSaver.CreateTempFolder();

            //Extract the zip
            try
            {
                ZipFile.ExtractToDirectory(path, tempDir);
            }
            catch (Exception ex)
            {
                throw new Exception("Soubor nemohl být rozbalen", ex);
            }

            return tempDir;
        }

        public static void ReadTimeline(Editor e, string path, Metadata md)
        {
            using StreamReader sr = new(path);

            string? line;
            List<IMongeItem> items = new();
            Style currentStyle = Style.Default;
            while((line = sr.ReadLine()) is not null)
            {
                var cmdName = line[..3];
                switch (cmdName)
                {
                    case "ADD":
                        e.Timeline.AddCommand(new(items.ToArray()));
                        items = new();
                        continue;
                    case "END":
                        return;
                    case "STL":
                        currentStyle = STL(line[4..], md);
                        continue;
                    case "NUL":
                        continue;
                }
                var mItem = ParseMongeItem(line, currentStyle);
                if (mItem is not null)
                {
                    mItem.AddToViewportLayer(e.viewPort.Layers[1]);
                    items.Add(mItem);
                }
            }
        }
        public static void ReadPalette(Editor e, string path)
        {
            using StreamReader sr = new(path);
            string? line;
            while((line = sr.ReadLine()) is not null)
            {
                string[] cols = line.Split(' ');
                if (!byte.TryParse(cols[0], out var r) || !byte.TryParse(cols[1], out var g) || !byte.TryParse(cols[2], out var b))
                    throw new Exception($"Chyba při čtení barvy \"{line}\"");
                var col = Color.FromRgb(r, g, b);
                e.styleSelector.ColorPalette.Add(col);
            }
            e.styleSelector.UpdateColorPalette();
        }
        public static void ReadGuide(Editor e, string path)
        {
            using StreamReader sr = new(path);
            string? line;

            Guide guide = new();
            int stepCounter = 0;

            while ((line = sr.ReadLine()) is not null)
            {
                string[] halves = line.Split(' ', 2);

                GuideStep step = new();
                step.Position = ++stepCounter;
                step.Items = int.Parse(halves[0]);

                StringBuilder descSb = new();
                for (int i = 0; i < halves[1].Length; i++)
                {
                    if (halves[1][i] != '&')
                    {
                        descSb.Append(halves[1][i]);
                        continue;
                    }
                    i++;
                    switch (halves[1][i])
                    {
                        case '&':
                            descSb.Append("&");
                            break;
                        case ';':
                            descSb.Append("\r\n");
                            break;
                    }
                }
                step.Description = descSb.ToString();

                guide.Steps.Add(step);
            }

            e.Guide = guide;
        }

        private static IMongeItem? ParseMongeItem(string s, Style stl)
        {
            var itemName = s[..3];

            return itemName switch
            {
                "AXS" => new Axis(stl),
                "PNT" => PNT(s[4..], stl),
                "LNE" => LNE(s[4..], stl),
                "SEG" => SEG(s[4..], stl),
                "CIR" => CIR(s[4..], stl),
                "ARC" => ARC(s[4..], stl),
                "ELL" => ELL(s[4..], stl),
                "PBL" => PBL(s[4..], stl),
                "HBL" => HBL(s[4..], stl),
                "LBL" => LBL(s[4..], stl),
                _ => null
            };
        }

        #region Command parsers
        private static Style STL(string s, Metadata md)
        {
            string[] vals = s.Split(' ');
            Color c = Color.FromRgb(
                byte.Parse(vals[0]),
                byte.Parse(vals[1]),
                byte.Parse(vals[2]));
            var stl = new Style()
            {
                Color = c,
                LineStyle = int.Parse(vals[3])
            };
            if (md.version >= new Version(0,4,0))
            {
                stl.Thickness = int.Parse(vals[4]);
            }

            return stl;
        }
        private static Point PNT(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                double.Parse(vals[0]),
                double.Parse(vals[1]),
                stl
            );
        }
        private static LineProjection LNE(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                new(
                    (double.Parse(vals[0]), double.Parse(vals[1])),
                    (double.Parse(vals[2]), double.Parse(vals[3]))
                ),
                bool.Parse(vals[4]),
                stl
            );
        }
        private static LineSegment SEG(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                (double.Parse(vals[0]), double.Parse(vals[1])),
                (double.Parse(vals[2]), double.Parse(vals[3])),
                stl
            );
        }
        private static Circle CIR(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                new(
                    (double.Parse(vals[0]), double.Parse(vals[1])),
                    double.Parse(vals[2])
                ),
                stl
            );
        }
        private static Arc ARC(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                new(
                    (double.Parse(vals[0]), double.Parse(vals[1])),
                    double.Parse(vals[2])
                ),
                double.Parse(vals[3]),
                double.Parse(vals[4]),
                stl
            );
        }
        private static Ellipse ELL(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                (double.Parse(vals[0]), double.Parse(vals[1])),
                (double.Parse(vals[2]), double.Parse(vals[3])),
                (double.Parse(vals[4]), double.Parse(vals[5])),
                stl
            );
        }
        private static Parabola PBL(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            if (bool.Parse(vals[4]))
                return new(
                    (double.Parse(vals[0]), double.Parse(vals[1])),
                    (double.Parse(vals[2]), double.Parse(vals[3])),
                    stl
                );
            return new(
                (double.Parse(vals[0]), double.Parse(vals[1])),
                (double.Parse(vals[2]), double.Parse(vals[3])),
                (double.Parse(vals[5]), double.Parse(vals[6])),
                stl
            );
        }
        private static Hyperbola HBL(string s, Style stl)
        {
            string[] vals = s.Split(' ');
            return new(
                (double.Parse(vals[2]), double.Parse(vals[3])),
                (double.Parse(vals[0]), double.Parse(vals[1])),
                (double.Parse(vals[4]), double.Parse(vals[5])),
                stl
            );
        }
        private static Label LBL(string s, Style stl)
        {
            string[] halves = s.Split("->", 2);
            string[] args = halves[0].SplitWithEscapeChar(' ', '\\').ToArray();
            IMongeItem? labeledItem = ParseMongeItem(halves[1], stl);
            if (labeledItem is null)
            {
                throw new Exception("Štítek nemá co označit");
            }
            return new(
                args[0],
                args[1],
                args[2],
                (double.Parse(args[3]), double.Parse(args[4])),
                stl,
                labeledItem
            );
        } 
        #endregion

        /// <summary>
        /// Splits a string with an escape character
        /// </summary>
        public static IEnumerable<string> SplitWithEscapeChar(this string s, char separator, char escapeChar)
        {
            StringBuilder sb = new();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == escapeChar)
                {
                    i++;
                    sb.Append(s[i]);
                    continue;
                }
                if (s[i] == separator)
                {
                    yield return sb.ToString();
                    sb.Clear();
                    continue;
                }
                sb.Append(s[i]);
            }
            yield return sb.ToString();
        }
    }

    public record Metadata
    {
        public Version version = new Version(0,0,0);
        public bool hasPalette = false;
        public bool hasGuide = false;
    }
}
