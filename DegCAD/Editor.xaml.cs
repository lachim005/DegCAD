using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
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
    public partial class Editor : UserControl
    {
        public GeometryDrawer GeometryDrawer { get; set; }
        public GeometryInputManager InputMgr { get; set; }
        public Timeline Timeline { get; set; }
        public Snapper Snapper { get; set; }
        public LabelManager LabelManager { get; set; }

        public bool ExecutingCommand { get; private set; } = false;

        public Editor()
        {
            InitializeComponent();
            GeometryDrawer = new(viewPort, false);
            viewPort.ViewportChanged += ViewPortChanged;
            Timeline = new();
            Snapper = new(Timeline);
            InputMgr = new(viewPort, Snapper);
            viewPort.SizeChanged += ViewPortChanged;
            LabelManager = new(Timeline, GeometryDrawer, viewPort, this);

            //Adds the axis
            Timeline.AddCommand(new TimelineItem(new IMongeItem[3]
            {
                new Axis(),
                new MongeItems.Label("x", "1, 2", "", (8,0), DegCAD.Style.Default),
                new MongeItems.Label("0", "", "", (0,0), DegCAD.Style.Default),
            }));;
        }

        public void ViewPortChanged(object? sender, EventArgs e)
        {
            Redraw();
        }

        public void Redraw()
        {
            GeometryDrawer.Clear();
            foreach (var cmd in Timeline.CommandHistory)
            {
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    cmd.Items[i].Draw(GeometryDrawer);
                }
            }
        }

        public async void ExecuteCommand(IGeometryCommand command)
        {
            Debug.WriteLine($"Executing command: {command}");
            ExecutingCommand = true;
            var res = await command.ExecuteAsync(InputMgr.PreviewGd, InputMgr);
            if (res is null)
                return;
            Timeline.AddCommand(res);
            ExecutingCommand = false;
            Redraw();
        }
    }
}
