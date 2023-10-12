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
            var axis = new Axis(DegCAD.Style.Default, vpl);
            Timeline.AddCommand(new TimelineItem(new IMongeItem[3]
            {
                axis,
                new MongeItems.Label("x", "1, 2", "", (8,0), DegCAD.Style.Default, new LineProjection(new((0,0), (1,0)), false, DegCAD.Style.Default), vpl),
                new MongeItems.Label("0", "", "", (0,0), DegCAD.Style.Default, new MongeItems.Point(0,0), vpl),
            }));
        }
        public void TimelineChanged(object? sender, EventArgs e)
        {
            Changed = true;
        }


        public async void ExecuteCommand(IGeometryCommand command)
        {
            Debug.WriteLine($"Executing command: {command}");
            ExecutingCommand = true;

            statusBar.ShowCommandStatus();
            var res = await command.ExecuteAsync(viewPort.Layers[2], viewPort.Layers[1], viewPort.Layers[0], InputMgr, statusBar);
            viewPort.Layers[2].Canvas.Children.Clear();
            viewPort.Layers[0].Canvas.Children.Clear();
            ExecutingCommand = false;
            statusBar.HideCommandStatus();
            statusBar.CommandName = "";
            statusBar.CommandHelp = "";

            if (res is null)
                return;
            Timeline.AddCommand(res);
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
            fs.ShowDialog();
        }
    }
}
