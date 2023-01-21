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
            if (version is null) version = new(0, 0, 0);

            string[] metaData =
            {
                "DegCAD version:" + version.ToString(),
            };
            File.WriteAllLines(Path.Combine(tempDir, "DEG-CAD-PROJECT.txt"), metaData);

            SaveTimelineToFile(editor.Timeline, Path.Combine(tempDir, "timeline.txt"));

            string outputFile = Path.Combine(editor.FolderPath, $"{editor.FileName}.zip");
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

        }
    }
}
