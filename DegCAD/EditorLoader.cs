﻿using DegCAD.DrawableItems;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Soubror {path} nebyl nalezen");
            }

            var dir = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            if (dir is null || fileName is null) throw new Exception("Špatná cesta");

            //Prepares the path for the temp folder so it doesn't overwrite anything
            string tempDir = Path.Combine(dir, $"temp - {fileName}");
            int tempNumber = 1;
            while (Directory.Exists(tempDir))
            {
                tempDir = Path.Combine(dir, $"temp{tempNumber} - {fileName}");
                tempNumber++;
            }

            //Extract the zip
            try
            {
                ZipFile.ExtractToDirectory(path, tempDir);
            } catch (Exception ex)
            {
                throw new Exception("Soubor nemohl být rozbalen", ex);
            }

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
            Version? version;
            bool hasPalette = false;

            foreach (var line in metaData)
            {
                string[] keyVal = line.Split(':');
                switch (keyVal[0])
                {
                    case "DegCAD version":
                        version = Version.Parse(keyVal[1]);
                        break;
                    case "Palette":
                        hasPalette = bool.Parse(keyVal[1]);
                        break;
                }
            }

            Editor res = new(fileName)
            {
                FolderPath = dir
            };

            //Reads and parses the timeline
            try
            {
                ReadTimeline(res, Path.Combine(tempDir, "timeline.txt"));
            }
            catch (Exception ex)
            {
                Directory.Delete(tempDir, true);
                throw new Exception("Chyba při čtení časové osy", ex);
            }

            //Reads and parses the palette
            try
            {
                if (hasPalette)
                    ReadPalette(res, Path.Combine(tempDir, "palette.txt"));
                else
                    res.styleSelector.AddDefaultColors();
            }
            catch (Exception ex)
            {
                Directory.Delete(tempDir, true);
                throw new Exception("Chyba při čtení palety", ex);
            }

            //Removes the temp directory
            Directory.Delete(tempDir, true);
            return res;
        }

        private static void ReadTimeline(Editor e, string path)
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
                        currentStyle = STL(line[4..]);
                        continue;
                    case "NUL":
                        continue;
                }
                var mItem = ParseMongeItem(line, currentStyle);
                if (mItem is not null) 
                    items.Add(mItem);
            }
        }
        private static void ReadPalette(Editor e, string path)
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

        private static IMongeItem? ParseMongeItem(string s, Style stl)
        {
            var itemName = s[..3];

            return itemName switch
            {
                "AXS" => new Axis(),
                "PNT" => PNT(s[4..], stl),
                "LNE" => LNE(s[4..], stl),
                "SEG" => SEG(s[4..], stl),
                "CIR" => CIR(s[4..], stl),
                "ARC" => ARC(s[4..], stl),
                "LBL" => LBL(s[4..], stl),
                _ => null
            };
        }

        #region Command parsers
        private static Style STL(string s)
        {
            string[] vals = s.Split(' ');
            Color c = Color.FromRgb(
                byte.Parse(vals[0]),
                byte.Parse(vals[1]),
                byte.Parse(vals[2]));
            return new Style()
            {
                Color = c,
                LineStyle = int.Parse(vals[3])
            };
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
}
