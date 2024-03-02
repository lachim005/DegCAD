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
    /// Interaction logic for MFContainer.xaml
    /// </summary>
    public partial class MFContainer : UserControl
    {
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
        private TransformChange trChange = new();

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
            contentBorder.Child = item;
        }

        public void Deselect()
        {
            handles.Visibility = Visibility.Collapsed;
            Deselected?.Invoke(this, EventArgs.Empty);
        }
        public void Select()
        {
            Selected?.Invoke(this, EventArgs.Empty);
            handles.Visibility = Visibility.Visible;
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Select();
                e.Handled = true;
            }
        }

        private void Handle1Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth - cd.X <= 5 || CHeight - cd.Y <= 5) return;
            CX += cd.X;
            CY += cd.Y;
            CWidth -= cd.X;
            CHeight -= cd.Y;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle2Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;
            if (CHeight - vc <= 5) return;
            CY += vc;
            CHeight -= vc;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle3Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth + cd.X <= 5 || CHeight - cd.Y <= 5) return;
            CY += cd.Y;
            CWidth += cd.X;
            CHeight -= cd.Y;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle4Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;
            if (CWidth - hc <= 5 ) return;
            CX += hc;
            CWidth -= hc;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle5Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var hc = e.HorizontalChange / Page.Scale / MFPage.unitSize;
            if (CWidth + hc <= 5) return;
            CWidth += hc;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle6Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth - cd.X <= 5 || CHeight + cd.Y <= 5) return;
            CX += cd.X;
            CWidth -= cd.X;
            CHeight += cd.Y;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle7Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var vc = e.VerticalChange / Page.Scale / MFPage.unitSize;
            if (CHeight + vc <= 5) return;
            CHeight += vc;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void Handle8Delta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            if (CWidth + cd.X <= 5 || CHeight + cd.Y <= 5) return;
            CWidth += cd.X;
            CHeight += cd.Y;
            Updating?.Invoke(this, EventArgs.Empty);
        }
        private void MoveDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var cd = new Vector2(e.HorizontalChange, e.VerticalChange) / Page.Scale / MFPage.unitSize;
            CX += cd.X;
            CY += cd.Y;
            Updating?.Invoke(this, EventArgs.Empty);
        }

        private void TransformStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            trChange = new();
            trChange.SetStartVals(this);
        }

        private void TransformCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            trChange.SetEndVals(this);
            Updated?.Invoke(this, trChange);
        }

        public MFContainer Clone()
        {
            return new(Page, Item.Clone()) { CX = CX+5, CY = CY+5, CWidth = CWidth, CHeight = CHeight};
        }
    }

    public record TransformChange()
    {
        public void SetStartVals(MFContainer cont)
        {
            startX = cont.CX;
            startY = cont.CY;
            startW = cont.CWidth;
            startH = cont.CHeight;
        }

        public void SetEndVals(MFContainer cont)
        {
            endX = cont.CX;
            endY = cont.CY;
            endW = cont.CWidth;
            endH = cont.CHeight;
        }

        public double startX;
        public double startY;
        public double startW;
        public double startH;
        public double endX;
        public double endY;
        public double endW;
        public double endH;
    }
}