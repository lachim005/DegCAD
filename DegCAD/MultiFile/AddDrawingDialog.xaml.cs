﻿using DegCAD.Dialogs;
using System;
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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for AddDrawingDialog.xaml
    /// </summary>
    public partial class AddDrawingDialog : Window
    {
        public Editor? editor;

        public AddDrawingDialog()
        {
            InitializeComponent();
        }

        private void AddEditor(object sender, RoutedEventArgs e)
        {
            editor = MainWindow.CreateNewEditor();
            Close();
        }

        private async void OpenEditor(object sender, RoutedEventArgs e)
        {
            if (MainWindow.OpenEditorOpenDialog() is not string path)
            {
                Close();
                return;
            }
            if (await MainWindow.OpenEditorAsync(path) is not Editor ed)
            {
                Close();
                return;
            }
            editor = ed;
            Close();
        }

        public static Editor? GetEditor()
        {
            AddDrawingDialog d = new();
            d.ShowDialog();
            return d.editor;
        }
    }
}