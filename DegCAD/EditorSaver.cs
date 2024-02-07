using DegCAD.Guides;
using DegCAD.MongeItems;
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
using Colors = System.Windows.Media.Colors;

namespace DegCAD
{
    public static class EditorSaver
    {
        public static async Task SaveEditor(this Editor editor)
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

            CultureInfo currentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                WriteMetaData(tempDir, editor);
                await SaveTimelineToFile(editor.Timeline, Path.Combine(tempDir, "timeline.txt"));
                SavePaletteToFile(editor.styleSelector.ColorPalette, Path.Combine(tempDir, "palette.txt"));

                if (editor.AxonometryAxes is not null) SaveAxonometryAxes(editor.AxonometryAxes, Path.Combine(tempDir, "axonometry.txt"));
                if (editor.Guide is not null) SaveGuideToFile(editor.Guide, Path.Combine(tempDir, "guide.txt"));

                string outputFile = Path.Combine(editor.FolderPath, $"{editor.FileName}.dgproj");
                Pack(outputFile, tempDir);
            }
            finally
            {
                //Remove the temporary directory
                Directory.Delete(tempDir, true);
                editor.Changed = false;
                Thread.CurrentThread.CurrentCulture = currentCI;
            }
        }

        private static void WriteMetaData(string tempDir, Editor e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            version ??= new(0, 0, 0);

            List<string> metaData = new()
            {
                "DegCAD version:" + version.ToString(),
                "Palette:True",
                "Projection:" + e.ProjectionType.ToString(),
            };

            if (e.Guide is not null) metaData.Add("Guide:True");


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

        private static async Task SaveTimelineToFile(Timeline tl, string path)
        {
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            //Writes the default style
            Style currentStyle = Style.Default;
            await sw.WriteLineAsync(SerializeStyle(currentStyle));
            foreach (var cmd in tl.CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    //If the style has changed, writes it to the file
                    if (currentStyle != item.Style)
                    {
                        await sw.WriteLineAsync(SerializeStyle(item.Style));
                        currentStyle = item.Style;
                    }

                    await sw.WriteLineAsync(SerializeMongeItem(item));
                }
                //Ends the command
                await sw.WriteLineAsync("ADD");
            }
            await sw.WriteLineAsync("END");
        }
        private static void SavePaletteToFile(List<System.Windows.Media.Color> palette, string path)
        {
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            //Writes the default style
            foreach (var color in palette)
            {
                var c = color;
                if (App.Skin == Skin.Dark)
                {
                    if (c == Colors.Black) c = Colors.White;
                    else if (c == Colors.White) c = Colors.Black;
                }
                sw.WriteLine($"{c.R} {c.G} {c.B}");
            }
        }
        private static void SaveGuideToFile(Guide guide, string path)
        {
            using StreamWriter sw = new(path, false, Encoding.UTF8);

            foreach (var step in guide.Steps)
            {
                sw.WriteLine($"{step.Items} {step.Description.Replace("&", "&&").Replace("\r\n", "&;")}");
            }
        }

        private static void SaveAxonometryAxes(AxonometryAxes aa, string path)
        {
            using StreamWriter sw = new(path);
            sw.WriteLine(aa.XAxis.X.ToString());
            sw.WriteLine(aa.XAxis.Y.ToString());
            sw.WriteLine(aa.YAxis.X.ToString());
            sw.WriteLine(aa.YAxis.Y.ToString());
            sw.WriteLine(aa.ZAxis.X.ToString());
            sw.WriteLine(aa.ZAxis.Y.ToString());
        }

        private static string SerializeMongeItem(IMongeItem item)
        {
            return item switch
            {
                Axis => "AXS",

                Point pt => $"PNT {pt.X} {pt.Y}",

                LineProjection ln => $"LNE {ln.Line.Point.X} {ln.Line.Point.Y} {ln.Line.DirectionVector.X} {ln.Line.DirectionVector.Y} {ln.Plane}",

                InfiniteLine iln => $"ILN {iln.Line.Point.X} {iln.Line.Point.Y} {iln.Line.DirectionVector.X} {iln.Line.DirectionVector.Y}",

                HalfLine hln => $"HLN {hln.StartPoint.X} {hln.StartPoint.Y} {hln.Direction.X} {hln.Direction.Y}",

                LineSegment seg => $"SEG {seg.P1.X} {seg.P1.Y} {seg.P2.X} {seg.P2.Y}",

                Circle cir => $"CIR {cir.Circle2.Center.X} {cir.Circle2.Center.Y} {cir.Circle2.Radius}",

                Arc arc => $"ARC {arc.Circle.Center.X} {arc.Circle.Center.Y} {arc.Circle.Radius} {arc.StartAngle} {arc.EndAngle}",

                Ellipse ell => $"ELL {ell.Center.X} {ell.Center.Y} {ell.P1.X} {ell.P1.Y} {ell.P2.X} {ell.P2.Y}",

                Parabola pbl => $"PBL {pbl.Focus.X} {pbl.Focus.Y} {pbl.Vertex.X} {pbl.Vertex.Y} {pbl.Infinite} {pbl.End.X} {pbl.End.Y}",

                Hyperbola hbl => $"HBL {hbl.Center.X} {hbl.Center.Y} {hbl.Vertex.X} {hbl.Vertex.Y} {hbl.Point.X} {hbl.Point.Y}",

                Label lbl => $"LBL {lbl.LabelText.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Subscript.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Superscript.Replace("\\", "\\\\").Replace(" ", "\\ ")} " +
                $"{lbl.Position.X} {lbl.Position.Y} {lbl.FontSize}" +
                $"->{SerializeMongeItem(lbl.LabeledObject)}",

                HideModification hid => $"HID {hid.CmdIndex} {hid.ItemIndex}",

                _ => "NUL"
            };
        }

        private static string SerializeStyle(Style s)
        {
            if (App.Skin == Skin.Dark)
            {
                if (s.Color == Colors.Black) s.Color = Colors.White;
                else if (s.Color == Colors.White) s.Color = Colors.Black;
            }

            return $"STL {s.Color.R} {s.Color.G} {s.Color.B} {s.LineStyle} {s.Thickness}";
        }
    }
}
