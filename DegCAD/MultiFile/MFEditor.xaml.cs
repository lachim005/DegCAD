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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFEditor.xaml
    /// </summary>
    public partial class MFEditor : UserControl
    {
        public MFPage ActivePage { get; set; }
        public MFContainer? SelectedContainer { get; set; }
        public MainWindow MainWindow { get; init; }


        public MFEditor(MainWindow mw)
        {
            InitializeComponent();

            ActivePage = new MFPage();
            pageBorder.Child = ActivePage;
            ActivePage.SelectionChanged += PageSelectionChanged;
            ActivePage.ContainerUpdated += ContainerUpdated;
            MainWindow = mw;
        }

        private void ContainerUpdated(object? sender, TransformChange e)
        {
            var cont = SelectedContainer;
            SelectedContainer = null;
            insTrX.Text = e.endX.ToString();
            insTrY.Text = e.endY.ToString();
            insTrW.Text = e.endW.ToString();
            insTrH.Text = e.endH.ToString();
            SelectedContainer = cont;
        }

        private void PageSelectionChanged(object? sender, MFContainer? e)
        {
            insTransform.Visibility = Visibility.Collapsed;
            insDrawing.Visibility = Visibility.Collapsed;
            insText.Visibility = Visibility.Collapsed;
            SelectedContainer = null;
            if (e is null)
            {
                return;
            }
            insTransform.Visibility = Visibility.Visible;
            insTrX.Text = e.CX.ToString();
            insTrY.Text = e.CY.ToString();
            insTrW.Text = e.CWidth.ToString();
            insTrH.Text = e.CHeight.ToString();


            if (e.Item is MFDrawing dr)
            {
                insDrawing.Visibility = Visibility.Visible;
                insText.Visibility = Visibility.Hidden;
                insDwItems.Maximum = dr.editor.Timeline.CommandHistory.Count;
                insDwItems.Value = dr.VisibleItems;
                insDwUnitSize.Text = dr.UnitSize.ToString();
            }

            SelectedContainer = e;
        }

        private void InspectorTransformTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedContainer is null) return;

            if (!double.TryParse(insTrX.Text, out double x)) return;
            if (!double.TryParse(insTrY.Text, out double y)) return;
            if (!double.TryParse(insTrW.Text, out double w)) return;
            if (!double.TryParse(insTrH.Text, out double h)) return;

            SelectedContainer.CX = x;
            SelectedContainer.CY = y;
            SelectedContainer.CWidth = Math.Clamp(w, 5, 10_000);
            SelectedContainer.CHeight = Math.Clamp(h, 5, 10_000);
            ActivePage.Redraw();
        }

        private void InsDuplicate(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer is null) return;
            var copy = SelectedContainer.Clone();
            ActivePage.AddItem(copy);
            copy.Select();
            ActivePage.Redraw();
        }

        private void InsDelete(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer is null) return;
            ActivePage.RemoveItem(SelectedContainer);
            ActivePage.SelectedItem = null;
            ActivePage.Redraw();
        }

        private void SelectWhole(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox tb) return;
            if (tb.IsFocused) return;
            tb.Focus();
            tb.SelectionStart = 0;
            tb.SelectionLength = tb.Text.Length;
            e.Handled = true;
        }

        private void InsDwCenterContent(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            dwg.Viewport.CenterContent();
        }

        private void InsDwItemsIncrement(object sender, RoutedEventArgs e)
        {
            insDwItems.Value++;
        }

        private void InsDwItemsDecrement(object sender, RoutedEventArgs e)
        {
            insDwItems.Value--;
        }

        private void InsDwValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            dwg.VisibleItems = (int)insDwItems.Value;
        }

        private void InsDwUnitSizeDecrement(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            insDwUnitSize.Text = (dwg.UnitSize - 1).ToString();
        }

        private void InsDwUnitSizeIncrement(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            insDwUnitSize.Text = (dwg.UnitSize + 1).ToString();
        }

        private void InsDwUnitSizeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            if (!double.TryParse(insDwUnitSize.Text, out double us)) return;

            var center = dwg.Viewport.ScreenToCanvas((dwg.Viewport.CWidth / 2, dwg.Viewport.CHeight / 2));

            dwg.UnitSize = Math.Clamp(us, 1, 100);
            dwg.ViewUpdated(ActivePage.OffsetX, ActivePage.OffsetY, ActivePage.Scale);

            // Adjusts the offset so it will zoom in and out from the center
            var newCenter = dwg.Viewport.ScreenToCanvas((dwg.Viewport.CWidth / 2, dwg.Viewport.CHeight / 2));
            var centerDiff = newCenter - center;
            dwg.Viewport.OffsetX -= centerDiff.X;
            dwg.Viewport.OffsetY -= centerDiff.Y;
            dwg.Viewport.Redraw();
        }

        private async void InsDwSaveClick(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            MainWindow.OpenEditorSaveFileDialog(dwg.editor);
            await MainWindow.SaveEditorAsync(dwg.editor);
        }

        private void InsDwEditDrawing(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            

            for (int i = 0; i < MainWindow.openTabs.Count; i++)
            {
                if (MainWindow.openTabs[i] is not ConnectedEditorTab cet) continue;
                if (!ReferenceEquals(cet.Editor, dwg.editor)) continue;
                MainWindow.editorTabs.SelectedIndex = i;
                return;
            }


            MainWindow.openTabs.Add(new ConnectedEditorTab(dwg.editor));
            MainWindow.editorTabs.SelectedIndex = MainWindow.editorTabs.Items.Count - 1;
        }

        public void TabSelected()
        {
            SelectedContainer?.Deselect();
            UpdateConnectedEditors();
        }

        public void UpdateConnectedEditors()
        {
            foreach (MFContainer c in ActivePage.Items)
            {
                if (c.Item is not MFDrawing d) continue;
                double ox = d.Viewport.OffsetX;
                double oy = d.Viewport.OffsetY;
                double sc = d.Viewport.Scale;

                if (d.Viewport.Timeline.UndoneCommands.Count == 0)
                {
                    d.VisibleItems = d.editor.Timeline.CommandHistory.Count;
                }

                d.Viewport = d.editor.viewPort.Clone();
                d.Viewport.OffsetX = ox;
                d.Viewport.OffsetY = oy;
                d.Viewport.Scale = sc;
            }
        }
    }
}
