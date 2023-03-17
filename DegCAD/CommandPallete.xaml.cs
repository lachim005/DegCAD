﻿using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for CommandPallete.xaml
    /// </summary>
    public partial class CommandPallete : UserControl
    {
        MainWindow? mw;

        public CommandPallete()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Generates commands from the command list and 
        /// </summary>
        public void GenerateCommands(MainWindow mw)
        {
            Action<IGeometryCommand> executeCommand = (c) => mw.ActiveEditor?.ExecuteCommand(c);
            this.mw = mw;

            AddCommand(new("Průměty bodu", "Umístí do půdorysny a nárysny průměty bodu", Key.B, ModifierKeys.Control, (_, _) => executeCommand(new Point3D()), "cmdPointProjectionsIcon"));
            AddCommand(new("Vynést body", "Umístí body podle zadaných souřadnic", Key.B, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new PointCoordInput()), "cmdPointInputIcon"));
            AddCommand(new("Průměty přímky", "Umístí do půdorysny a nárysny průměty přímky", Key.P, ModifierKeys.Control, (_, _) => executeCommand(new Line3D()), "cmdLineProjectionsIcon"));
            AddCommand(new("Úsečka", "Úsečka mezi dvěma body", Key.U, ModifierKeys.Control, (_, _) => executeCommand(new LineSegment()), "cmdSegmentIcon"));
            AddCommand(new("Rovnoběžka", "Rovnoběžka na danou přímku", Key.R, ModifierKeys.Control, (_, _) => executeCommand(new ParallelLine()), "cmdParallelLineIcon"));
            AddCommand(new("Kolmice", "Kolmice na danou přímku", Key.K, ModifierKeys.Control, (_, _) => executeCommand(new PerpendicularLine()), "cmdPerpendicularLineIcon"));
            AddCommand(new("Střed", "Střed mezi dvěma body", Key.S, ModifierKeys.Control, (_, _) => executeCommand(new Middle()), "cmdMiddleIcon"));
            AddCommand(new("Přenést vzdálenost", "Přenese vzdálenost mezi dvěma body", Key.V, ModifierKeys.Control, (_, _) => executeCommand(new TranslateSize()), "cmdTranslateSizeIcon"));
            AddCommand(new("Sklopit bod", "Sklopí bod pomocí jeho druhého průmětu", Key.S, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new CastPoint()), "cmdCastPointIcon"));
            AddCommand(new("Kružnice", "Kružnice dána středem a bodem na ní", Key.O, ModifierKeys.Control, (_, _) => executeCommand(new Circle()), "cmdCircleIcon"));
            AddCommand(new("Oblouk", "Oblouk dán středem, bodem na kružnici a dvěma úhly", Key.O, ModifierKeys.Control | ModifierKeys.Shift, (_, _) => executeCommand(new Arc()), "cmdArcIcon"));

        }

        public void AddCommand(GeometryCommandData cmd)
        {
            //Creates the RoutedUICommand to work with keyboard shortcuts
            RoutedCommand rcmd = new RoutedUICommand(
                cmd.Name,
                cmd.Description,
                typeof(MainWindow),
                new InputGestureCollection() { new KeyGesture(cmd.Key, cmd.ModifierKey)
            });

            //Binds the new command to the main window. Can execute if active editor isn't null
            mw?.CommandBindings.Add(new(rcmd, cmd.ExecuteHandler, mw.CanExecuteEditorCommand));

            //Adds a button for every command to the toolbar
            StackPanel stp = new();
            if (cmd.IconName is not null)
                stp.Children.Add(new ContentControl()
                {
                    Content = FindResource(cmd.IconName),
                    Width = 60,
                    Height = 60
                });
            stp.Children.Add(new TextBlock() { 
                Text = cmd.Name, 
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
            });
            Button cmdButton = new() { Width = 70, Content = stp };
            cmdButtons.Children.Add(cmdButton);
        }
    }
}
