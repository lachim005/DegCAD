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
        public MFEditor()
        {
            InitializeComponent();

            ActivePage = new MFPage();
            pageBorder.Child = ActivePage;
            ActivePage.SelectionChanged += PageSelectionChanged;
            ActivePage.ContainerUpdated += ContainerUpdated;
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
            SelectedContainer = null;
            if (e is null)
            {
                insTransform.Visibility = Visibility.Collapsed;
                insDrawing.Visibility = Visibility.Collapsed;
                insText.Visibility = Visibility.Collapsed;
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
                insText.Visibility = Visibility.Collapsed;


            }

            SelectedContainer = e;
        }

        private void InspectorTransformTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedContainer is null) return;

            if (!double.TryParse('0' + insTrX.Text, out double x)) return;
            if (!double.TryParse('0' + insTrY.Text, out double y)) return;
            if (!double.TryParse('0' + insTrW.Text, out double w)) return;
            if (!double.TryParse('0' + insTrH.Text, out double h)) return;

            SelectedContainer.CX = x;
            SelectedContainer.CY = y;
            SelectedContainer.CWidth = w;
            SelectedContainer.CHeight = h;
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
    }
}
