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

namespace DegCAD.Guides
{
    /// <summary>
    /// Interaction logic for EditorGuideView.xaml
    /// </summary>
    public partial class EditorGuideView : UserControl
    {
        Timeline clonedTl;
        Guide guide;
        ViewPort vp;
        GuideStep selectedStep;

        public EditorGuideView(Timeline tl, Guide g)
        {
            InitializeComponent();

            selectedStep = g.Steps[0];
            guide = g;

            clonedTl = tl.Clone();
            clonedTl.UndoneCommands.Clear();

            vp = new(clonedTl);
            clonedTl.SetViewportLayer(vp.Layers[1]);
            vpBorder.Child = vp;

            stepButtonsIc.ItemsSource = guide.Steps;
        }

        private void StepButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (b.DataContext is not GuideStep step) return;

            SelectStep(step);
        }

        private void SelectStep(GuideStep step)
        {
            stepButtonsIc.UpdateLayout();
            for (int i = 0; i < stepButtonsIc.Items.Count; i++)
            {
                var cp = (ContentPresenter)stepButtonsIc.ItemContainerGenerator.ContainerFromIndex(i);
                var it = cp.ContentTemplate.FindName("stepButton", cp);
                if (it is not Button btn) break;
                btn.IsEnabled = (step.Position - 1 == i) ? false : true;
            }

            stepDisplay.DataContext = step;
            selectedStep = step;
            UpdateDrawingState();
        }
        private void UpdateDrawingState()
        {
            while (clonedTl.CanUndo) clonedTl.Undo();
            for (int i = 0; i < guide.Steps.Count; i++)
            {
                for (int j = 0; j < guide.Steps[i].Items; j++)
                {
                    clonedTl.Redo();
                    if (!clonedTl.CanRedo) return;
                }
                if (guide.Steps[i] == selectedStep) break;
            }
        }
    }
}
