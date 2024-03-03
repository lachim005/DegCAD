using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.Threading;

namespace DegCAD.MultiFile
{
    public static class MFLoader
    {
        public static async Task<MFEditor> Load(string path, MainWindow mw)
        {
            // Unpack
            string tempDir;
            try
            {
                tempDir = Unpack(path);
            } catch (Exception ex)
            {
                throw new Exception("Chyba při rozbalování souboru", ex);
            }
            var currentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            

            try
            {
                // Read manifest
                FileManifest mf;
                try
                {
                    mf = await ReadManifest(Path.Combine(tempDir, "manifest.txt"));
                }
                catch (Exception ex)
                {
                    throw new Exception("Chyba při čtení manifestu", ex);
                }

                var pagesDir = Path.Combine(tempDir, "pages");
                if (!Directory.Exists(pagesDir)) throw new Exception("Chyba formátu - nebyla nalezena složka se stránkami");

                List<MFPage> pages = new List<MFPage>();

                var pageDirs = Directory.GetDirectories(pagesDir);
                foreach (var pageDir in pageDirs)
                {
                    try
                    {
                        var page = await ReadPage(pageDir);
                        pages.Add(page);
                    }
                    catch (Exception ex)
                    {
                        throw new("Chyba při čtení stránky", ex);
                    }
                }

                Directory.Delete(tempDir, true);
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                return new(mw, pages);
            }
            catch (Exception ex)
            {
                Directory.Delete(tempDir, true);
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                throw new Exception("Chyba při čtení souboru", ex);
            }
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

        public static async Task<FileManifest> ReadManifest(string path)
        {
            using StreamReader sr = new(path);
            string? line;

            FileManifest m = new();

            while (!string.IsNullOrWhiteSpace((line = await sr.ReadLineAsync())))
            {
                string[] keyVal = line.Split(':');
                switch (keyVal[0])
                {
                    case "DegCAD version":
                        m.version = Version.Parse(keyVal[1]);
                        break;
                }
            }

            return m;
        }

        public static async Task<MFPage> ReadPage(string folder)
        {
            using StreamReader sr = new(Path.Combine(folder, "page.txt"));
            double w, h;
            var sizeStr = await sr.ReadLineAsync() ?? throw new Exception("Soubor page.txt je prázdný");
            var size = sizeStr.Split('x');
            w = double.Parse(size[0]);
            h = double.Parse(size[1]);

            MFPage page = new() { PaperWidth = w, PaperHeight = h };
            string? line;
            while (!string.IsNullOrWhiteSpace((line = await sr.ReadLineAsync())))
            {
                var halves = line.Split("->", 2);
                var contVals = halves[0].Split(" ");
                var cx = double.Parse(contVals[1]);
                var cy = double.Parse(contVals[2]);
                var cw = double.Parse(contVals[3]);
                var ch = double.Parse(contVals[4]);
                var itemVals = halves[1].Split(" ", 2);

                MFItem? item = itemVals[0] switch
                {
                    "TXT" => ParseText(itemVals[1]),
                    "DWG" => await ReadDrawing(itemVals[1], folder),
                    _ => null
                };

                if (item is null) continue;
                page.AddItem(new(page, item) { CX = cx, CY = cy, CWidth = cw, CHeight = ch});
            }

            return page;
        }

        public static MFText ParseText(string str)
        {
            var vals = str.Split(' ', 11);

            // Parses text
            StringBuilder textSb = new();
            for (int i = 0; i < vals[10].Length; i++)
            {
                if (vals[10][i] != '&')
                {
                    textSb.Append(vals[10][i]);
                    continue;
                }
                i++;
                switch (vals[10][i])
                {
                    case '&':
                        textSb.Append('&');
                        break;
                    case ';':
                        textSb.Append("\r\n");
                        break;
                }
            }


            return new(textSb.ToString())
            {
                Color = Color.FromRgb(byte.Parse(vals[0]), byte.Parse(vals[1]), byte.Parse(vals[2])),
                TextFontSize = int.Parse(vals[3]),
                VAlign = (VerticalAlignment)int.Parse(vals[4]),
                HAlign = (TextAlignment)int.Parse(vals[5]),
                Bold = bool.Parse(vals[6]),
                Italic = bool.Parse(vals[7]),
                Underline = bool.Parse(vals[8]),
                Strikethrough = bool.Parse(vals[9])
            };
        }

        public static async Task<MFDrawing> ReadDrawing(string str, string folder)
        {
            var vals = str.Split(' ');

            var ed = await EditorLoader.CreateFromFile(Path.Combine(folder, vals[5]));
            MFDrawing dwg = new(ed)
            {
                UnitSize = double.Parse(vals[2]),
                VisibleItems = int.Parse(vals[3]),
                PositionLocked = bool.Parse(vals[4])
            };
            dwg.Viewport.OffsetX = double.Parse(vals[0]);
            dwg.Viewport.OffsetY = double.Parse(vals[1]);

            return dwg;
        }
    }

    public record FileManifest
    {
        public Version version = new(0, 0, 0);
    }
}
