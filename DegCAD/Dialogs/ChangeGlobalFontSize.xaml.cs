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
using Label = DegCAD.MongeItems.Label;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for ChangeGlobalFontSize.xaml
    /// </summary>
    public partial class ChangeGlobalFontSize : Window
    {
        Editor editor;
        ViewPort previewVp;
        bool initiated = false;

        List<Label> labels = new();
        List<int> initialFontSizes = new List<int>();

        public ChangeGlobalFontSize(Editor e)
        {
            InitializeComponent();
            editor = e;
            previewVp = e.viewPort.Clone();
            vpBorder.Child = previewVp;
            previewVp.AllowLabelInteractions = false;

            foreach (var cmd in previewVp.Timeline.CommandHistory)
            {
                foreach (var item in cmd.Items)
                {
                    if (item is not Label lbl) continue;
                    labels.Add(lbl);
                    initialFontSizes.Add(lbl.FontSize);
                }
            }
        }

        private void UpdateText()
        {
            if (!initiated) return;
            if (sizingUniform.IsChecked == true)
            {
                if (!int.TryParse(uniformFontSizeTbx.Text, out int fs)) return;
                foreach (var lbl in labels)
                {
                    lbl.FontSize = fs;
                }
            } else 
            {
                if (!double.TryParse(fontSizeFactorTbx.Text, out double factor)) return;
                for (int i = 0; i < labels.Count; i++)
                {
                    labels[i].FontSize = (int)(initialFontSizes[i] * factor);
                }
            }
            previewVp.Redraw();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateText();
        }

        private void SizingOptionChanged(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (initiated) return;
            initiated = true;

            previewVp.CenterContent();
        }

        private void CenterViewport(object sender, RoutedEventArgs e)
        {
            previewVp.CenterContent();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {      
            if (sizingUniform.IsChecked == true)
            {
                if (!int.TryParse(uniformFontSizeTbx.Text, out int fs))
                {
                    MessageBox.Show("Neplatná hodnota", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var cmd in editor.Timeline.CommandHistory)
                {
                    foreach (var item in cmd.Items)
                    {
                        if (item is not Label lbl) continue;
                        lbl.FontSize = fs;
                    }
                }
            }
            else
            {
                if (!double.TryParse(fontSizeFactorTbx.Text, out double factor))
                {
                    MessageBox.Show("Neplatná hodnota", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var cmd in editor.Timeline.CommandHistory)
                {
                    foreach (var item in cmd.Items)
                    {
                        if (item is not Label lbl) continue;
                        lbl.FontSize = (int)(lbl.FontSize * factor);
                    }
                }
            }
            editor.viewPort.Redraw();
            Close();
        }
    }
}
