﻿using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using DegCAD.Guides;
using DegCAD.MultiFile;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl, INotifyPropertyChanged
    {
        private string _fileName;
        private string? _folderPath;
        private bool _executingCommand = false;
        private bool _changed = false;
        private bool initiated = false;
        private Control? _activeView = null;

        public GeometryInputManager InputMgr { get; protected set; }
        public Timeline Timeline => viewPort.Timeline;
        public Snapper Snapper { get; protected set; }
        public ProjectionType ProjectionType { get; set; }
        public AxonometryAxes? AxonometryAxes { get; set; } = null;
        public Control? ActiveView 
        {
            get => _activeView;
            protected set
            {
                _activeView = value;
                ActiveViewChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<Control?> ActiveViewChanged;

        public bool ExecutingCommand
        {
            get => _executingCommand;
            private set
            {
                _executingCommand = value;
                PropertyChanged?.Invoke(this, new(nameof(ExecutingCommand)));
            }
        }
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                PropertyChanged?.Invoke(this, new(nameof(FileName)));
            }

        }
        public string? FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value;
                PropertyChanged?.Invoke(this, new(nameof(FolderPath)));
            }
        }

        public bool Changed
        {
            get => _changed;
            set
            {
                _changed = value;
                PropertyChanged?.Invoke(this, new(nameof(Changed)));
            }
        }

        public Editor(string fileName, ProjectionType projectionType)
        {
            InitializeComponent();
            Timeline.TimelineChanged += TimelineChanged;
            Snapper = new(Timeline);
            InputMgr = new(viewPort, Snapper, styleSelector);

            _fileName = fileName;
            ProjectionType = projectionType;

            repeatCommandCbx.IsChecked = Settings.RepeatCommands;
            nameNewItemsCbx.IsChecked = Settings.NameNewItems;


            OOBEToolsPanel.OpenIfTrue(ref Settings.OOBEState.toolsPanel);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddAxis(ViewportLayer vpl)
        {
            switch (ProjectionType)
            {
                case ProjectionType.Plane:
                    Timeline.AddCommand(new TimelineItem(
                    [
                        new Axis(),
                        new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default, vpl),
                        new InfiniteLine(new((0,0), (0,1)), DegCAD.Style.Default, vpl),
                        new TimelineElements.Label("x", "", "", (8,0), DegCAD.Style.Default, new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default), vpl),
                        new TimelineElements.Label("y", "", "", (0,-8), DegCAD.Style.Default, new InfiniteLine(new((0,0), (0,1)), DegCAD.Style.Default), vpl),
                        new TimelineElements.Label("0", "", "", (0,0), DegCAD.Style.Default, new TimelineElements.Point(0,0), vpl),
                    ]));
                    break;
                case ProjectionType.Monge:
                    Timeline.AddCommand(new TimelineItem(
                    [
                        new Axis(),
                        new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default, vpl),
                        new TimelineElements.Point(0, 0, DegCAD.Style.Default, vpl),
                        new TimelineElements.Label("x", "1, 2", "", (8,0), DegCAD.Style.Default, new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default), vpl),
                        new TimelineElements.Label("0", "", "", (0,0), DegCAD.Style.Default, new TimelineElements.Point(0,0), vpl),
                    ]));
                    break;
            }

        }
        public void TimelineChanged(object? sender, EventArgs e)
        {
            Changed = true;
        }


        public async void ExecuteCommand(ICommand c)
        {
            if (c is not IGeometryCommand command) return;

            OOBEStatusBar.OpenIfTrue(ref Settings.OOBEState.statusBar);

            do
            {
                Debug.WriteLine($"Executing command: {command}");
                ExecutingCommand = true;

                //Disables moving labels
                foreach (var layer in viewPort.Layers)
                {
                    layer.Canvas.IsHitTestVisible = false;
                }

                statusBar.ShowCommandStatus();
                cancelCmdBtn.Visibility = Visibility.Visible;
                try
                {
                    var res = await command.ExecuteAsync(viewPort.Layers[2], viewPort.Layers[1], viewPort.Layers[0], InputMgr, statusBar);
                    if (res is not null)
                    {
                        Timeline.AddCommand(res);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is CommandCanceledException) return;
                    MessageBox.Show(Window.GetWindow(this), "Při spuštění nástroje se vyskytla chyba\n\n" + ex.Message, img: MessageBoxImage.Error);
                    return;
                }
                finally
                {
                    viewPort.Layers[2].Canvas.Children.Clear();
                    viewPort.Layers[0].Canvas.Children.Clear();
                    ExecutingCommand = false;
                    statusBar.HideCommandStatus();
                    statusBar.CommandName = "";
                    statusBar.CommandHelp = "";
                    cancelCmdBtn.Visibility = Visibility.Collapsed;
                    //Reenables moving labels
                    foreach (var layer in viewPort.Layers)
                    {
                        layer.Canvas.IsHitTestVisible = true;
                    }
                }
            } while (repeatCommandCbx.IsChecked == true);

            await Task.Delay(100);
            OOBEPanViewport.OpenIfTrue(ref Settings.OOBEState.panViewport);
        }

        public void ShowView(Control view, string viewTitle, bool changed = true)
        {
            ExecutingCommand = true;
            mainView.Visibility = Visibility.Collapsed;
            altViewGrid.Visibility = Visibility.Visible;
            altView.Child = view;
            altViewTitle.Content = viewTitle;
            ActiveView = view;
            if (changed)
                Changed = true;
        }

        private void ExitView(object sender, RoutedEventArgs e)
        {
            altViewGrid.Visibility = Visibility.Collapsed;
            mainView.Visibility = Visibility.Visible;
            altView.Child = null;
            ExecutingCommand = false;
            ActiveView = null;
        }

        #region Guide
        private Guide? _guide;

        public Guide? Guide
        {
            get => _guide;
            set
            {
                _guide = value;
                if (value is null)
                    guideButtons.Visibility = Visibility.Collapsed;
                else
                    guideButtons.Visibility = Visibility.Visible;
            }
        }
        private void EditGuideBtn(object sender, RoutedEventArgs e)
        {
            if (Guide is null) return;
            EditorGuideEditorView ge = new(viewPort, Guide);
            ShowView(ge, "Editor návodu");
        }
        private void ShowGuideBtn(object sender, RoutedEventArgs e)
        {
            if (Guide is null) return;
            if (Guide.Steps.Count == 0)
            {
                MessageBox.Show(Window.GetWindow(this), "Návod neobsahuje žádné kroky", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EditorGuideView gv = new(viewPort, Guide);
            ShowView(gv, "Návod", false);
        }
        private void RemoveGuideBtn(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(Window.GetWindow(this), "Opravdu chcete vymazat návod?\nTato akce je nevratná", "Varování", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
                Guide = null;
        }
        public void PromptGuide()
        {
            if (!Settings.AlertGuides) return;
            if (Guide is not Guide g) return;
            if (g.Steps.Count == 0) return;
            if (Window.GetWindow(this) is not Window win) return;
            if (!OpenGuideDialog.OpenDialog(win)) return;
            EditorGuideView gv = new(viewPort, Guide);
            ShowView(gv, "Návod", false);
        }
        #endregion

        private void OpenEditorFullscreen(object sender, RoutedEventArgs e)
        {
            FullscreenPresenter fs = new(viewPort.Clone());
            fs.Owner = Window.GetWindow(this);
            fs.ShowDialog();
        }
        private void CenterScreenClick(object sender, RoutedEventArgs e)
        {
            viewPort.CenterContent();
        }

        private void EditorLoaded(object sender, RoutedEventArgs e)
        {
            if (!initiated)
            {
                initiated = true;
                viewPort.CenterContent();
                PromptGuide();
            }
        }

        public void SwapWhiteAndBlack()
        {
            viewPort.SwapWhiteAndBlack();
            styleSelector.SwapWhiteAndBlack();
            if (ActiveView is IChangesWithDarkMode cwdm)
            {
                cwdm.SwapWhiteAndBlack();
            }
        }

        public Editor Clone()
        {
            Editor ed = new(FileName, ProjectionType);
            ed.FolderPath = FolderPath;

            // Clone the palette
            ed.styleSelector.ColorPalette.Clear();
            foreach (var color in styleSelector.ColorPalette)
            {
                ed.styleSelector.ColorPalette.Add(color);
            }
            ed.styleSelector.UpdateColorPalette();

            // Clone the commands
            foreach (var cmd in Timeline.CommandHistory)
            {
                ITimelineElement[] mits = new ITimelineElement[cmd.Items.Length];
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    var it = cmd.Items[i].Clone();
                    mits[i] = it;
                }
                ed.Timeline.AddCommand(new(mits));
            }
            ed.Timeline.SetViewportLayer(ed.viewPort.Layers[1]);

            // Clone the guide
            if (Guide is not null)
            {
                ed.Guide = Guide.Clone();
            }

            // Clone axonometry axes
            if (AxonometryAxes is not null)
            {
                ed.AxonometryAxes = AxonometryAxes.Clone();
            }
            return ed;
        }

        private void NameNewItemsChanged(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            InputMgr.NameNewItems = nameNewItemsCbx.IsChecked == true;
        }

        private async void OOBEPanViewportClosed(object sender, EventArgs e)
        {
            await Task.Delay(200);
            OOBEStyleSelector.OpenIfTrue(ref Settings.OOBEState.styleSelector);
        }
    }

    public interface IChangesWithDarkMode
    {
        void SwapWhiteAndBlack();
    }
}
