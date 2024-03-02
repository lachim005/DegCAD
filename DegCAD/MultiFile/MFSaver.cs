using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DegCAD.MultiFile
{
    public static class MFSaver
    {
        public static async Task Save(this MFEditor e, string path)
        {
            var dir = Path.GetDirectoryName(path) ?? "";
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    throw new Exception("Složka nemohla být vytvořena", ex);
                }
            }

            var tempDir = CreateTempFolder();

            CultureInfo currentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                await WriteManifest(Path.Combine(tempDir, "manifest.txt"), e);
                var pagesPath = Path.Combine(tempDir, "pages");
                Directory.CreateDirectory(pagesPath);
                for (int i = 0; i < e.Pages.Count; i++)
                {
                    var pagePath = Path.Combine(pagesPath, i.ToString());
                    Directory.CreateDirectory(pagePath);
                    await SavePage(pagePath, e.Pages[i].Page);
                }
                Pack(path, tempDir);
            }
            finally
            {
                //Remove the temporary directory
                Directory.Delete(tempDir, true);
                Thread.CurrentThread.CurrentCulture = currentCI;
            }
        }

        public static string CreateTempFolder()
        {
            var tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmp);
            return tmp;
        }

        public async static Task WriteManifest(string path, MFEditor e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            version ??= new(0, 0, 0);

            List<string> metaData = new()
            {
                "DegCAD version:" + version.ToString()
            };

            await File.WriteAllLinesAsync(path, metaData);
        }
        public async static Task SavePage(string dir, MFPage page)
        {
            using StreamWriter sw = new(Path.Combine(dir, "page.txt"));
            await sw.WriteLineAsync(page.PaperWidth + "x" + page.PaperHeight);
            int dwgCounter = 0;
            StringBuilder sb = new();

            foreach (var item in page.Items)
            {
                sb.Append($"CON {item.CX} {item.CY} {item.CWidth} {item.CHeight}->");
                if (item.Item is MFText txt)
                {
                    sb.Append($"TXT {txt.Color.R} {txt.Color.G} {txt.Color.B} {txt.TextFontSize} {(int)txt.VAlign} {(int)txt.HAlign} {txt.Bold} {txt.Italic} {txt.Underline} {txt.Strikethrough} {txt.Text.Replace("&", "&&").Replace("\r\n", "&;")}");
                } else if (item.Item is MFDrawing dwg)
                {
                    dwg.editor.FileName = dwgCounter.ToString();
                    dwg.editor.FolderPath = dir;
                    await dwg.editor.SaveEditor();
                    sb.Append($"DWG {dwg.Viewport.OffsetX} {dwg.Viewport.OffsetY} {dwg.UnitSize} {dwg.VisibleItems} {dwg.PositionLocked} {dwgCounter}.dgproj");
                    dwgCounter++;
                }
                await sw.WriteLineAsync(sb.ToString());
                sb.Clear();
            }
        }

        public static void Pack(string outputFile, string tempDir)
        {
            //Deletes the file if it exists so the archive can be created
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
            //Create the output file
            ZipFile.CreateFromDirectory(tempDir, outputFile);
        }
    }
}
