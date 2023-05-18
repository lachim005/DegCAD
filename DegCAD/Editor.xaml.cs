using DegCAD.DrawableItems;
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
        public Timeline Timeline { get; protected set; }
        public Snapper Snapper { get; protected set; }
        public LabelManager LabelManager { get; protected set; }

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

        public Editor(string fileName)
        {
            InitializeComponent();
            viewPort.ViewportChanged += ViewPortChanged;
            Timeline = new();
            Timeline.TimelineChanged += TimelineChanged;
            Snapper = new(Timeline);
            InputMgr = new(viewPort, Snapper, styleSelector);
            viewPort.SizeChanged += ViewPortChanged;
            LabelManager = new(Timeline, viewPort.Layers[1], viewPort, this);

            _fileName = fileName;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddAxis()
        {
            Timeline.AddCommand(new TimelineItem(new IMongeItem[3]
            {
                new Axis(),
                new MongeItems.Label("x", "1, 2", "", (8,0), DegCAD.Style.Default, new LineProjection(new((0,0), (1,0)), false, DegCAD.Style.Default)),
                new MongeItems.Label("0", "", "", (0,0), DegCAD.Style.Default, new DrawableItems.Point(0,0)),
            }));
        }
        public void ViewPortChanged(object? sender, EventArgs e)
        {
            Redraw();
        }
        public void TimelineChanged(object? sender, EventArgs e)
        {
            Redraw();
            Changed = true;
        }

        public void Redraw()
        {
            foreach (var cmd in Timeline.CommandHistory)
            {
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    cmd.Items[i].Draw(viewPort.Layers[1]);
                }
            }
        }

        public async void ExecuteCommand(IGeometryCommand command)
        {
            Debug.WriteLine($"Executing command: {command}");
            ExecutingCommand = true;

            statusBar.ShowCommandStatus();
            var res = await command.ExecuteAsync(InputMgr.PreviewGd, InputMgr, statusBar);
            ExecutingCommand = false;
            statusBar.HideCommandStatus();
            statusBar.CommandName = "";
            statusBar.CommandHelp = "";

            if (res is null)
                return;
            Timeline.AddCommand(res);
        }
    }
}
