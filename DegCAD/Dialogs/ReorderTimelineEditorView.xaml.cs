﻿using DegCAD.Guides;
using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for ReorderTimelineEditorView.xaml
    /// </summary>
    public partial class ReorderTimelineEditorView : UserControl, IChangesWithDarkMode
    {
        private readonly Editor editor;

        private ObservableCollection<TimelineItemModel> tlItems = [];
        private ObservableCollection<GuideStepModel> guideSteps = [];

        private readonly ViewPort vp;

        public ReorderTimelineEditorView(Editor ed)
        {
            InitializeComponent();

            guideEditPopup.DataContext = new GuideStepModel(new());

            editor = ed;

            vp = ed.viewPort.Clone();
            vp.AllowLabelInteractions = false;
            vpBorder.Child = vp;

            for (int i = 0; i < vp.Timeline.CommandHistory.Count; i++)
            {
                var item = vp.Timeline.CommandHistory[i];
                if (item.Items.Length < 1) continue;
                if (item.Items[0] is Axis) continue;

                tlItems.Add(new(item, i));
            }

            itemsIC.ItemsSource = tlItems;

            if (ed.Guide is null)
            {
                guideStp.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (var step in ed.Guide.Steps)
            {
                guideSteps.Add(new(step));
            }

            guideIc.ItemsSource = guideSteps;
        }

        private void ItemMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not TimelineItemModel ti) return;

            vp.Timeline.ShowItem(ti.Index);

            foreach (var el in ti.Item.Items)
            {
                if (el is not GeometryElement ge) return;
                ge.IsHighlighted = true;
            }
        }

        private void ItemMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not TimelineItemModel ti) return;

            foreach (var el in ti.Item.Items)
            {
                if (el is not GeometryElement ge) return;
                ge.IsHighlighted = false;
            }
        }

        private void VpMouseEnter(object sender, MouseEventArgs e)
        {
            vp.Timeline.RedoAll();
        }

        private void CenterViewport(object sender, RoutedEventArgs e)
        {
            vp.CenterContent();
        }

        private class TimelineItemModel
        {
            public StackPanel Elements { get; init; } = new StackPanel() { Orientation = Orientation.Horizontal };
            public TimelineItem Item { get; init; }
            public int Index { get; set; }

            public TimelineItemModel(TimelineItem item, int index)
            {
                Item = item;
                Index = index;

                foreach (var el in item.Items)
                {
                    AddElement(el);
                }
            }

            private void AddElement(ITimelineElement el)
            {
                if (el is GeometryElement ge)
                {
                    // Adding label
                    if (ge is TimelineElements.Label lbl)
                    {
                        Grid grid = new() { Height = 30, Margin = new(5, 0, 5, 0) };
                        grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Auto) });
                        grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Auto) });
                        grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });
                        grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });

                        TextBlock tbl1 = new()
                        {
                            FontSize = 20,
                            Foreground = new SolidColorBrush(lbl.Style.Color),
                            Text = lbl.LabelText,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        grid.Children.Add(tbl1);
                        Grid.SetRowSpan(tbl1, 2);

                        TextBlock tbl2 = new()
                        {
                            FontSize = 10,
                            Foreground = new SolidColorBrush(lbl.Style.Color),
                            Text = lbl.Superscript,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        grid.Children.Add(tbl2);
                        Grid.SetColumn(tbl2, 1);

                        TextBlock tbl3 = new()
                        {
                            FontSize = 10,
                            Foreground = new SolidColorBrush(lbl.Style.Color),
                            Text = lbl.Subscript,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        grid.Children.Add(tbl3);
                        Grid.SetColumn(tbl3, 1);
                        Grid.SetRow(tbl3, 1);


                        Elements.Children.Add(grid);
                        return;
                    }
                    if (ge is Circle)
                    {
                        Elements.Children.Add(new System.Windows.Shapes.Ellipse()
                        {
                            Width = 26,
                            Height = 26,
                            Margin = new(2),
                            StrokeThickness = 2,
                            StrokeDashArray = new(strokeDashArrays[ge.Style.LineStyle]),
                            Stroke = new SolidColorBrush(ge.Style.Color)
                        });
                        return;
                    }
                    if (ge is TimelineElements.Ellipse)
                    {
                        Elements.Children.Add(new System.Windows.Shapes.Ellipse()
                        {
                            Width = 26,
                            Height = 14,
                            Margin = new(2, 7, 2, 7),
                            StrokeThickness = 2,
                            StrokeDashArray = new(strokeDashArrays[ge.Style.LineStyle]),
                            Stroke = new SolidColorBrush(ge.Style.Color)
                        });
                        return;
                    }

                    Path path = new()
                    {
                        Stroke = new SolidColorBrush(ge.Style.Color),
                        StrokeThickness = 2,
                        StrokeDashArray = new(strokeDashArrays[ge.Style.LineStyle]),
                        Data = Geometry.Parse("M 2 28 L 28 2"),
                        Width = 30,
                        Height = 30
                    };

                    if (ge is TimelineElements.LineSegment)
                    {
                        path.Data = Geometry.Parse("M 2 22 L 5 25 M 5 25 L 8 28 M 5 25 L 25 5 M 22 2 L 25 5 M 25 5 L 28 8");
                    }
                    else if (ge is TimelineElements.Point)
                    {
                        path.StrokeDashArray = new();
                        path.Data = Geometry.Parse("M 15 5 L 15 25 M 5 15 L 25 15");
                    }
                    else if (ge is CRSpline)
                    {
                        path.Data = Geometry.Parse("M 2 27 C 2 22, 6 9, 9 9 C 12 9, 17 20, 20 20 C 23 20, 27 7, 27 2");
                    }
                    else if (ge is Arc)
                    {
                        path.Data = Geometry.Parse("M 2 28 A 26 26 1 0 1 28 2");
                    }
                    else if (ge is HalfLine)
                    {
                        path.Data = Geometry.Parse("M 2 22 L 5 25 M 5 25 L 8 28 M 5 25 L 28 2 M 28 2 L 22 4 M 28 2 L 26 8");
                    }
                    else if (ge is Hyperbola)
                    {
                        path.Data = Geometry.Parse("M 6 28 C 14 26, 24 20, 24 15 C 24 10, 14 4, 6 2");
                    }
                    else if (ge is Parabola)
                    {
                        path.Data = Geometry.Parse("M 6 2 Q 15 50 24 2");
                    }

                    Elements.Children.Add(path);
                    return;
                }
                if (el is HideModification)
                {
                    Canvas c = new() { Width = 30, Height = 30 };

                    Path p = new() { StrokeThickness = 1 };
                    p.SetResourceReference(Shape.StrokeProperty, "fg");
                    p.Data = Geometry.Parse("M 2 15 Q 8 8, 15 8 Q 22 8, 28 15 Q 22 22, 15 22 Q 8 22, 2 15 M 4 26 L 26 4");
                    c.Children.Add(p);

                    System.Windows.Shapes.Ellipse el1 = new()
                    {
                        Width = 14,
                        Height = 14,
                        StrokeThickness = 1
                    };
                    c.Children.Add(el1);
                    el1.SetResourceReference(Shape.StrokeProperty, "fg");
                    Canvas.SetTop(el1, 8);
                    Canvas.SetLeft(el1, 8);

                    System.Windows.Shapes.Ellipse el2 = new()
                    {
                        Width = 6,
                        Height = 6
                    };
                    c.Children.Add(el2);
                    el2.SetResourceReference(Shape.FillProperty, "fg");
                    Canvas.SetTop(el2, 12);
                    Canvas.SetLeft(el2, 12);

                    Elements.Children.Add(c);
                }
            }

            private static double[][] strokeDashArrays = [
                [],
                [3, 2.2],
                [3, 1.5, 1, 1.5]
            ];
        }

        private class GuideStepModel : INotifyPropertyChanged
        {
            public GuideStep GuideStep { get; init; }

            public double Height => Math.Max(GuideStep.Items * 36 - 4, 0);
            public string Title => ((GuideStep.Items < 2) ? "K " : "Krok ") + GuideStep.Position.ToString();
            public string TooltipTitle => "Krok " + GuideStep.Position;
            public string Tooltip => GuideStep.Description;

            public Visibility Visibility => (GuideStep.Items == 0) ? Visibility.Collapsed : Visibility.Visible;

            public GuideStepModel(GuideStep step)
            {
                GuideStep = step;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public void InvokePropertyChange(string name)
            {
                PropertyChanged?.Invoke(this, new(name));
            }
        }

        private void StartTimelineItemDrag(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            DragDrop.DoDragDrop(f, f.DataContext, DragDropEffects.Move);
        }

        private void TimelineItemDrop(object sender, DragEventArgs e)
        {
            // Gets the dragged and dropped color
            var dragItem = e.Data.GetData(typeof(TimelineItemModel)) as TimelineItemModel;
            if (dragItem is null) return;
            if ((sender as FrameworkElement)?.DataContext is not TimelineItemModel dropItem) return;


            // Switch models
            int dragIndex = tlItems.IndexOf(dragItem);
            int dropIndex = tlItems.IndexOf(dropItem);
            if (dragIndex == -1 || dropIndex == -1 || dragIndex == dropIndex) return;

            // Switch in timeline
            int tlDragIndex = vp.Timeline.IndexOf(dragItem.Item);
            int tlDropIndex = vp.Timeline.IndexOf(dropItem.Item);
            if (tlDragIndex == -1 || tlDropIndex == -1) return;

            if (!vp.Timeline.Move(tlDragIndex, tlDropIndex)) return;
            tlItems.Move(dragIndex, dropIndex);

            foreach (var i in tlItems)
            {
                i.Index = vp.Timeline.IndexOf(i.Item);
            }

            _ = editor.Timeline.Move(tlDragIndex, tlDropIndex);
        }

        public void SwapWhiteAndBlack()
        {
            vp.SwapWhiteAndBlack();
            foreach (var cmd in tlItems)
            {
                foreach (var el in cmd.Elements.Children)
                {
                    if (el is Shape shape)
                    {
                        if (shape.Stroke is not SolidColorBrush cb) continue;
                        if (cb.Color == Colors.Black)
                            shape.Stroke = Brushes.White;
                        else if (cb.Color == Colors.White)
                            shape.Stroke = Brushes.Black;
                        continue;
                    }
                    if (el is Grid grid)
                    {
                        // Label
                        foreach (var ch in grid.Children)
                        {
                            if (ch is not TextBlock tbl) continue;
                            if (tbl.Foreground is not SolidColorBrush cb) continue;
                            if (cb.Color == Colors.Black)
                                tbl.Foreground = Brushes.White;
                            else if (cb.Color == Colors.White)
                                tbl.Foreground = Brushes.Black;
                            continue;
                        }
                    }
                }
            }
        }

        private void GuideStepClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not GuideStepModel step) return;

            guideEditPopup.DataContext = step;
            guideEditPopup.PlacementTarget = fe;
            guideEditPopup.IsOpen = true;
        }

        private void IncrementItemsCount(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not GuideStepModel step) return;
            step.GuideStep.Items++;
            step.InvokePropertyChange(nameof(step.Height));
            step.InvokePropertyChange(nameof(step.Title));
        }
        private void DecrementItemsCount(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not GuideStepModel step) return;
            if (step.GuideStep.Items <= 1) return;
            step.GuideStep.Items--;
            step.InvokePropertyChange(nameof(step.Height));
            step.InvokePropertyChange(nameof(step.Title));
        }

        private void GuideStepDescriptionChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not GuideStepModel step) return;
            step.InvokePropertyChange(nameof(step.Tooltip));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (editor.Guide is not Guide g) return;
            GuideStep newStep = new() { Position = g.Steps.Count + 1 };
            g.Steps.Add(newStep);
            guideSteps.Add(new(newStep));
        }
    }
}
