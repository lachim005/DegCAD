using DegCAD.MongeItems;
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

        public GeometryInputManager InputMgr { get; protected set; }
        public Timeline Timeline => viewPort.Timeline;
        public Snapper Snapper { get; protected set; }
        public ProjectionType ProjectionType { get; set; }
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
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddAxis(ViewportLayer vpl)
        {
            switch (ProjectionType)
            {
                case ProjectionType.Plane:
                    Timeline.AddCommand(new TimelineItem(new IMongeItem[]
                    {
                        new Axis(),
                        new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default, vpl),
                        new InfiniteLine(new((0,0), (0,1)), DegCAD.Style.Default, vpl),
                        new MongeItems.Label("x", "", "", (8,0), DegCAD.Style.Default, new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default), vpl),
                        new MongeItems.Label("y", "", "", (0,-8), DegCAD.Style.Default, new InfiniteLine(new((0,0), (0,1)), DegCAD.Style.Default), vpl),
                        new MongeItems.Label("0", "", "", (0,0), DegCAD.Style.Default, new MongeItems.Point(0,0), vpl),
                    }));
                    break;
                case ProjectionType.Monge:
                    Timeline.AddCommand(new TimelineItem(new IMongeItem[]
                    {
                        new Axis(),
                        new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default, vpl),
                        new MongeItems.Point(0, 0, DegCAD.Style.Default, vpl),
                        new MongeItems.Label("x", "1, 2", "", (8,0), DegCAD.Style.Default, new InfiniteLine(new((0,0), (1,0)), DegCAD.Style.Default), vpl),
                        new MongeItems.Label("0", "", "", (0,0), DegCAD.Style.Default, new MongeItems.Point(0,0), vpl),
                    }));
                    break;
            }
            
        }
        public void TimelineChanged(object? sender, EventArgs e)
        {
            Changed = true;
        }


        public async void ExecuteCommand(IGeometryCommand command)
        {
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
                    if (ex is not CommandCanceledException) throw new Exception("Command thrown an exception", ex);
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
                    //Reenables moving labels
                    foreach (var layer in viewPort.Layers)
                    {
                        layer.Canvas.IsHitTestVisible = true;
                    }
                }    
            } while (repeatCommandCbx.IsChecked == true);
        }

        public void ShowView(Control view, string viewTitle, bool changed = true)
        {
            ExecutingCommand = true;
            mainView.Visibility = Visibility.Collapsed;
            altViewGrid.Visibility = Visibility.Visible;
            altView.Child = view;
            altViewTitle.Content = viewTitle;
            if (changed)
                Changed = true;
        }

        private void ExitView(object sender, RoutedEventArgs e)
        {
            altViewGrid.Visibility = Visibility.Collapsed;
            mainView.Visibility = Visibility.Visible;
            altView.Child = null;
            ExecutingCommand = false;
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
                MessageBox.Show("Návod neobsahuje žádné kroky", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EditorGuideView gv = new(viewPort, Guide);
            ShowView(gv, "Návod", false);
        }
        private void RemoveGuideBtn(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Opravdu chcete vymazat návod?\nTato akce je nevratná", "Varování", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
                Guide = null;
        }
        public void PromptGuide()
        {
            if (Guide is not Guide g) return;
            if (g.Steps.Count == 0) return;
            if (!OpenGuideDialog.OpenDialog()) return;
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
    }
}
