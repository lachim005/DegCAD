﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DegCAD.Guides
{
    /// <summary>
    /// Interaction logic for OpenGuideDialog.xaml
    /// </summary>
    public partial class OpenGuideDialog : Window
    {
        public OpenGuideDialog(Window owner)
        {
            Owner = owner;
            InitializeComponent();
        }

        private void OpenEditor(object sender, ExecutedRoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private void OpenGuide(object sender, ExecutedRoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        public bool Result { get; set; } = false;

        public static bool OpenDialog(Window owner)
        {
            OpenGuideDialog ogd = new(owner);
            ogd.ShowDialog();
            return ogd.Result;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (dontShowAnymoreChbx.IsChecked == false) return;
            Settings.AlertGuides = false;
            Settings.SaveSettings();
        }
    }
}
