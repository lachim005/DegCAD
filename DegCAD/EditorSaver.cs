using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DegCAD
{
    public static class EditorSaver
    {
        public static void SaveEditor(this Editor editor)
        {
            //Validate or create the folder that the editor will be saved to
            if (editor.FolderPath is null) throw new Exception("Nebyla dána složka");
            if (!Directory.Exists(editor.FolderPath))
            {
                try
                {
                    Directory.CreateDirectory(editor.FolderPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Složka nemohla být vytvořena", ex);
                }
            }

            var tempDir = CreateTempFolder();
            try
            {
                WriteMetaData(tempDir);
                SaveTimelineToFile(editor.Timeline, Path.Combine(tempDir, "timeline.txt"));
                SavePaletteToFile(editor.styleSelector.ColorPalette, Path.Combine(tempDir, "palette.txt"));

                string outputFile = Path.Combine(editor.FolderPath, $"{editor.FileName}.dgproj");
                Pack(outputFile, tempDir);
            }
            finally
            {
                //Remove the temporary directory
                Directory.Delete(tempDir, true);
                editor.Changed = false;
            }
        }

        private static void WriteMetaData(string tempDir)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            version ??= new(0, 0, 0);

            string[] metaData =
            {
                "DegCAD version:" + version.ToString(),
                "Palette:True"
            };
            File.WriteAllLines(Path.Combine(tempDir, "DEG-CAD-PROJECT.txt"), metaData);
        }

        public static string CreateTempFolder()
        {
            var tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmp);
            return tmp;
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

        private static void SaveTimelineToFile(Timeline tl, string path)
        {
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            //Writes the default style
            Style currentStyle = Style.Default;
            sw.WriteLine(SerializeStyle(currentStyle));
            foreach (var cmd in tl.CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    //If the style has changed, writes it to the file
                    if (currentStyle != item.Style)
                    {
                        sw.WriteLine(SerializeStyle(item.Style));
                        currentStyle = item.Style;
                    }

                    sw.WriteLine(SerializeMongeItem(item));
                }
                //Ends the command
                sw.WriteLine("ADD");
            }
            sw.WriteLine("END");
        }
        private static void SavePaletteToFile(List<System.Windows.Media.Color> palette, string path)
        {
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            //Writes the default style
            foreach (var color in palette)
            {
                sw.WriteLine($"{color.R} {color.G} {color.B}");
            }
        }

        private static string SerializeMongeItem(IMongeItem item)
        {
            return item switch
            {
                Axis => "AXS",

                Point pt => $"PNT {pt.X} {pt.Y}",

                LineProjection ln => $"LNE {ln.Line.Point.X} {ln.Line.Point.Y} {ln.Line.DirectionVector.X} {ln.Line.DirectionVector.Y} {ln.Plane}",

                LineSegment seg => $"SEG {seg.P1.X} {seg.P1.Y} {seg.P2.X} {seg.P2.Y}",

                Circle cir => $"CIR {cir.Circle2.Center.X} {cir.Circle2.Center.Y} {cir.Circle2.Radius}",

                Arc arc => $"ARC {arc.Circle.Center.X} {arc.Circle.Center.Y} {arc.Circle.Radius} {arc.StartAngle} {arc.EndAngle}",

                Ellipse ell => $"ELL {ell.Center.X} {ell.Center.Y} {ell.P1.X} {ell.P1.Y} {ell.P2.X} {ell.P2.Y}",

                Label lbl => $"LBL {lbl.LabelText.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Subscript.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Superscript.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Position.X} {lbl.Position.Y}->{SerializeMongeItem(lbl.LabeledObject)}",

                _ => "NUL"
            };
        }

        private static string SerializeStyle(Style s) => $"STL {s.Color.R} {s.Color.G} {s.Color.B} {s.LineStyle} {s.Thickness}";
    }
}
