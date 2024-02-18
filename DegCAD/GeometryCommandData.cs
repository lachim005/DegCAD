using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DegCAD
{
    /// <summary>
    /// Used to add commands to the commands list and uses its data to create UICommands
    /// and buttons on the toolbar
    /// </summary>
    public class GeometryCommandData
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public Key Key { get; set; }
        public ModifierKeys ModifierKey { get; set; }
        public ExecutedRoutedEventHandler ExecuteHandler { get; set; }
        public string? IconName { get; set; }
        public FileType FileTypes { get; set; }


        public GeometryCommandData(string name, string description, Key key, ModifierKeys modifierKey, ExecutedRoutedEventHandler executeHandler, string? iconName, FileType fileTypes)
        {
            Name = name;
            Description = description;
            Key = key;
            ModifierKey = modifierKey;
            ExecuteHandler = executeHandler;
            IconName = iconName;
            FileTypes = fileTypes;
        }
    }
}
