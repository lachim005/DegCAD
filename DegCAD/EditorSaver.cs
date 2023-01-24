using DegCAD.DrawableItems;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            //Prepares the path for the temp folder so it doesn't overwrite anything
            string tempDir = Path.Combine(editor.FolderPath, $"temp - {editor.FileName}");
            int tempNumber = 1;
            while (Directory.Exists(tempDir))
            {
                tempDir = Path.Combine(editor.FolderPath, $"temp{tempNumber} - {editor.FileName}");
                tempNumber++;
            }

            //Create a temporary folder that will get archived
            try
            {
                Directory.CreateDirectory(tempDir);
            }
            catch (Exception ex)
            {
                throw new Exception("Nemohla být vytvořena dočasná složka", ex);
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            version ??= new(0, 0, 0);

            string[] metaData =
            {
                "DegCAD version:" + version.ToString(),
            };
            File.WriteAllLines(Path.Combine(tempDir, "DEG-CAD-PROJECT.txt"), metaData);

            SaveTimelineToFile(editor.Timeline, Path.Combine(tempDir, "timeline.txt"));

            string outputFile = Path.Combine(editor.FolderPath, $"{editor.FileName}.dgproj");
            //Deletes the file if it exists so the archive can be created
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
            //Create the output file
            ZipFile.CreateFromDirectory(tempDir, outputFile);
            //Remove the temporary directory
            Directory.Delete(tempDir, true);
        }

        private static void SaveTimelineToFile(Timeline tl, string path)
        {
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            //Writes the default style
            Style currentStyle = Style.Default;
            sw.WriteLine(SerializeStyle(currentStyle));
            foreach (var cmd in tl.CommandHistory.ToArray().Reverse())
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

                Label lbl => $"LBL {lbl.LabelText.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Subscript.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Superscript.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Position.X} {lbl.Position.Y}->{SerializeMongeItem(lbl.LabeledObject)}",

                _ => "NUL"
            };
        }

        private static string SerializeStyle(Style s) => $"STL {s.Color.R} {s.Color.G} {s.Color.B} {s.LineStyle}";
    }
}
