using DegCAD.Dialogs;
using DegCAD.MultiFile.History;
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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFEditor.xaml
    /// </summary>
    public partial class MFEditor : UserControl, INotifyPropertyChanged
    {
        private MFPage _activePage;
        private bool initiated;
        private bool _changed;

        public MFPage ActivePage
        {
            get => _activePage;
            set
            {
                _activePage = value;
                PropertyChanged?.Invoke(this, new(nameof(ActivePage)));
            }
        }
        public MFContainer? SelectedContainer { get; set; }
        public MainWindow MainWindow { get; init; }

        public ObservableCollection<MFPageModel> Pages { get; init; } = new();
        public MFTimeline Timeline { get; init; }
        public bool Changed
        {
            get => _changed;
            set
            {
                _changed = value;
                PropertyChanged?.Invoke(this, new(nameof(Changed)));
            }
        }
        public MFEditorTab? Tab { get; set; } = null;

        public MFEditor(MainWindow mw) : this(mw, new MFPage[0])
        {
            
        }

        public MFEditor(MainWindow mw, IEnumerable<MFPage> pages)
        {
            DataContext = this;
            InitializeComponent();

            Timeline = new(this);

            Pages.CollectionChanged += ReIndexPages;

            if (pages.Count() == 0)
            {
                AddPage(new(this));
            }
            else
            {
                foreach (var page in pages)
                {
                    AddPage(page);
                    page.Editor = this;
                }
            }
            _activePage = Pages[0].Page;
            SelectPage(_activePage);
            

            Pages[0].IsButtonEnabled = false;


            insPagesIc.ItemsSource = Pages;

            MainWindow = mw;

            intTxtColorPaletteIC.ItemsSource = Settings.DefaultColors;
        }
        private void EditorLoaded(object sender, RoutedEventArgs e)
        {
            if (!initiated)
            {
                initiated = true;
                insTransform.Visibility = Visibility.Collapsed;
                insDrawing.Visibility = Visibility.Collapsed;
                insText.Visibility = Visibility.Collapsed;
                ActivePage.CenterPage();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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
            insComposition.Visibility = Visibility.Collapsed;
            insPages.Visibility = Visibility.Collapsed;
            insSnapping.Visibility = Visibility.Collapsed;
            SelectedContainer = null;
            if (e is null)
            {
                insPages.Visibility = Visibility.Visible;
                insComposition.Visibility = Visibility.Visible;
                insSnapping.Visibility = Visibility.Visible;
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
                insDwItems.Maximum = dr.editor.Timeline.CommandHistory.Count;
                insDwItems.Value = dr.VisibleItems;
                insDwUnitSize.Text = dr.UnitSize.ToString();
                insDwLockPosition.IsChecked = dr.PositionLocked;
            } else if (e.Item is MFText txt)
            {
                insText.Visibility = Visibility.Visible;
                insTxtText.Text = txt.Text;
                insTxtText.Focus();
                insTxtText.SelectAll();

                // Set horizontal alignment
                insTxtHALeft.IsEnabled = true;
                insTxtHACenter.IsEnabled = true;
                insTxtHARight.IsEnabled = true;
                insTxtHAJustify.IsEnabled = true;
                (txt.HAlign switch
                {
                    TextAlignment.Left => insTxtHALeft,
                    TextAlignment.Right => insTxtHARight,
                    TextAlignment.Justify => insTxtHAJustify,
                    _ => insTxtHACenter,
                }).IsEnabled = false;

                // Set vertical alignment
                insTxtVATop.IsEnabled = true;
                insTxtVACenter.IsEnabled = true;
                insTxtVABottom.IsEnabled = true;
                (txt.VAlign switch
                {
                    VerticalAlignment.Top => insTxtVATop,
                    VerticalAlignment.Bottom => insTxtVABottom,
                    _ => insTxtVACenter,
                }).IsEnabled = false;

                // Set BIUS
                insTxtBold.IsEnabled = !txt.Bold;
                insTxtItalic.IsEnabled = !txt.Italic;
                insTxtUnderline.IsEnabled = !txt.Underline;
                insTxtStrikethrough.IsEnabled = !txt.Strikethrough;

                // Set Fontsize and color
                insTxtFontSize.Text = txt.TextFontSize.ToString();
                insTxtColor.Fill = new SolidColorBrush(txt.Color);
            }

            SelectedContainer = e;
        }

        public void ExecuteCommand(ICommand c)
        {
            if (c is not IMFCommand command) return;
            if (command.Execute() is not MFItem item) return;
            MFContainer cont = new(ActivePage, item) { CX = 0, CY = ActivePage.GetMaxY() };
            ActivePage.AddItem(cont);
            cont.Select();

            ActivePage.Redraw();

            Timeline.AddState(new ContainerAddedState(cont));
        }

        #region Container inspector
        private void InspectorTransformTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedContainer is null) return;

            if (!double.TryParse(insTrX.Text, out double x)) return;
            if (!double.TryParse(insTrY.Text, out double y)) return;
            if (!double.TryParse(insTrW.Text, out double w)) return;
            if (!double.TryParse(insTrH.Text, out double h)) return;

            Timeline.AddState(new ContainerTransformState(SelectedContainer));

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
            copy.CX += 5;
            copy.CY += 5;
            ActivePage.AddItem(copy);
            copy.Select();
            ActivePage.Redraw();

            Timeline.AddState(new ContainerAddedState(SelectedContainer));
        }

        private void InsDelete(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer is null) return;

            Timeline.AddState(new ContainerRemovedState(SelectedContainer));

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
#endregion

        #region Drawing inspector
        private void InsDwCenterContent(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;

            Timeline.AddState(new DrawingState(dwg));

            dwg.Viewport.CenterContent();
        }

        private void InsDwItemsIncrement(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;

            Timeline.AddState(new DrawingState(dwg));

            insDwItems.Value++;
        }

        private void InsDwItemsDecrement(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;

            Timeline.AddState(new DrawingState(dwg));

            insDwItems.Value--;
        }

        private void InsDwValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;
            dwg.VisibleItems = (int)insDwItems.Value;
        }

        private void InsDwItemsSliderDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;

            Timeline.AddState(new DrawingState(dwg));
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

            Timeline.AddState(new DrawingState(dwg));

            var center = dwg.Viewport.ScreenToCanvas((dwg.Viewport.CWidth / 2, dwg.Viewport.CHeight / 2));

            dwg.UnitSize = Math.Clamp(us, 1, 100);
            dwg.ViewUpdated(ActivePage.OffsetX, ActivePage.OffsetY, ActivePage.Scale);
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
            if (Tab is null) return;

            for (int i = 0; i < MainWindow.openTabs.Count; i++)
            {
                if (MainWindow.openTabs[i] is not ConnectedEditorTab cet) continue;
                if (!ReferenceEquals(cet.Editor, dwg.editor)) continue;
                MainWindow.editorTabs.SelectedIndex = i;
                return;
            }


            var tab = new ConnectedEditorTab(dwg.editor, MainWindow, Tab);
            MainWindow.openTabs.Add(tab);
            MainWindow.editorTabs.SelectedIndex = MainWindow.editorTabs.Items.Count - 1;
        }

        private void InsDwLockPosChecked(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFDrawing dwg) return;

            Timeline.AddState(new DrawingState(dwg));

            dwg.PositionLocked = insDwLockPosition.IsChecked == true;
        }
        #endregion

        #region Text inspector

        bool insTxtTextboxFocused;

        private void InsTxtTextboxGotFocus(object sender, RoutedEventArgs e)
        {
            insTxtTextboxFocused = true;
        }

        private void InsTxtTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;

            if (insTxtTextboxFocused)
            {
                insTxtTextboxFocused = false;
                Timeline.AddState(new TextTextState(txt));
            }

            txt.Text = insTxtText.Text;
        }

        private void InsTxtVaChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            insTxtVATop.IsEnabled = true;
            insTxtVACenter.IsEnabled = true;
            insTxtVABottom.IsEnabled = true;

            b.IsEnabled = false;
            txt.VAlign = b.Name switch
            {
                "insTxtVATop" => VerticalAlignment.Top,
                "insTxtVABottom" => VerticalAlignment.Bottom,
                _ => VerticalAlignment.Center,
            };
        }

        private void InsTxtHaChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            insTxtHALeft.IsEnabled = true;
            insTxtHACenter.IsEnabled = true;
            insTxtHARight.IsEnabled = true;
            insTxtHAJustify.IsEnabled = true;

            b.IsEnabled = false;
            txt.HAlign = b.Name switch
            {
                "insTxtHALeft" => TextAlignment.Left,
                "insTxtHARight" => TextAlignment.Right,
                "insTxtHAJustify" => TextAlignment.Justify,
                _ => TextAlignment.Center,
            };
        }

        private void InsTxtBold(object sender, MouseButtonEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            bool enabled = !txt.Bold;
            insTxtBold.IsEnabled = !enabled;
            txt.Bold = enabled;
            e.Handled = true;
        }
        private void InsTxtItalic(object sender, MouseButtonEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            bool enabled = !txt.Italic;
            insTxtItalic.IsEnabled = !enabled;
            txt.Italic = enabled;
            e.Handled = true;
        }
        private void InsTxtUnderline(object sender, MouseButtonEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            bool enabled = !txt.Underline;
            insTxtUnderline.IsEnabled = !enabled;
            txt.Underline = enabled;
            e.Handled = true;
        }
        private void InsTxtStrikethrough(object sender, MouseButtonEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            bool enabled = !txt.Strikethrough;
            insTxtStrikethrough.IsEnabled = !enabled;
            txt.Strikethrough = enabled;
            e.Handled = true;
        }

        private void InsTxtFontSizeChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;
            if (!int.TryParse(insTxtFontSize.Text, out int fs)) return;

            Timeline.AddState(new TextStyleState(txt));

            txt.TextFontSize = Math.Clamp(fs, 8, 256);
            txt.ViewUpdated(ActivePage.OffsetX, ActivePage.OffsetY, ActivePage.Scale);
        }

        private void InsTxtDecrementFontSize(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;
            insTxtFontSize.Text = (txt.TextFontSize - 1).ToString();
        }

        private void InsTxtIncrementFontSize(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;
            insTxtFontSize.Text = (txt.TextFontSize + 1).ToString();
        }

        private void InsTxtChangeColor(object sender, MouseButtonEventArgs e)
        {
            insTxtColorPalettePopup.IsOpen = true;
        }
        private void InsTxtCustomColorClick(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;

            Timeline.AddState(new TextStyleState(txt));

            var c = ColorPicker.EditColor(txt.Color);
            txt.Color = c;
            insTxtColor.Fill = new SolidColorBrush(c);
            e.Handled = true;
        }

        private void InsTxtPaletteColorClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedContainer?.Item is not MFText txt) return;
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not Color c) return;

            Timeline.AddState(new TextStyleState(txt));

            txt.Color = c;
            insTxtColor.Fill = new SolidColorBrush(c);
            e.Handled = true;
        }

        #endregion

        #region Pages
        public void AddPage(MFPage page, int? index = null)
        {
            if (index is not null)
            {
                Pages.Insert(index.Value, new(page));
            }
            else
            {
                Pages.Add(new(page));
            }

            page.SelectionChanged += PageSelectionChanged;
            page.ContainerUpdated += ContainerUpdated;
            page.BordersVisible = insCompBordersVisibility.IsChecked == true;
            page.Snapper.CopySettings(Pages[0].Page.Snapper);
        }
        public void RemovePage(MFPage page)
        {
            Pages.Remove(GetPageModel(page));
            page.SelectionChanged -= PageSelectionChanged;
            page.ContainerUpdated -= ContainerUpdated;
        }
        private MFPageModel GetPageModel(MFPage page) => Pages.First(p => p.Page == page);

        private void ReIndexPages(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].Index = i + 1;
            }
        }

        private void InsPageClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not MFPageModel pm) return;

            SelectPage(pm.Page);
        }
        public void SelectPage(MFPage page)
        {
            SelectedContainer?.Deselect();

            var pm = GetPageModel(page);
            foreach (var p in Pages)
            {
                p.IsButtonEnabled = true;
            }

            double offsetX = ActivePage.OffsetX,
                offsetY = ActivePage.OffsetY,
                scale = ActivePage.Scale;

            ActivePage = pm.Page;

            ActivePage.OffsetX = offsetX;
            ActivePage.OffsetY = offsetY;
            ActivePage.Scale = scale;

            pm.IsButtonEnabled = false;

            // Update inspector page size
            var w = page.PaperWidth.ToString();
            var h = page.PaperHeight.ToString();
            insPageWidth.Text = w;
            insPageHeight.Text = h;
        }
        private void InsPagesAdd(object sender, RoutedEventArgs e)
        {
            MFPage page = new(this) { PaperWidth = ActivePage.PaperWidth, PaperHeight = ActivePage.PaperHeight };
            AddPage(page);
            SelectPage(page);

            Timeline.AddState(new PageAddedState(page));
        }
        private void InsPagesRemove(object sender, RoutedEventArgs e)
        {
            Timeline.AddState(new PageRemovedState(ActivePage));
            if (Pages.Count == 1)
            {
                AddPage(new(this));
            }
            RemovePage(ActivePage);
            SelectPage(Pages[0].Page);
        }

        bool insPageSizeFocused;
        private void insPageWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            insPageSizeFocused = true;
        }
        private void InsPageSizeChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse(insPageWidth.Text, out var w)) return;
            if (!double.TryParse(insPageHeight.Text, out var h)) return;

            if (insPageSizeFocused)
            {
                Timeline.AddState(new PagePaperSizeState(ActivePage));
                insPageSizeFocused = false;
            }

            ActivePage.PaperWidth = Math.Max(w, 1);
            ActivePage.PaperHeight = Math.Max(h, 1);
        }
        private void SelectPageSizePresetClick(object sender, RoutedEventArgs e)
        {
            var preset = SelectPaperSizePresetDialog.GetPaperSizePreset();
            if (preset is null) return;

            insPageSizeFocused = true;
            insPageWidth.Text = preset.Width.ToString();
            insPageHeight.Text = preset.Height.ToString();
        }
        private void PageRedrag(object sender, DragEventArgs e)
        {
            //Gets the dragged and dropped tab
            if (e.Data.GetData(typeof(MFPageModel)) is not MFPageModel dragPage) return;

            if (sender is not FrameworkElement f) return;
            if (f.DataContext is not MFPageModel m) return;

            int dragIndex = Pages.IndexOf(dragPage);
            int dropIndex = Pages.IndexOf(m);
            if (dragIndex == -1 || dropIndex == -1) return;
            if (dragIndex == dropIndex) return;

            e.Handled = true;
            Pages.Move(dragIndex, dropIndex);
            if (Timeline.History.Peek() is PageOrderChangeState pocs)
            {
                pocs.index2 = dropIndex;
            }
        }

        private void StartPageReorder(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement btn) return;
            if (btn.DataContext is not MFPageModel pm) return;

            e.Handled = true;
            SelectPage(pm.Page);

            Timeline.AddState(new PageOrderChangeState(this, pm.Index - 1, pm.Index - 1));

            DragDrop.DoDragDrop(btn, pm, DragDropEffects.Move);
        }
        #endregion

        #region Snapper settings
        private void SnapSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (!initiated) return;
            bool snapToPage = insSnapToPageCbx.IsChecked == true;
            bool snapToObjects = insSnapToObjectsCbx.IsChecked == true;
            bool snapToGrid = insSnapToGridCbx.IsChecked == true;

            foreach (var page in Pages)
            {
                page.Page.Snapper.SnapToPage = snapToPage;
                page.Page.Snapper.SnapToObjects = snapToObjects;
                page.Page.Snapper.SnapToGrid = snapToGrid;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!initiated) return;
            if (!double.TryParse(insSnapGridWidth.Text, out double w) || w < .001) return;
            if (!double.TryParse(insSnapGridHeight.Text, out double h) || h < .001) return;

            foreach (var page in Pages)
            {
                page.Page.Snapper.GridWidth = w;
                page.Page.Snapper.GridHeight = h;
            }
        }
        #endregion

        #region Composition
        private void InsCompShowBorders(object sender, RoutedEventArgs e)
        {
            foreach (var p in Pages)
            {
                p.Page.BordersVisible = true;
            }
        }

        private void InsCompHideBorders(object sender, RoutedEventArgs e)
        {
            foreach (var p in Pages)
            {
                p.Page.BordersVisible = false;
            }
        }
        #endregion

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
                d.Viewport.AllowLabelInteractions = false;
                d.Viewport.OffsetX = ox;
                d.Viewport.OffsetY = oy;
                d.Viewport.Scale = sc;
            }
        }

        public void SwapWhiteAndBlack()
        {
            foreach (var page in Pages)
            {
                page.Page.SwapWhiteAndBlack();
            }
        }

        private void CenterScreenClick(object sender, RoutedEventArgs e)
        {
            ActivePage.CenterPage();
        }

        private void PrintClick(object sender, RoutedEventArgs e)
        {
            Print();
        }

        public void Print()
        {
            MFPrintDialog pd = new(this);
            pd.ShowDialog();
        }

        private void DuplicatePageClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not MFPageModel pm) return;

            var newPage = pm.Page.Clone();
            AddPage(newPage, pm.Index);
            SelectPage(newPage);

            Timeline.AddState(new PageAddedState(newPage));
        }
    }
}
