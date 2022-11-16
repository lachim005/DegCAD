﻿using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DegCAD
{
    /// <summary>
    /// Contains the list of geometry commands.
    /// Is used to populate the command pallete and add keyboard shortucts
    /// </summary>
    public static class GeometryCommandsList
    {
        public static List<GeometryCommandData> GetCommands(Action<IGeometryCommand> executeCommand)
        {
            List<GeometryCommandData> commands = new List<GeometryCommandData>();

            //Add commands here
            commands.Add(new("Průměty bodu", "Umístí do půdorysny a nárysny průměty bodu", Key.B, ModifierKeys.Control, (_, _) => executeCommand(new Point3D())));
            commands.Add(new("Vynést body", "Umístí body podle zadaných souřadnic", Key.B, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new PointCoordInput())));
            commands.Add(new("Průměty přímky", "Umístí do půdorysny a nárysny průměty přímky", Key.P, ModifierKeys.Control, (_, _) => executeCommand(new Line3D())));
            commands.Add(new("Úsečka", "Úsečka mezi dvěma body", Key.U, ModifierKeys.Control, (_, _) => executeCommand(new LineSegment())));
            commands.Add(new("Rovnoběžka", "Rovnoběžka na danou přímku", Key.R, ModifierKeys.Control, (_, _) => executeCommand(new ParallelLine())));
            commands.Add(new("Kolmice", "Kolmice na danou přímku", Key.K, ModifierKeys.Control, (_, _) => executeCommand(new PerpendicularLine())));


            return commands;
        }
    }
}
