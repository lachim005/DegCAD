using DegCAD.MultiFile.History;
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
    /// Interaction logic for MFContainer.xaml
    /// </summary>
    public partial class MFContainer : UserControl
    {
        public const double minSize = 5;

        private bool _borderVisible = true;

        public double CWidth { get; set; } = 100;
        public double CHeight { get; set; } = 100;
        public double CX { get; set; }
        public double CY { get; set; }

        public event EventHandler? Selected;
        public event EventHandler? Deselected;
        public event EventHandler? Updating;
        public event EventHandler<TransformChange>? Updated;
        public MFPage Page { get; set; }
        public MFItem Item { get; init; }
        public bool IsSelected { get; private set; }

        public bool BorderVisible
        {
            get => _borderVisible;
            set
            {
                _borderVisible = value;
                containerBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public MFContainer(MFPage page, MFItem item)
        {
            InitializeComponent();
            Page = page;
            Item = item;
            item.Container = this;
            contentBorder.Child = item;
        }

        public void Deselect()
        {    
            IsSelected = false;
            Deselected?.Invoke(this, EventArgs.Empty);
        }
        public void Select()
        {
            IsSelected = true;
            Selected?.Invoke(this, EventArgs.Empty);
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Select();
                e.Handled = true;
            }
        }

        public MFContainer Clone()
        {
            return new(Page, Item.Clone()) { CX = CX, CY = CY, CWidth = CWidth, CHeight = CHeight};
        }
        public MFContainer Clone(MFPage page)
        {
            return new(page, Item.Clone()) { CX = CX, CY = CY, CWidth = CWidth, CHeight = CHeight };
        }

        public void InvokeUpdating()
        {
            Updating?.Invoke(this, EventArgs.Empty);
        }

        public void InvokeUpdated(TransformChange tc)
        {
            Updated?.Invoke(this, tc);
        }
    }
}
