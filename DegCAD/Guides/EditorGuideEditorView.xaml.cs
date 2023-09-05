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

namespace DegCAD.Guides
{
    /// <summary>
    /// Interaction logic for EditorGuideEditorView.xaml
    /// </summary>
    public partial class EditorGuideEditorView : UserControl
    {
        private Guide guide;
        private ViewPort vp;
        private Timeline clonedTl;

        private GuideStep selectedStep = new();

        public EditorGuideEditorView(Timeline tl, Guide g)
        {

            InitializeComponent();

            stepEditor.DataContext = selectedStep;
            guide = g;

            clonedTl = tl.Clone();
            clonedTl.UndoneCommands.Clear();

            vp = new(clonedTl);
            clonedTl.SetViewportLayer(vp.Layers[1]);
            vpBorder.Child = vp;

            stepsIc.ItemsSource = guide.Steps;
        }

        private void StepButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button senderBtn) return;
            if (senderBtn.DataContext is not GuideStep step) return;

            for (int i = 0; i < stepsIc.Items.Count; i++)
            {
                var cp = (ContentPresenter)stepsIc.ItemContainerGenerator.ContainerFromIndex(i);
                var it = cp.ContentTemplate.FindName("stepButton", cp);
                if (it is not Button btn) break;
                btn.IsEnabled = true;
            }

            senderBtn.IsEnabled = false;
            StepSelected(step);
        }

        private void StepSelected(GuideStep step)
        {
            stepEditor.DataContext = step;
            selectedStep = step;
        }

        private void AddStep(object sender, RoutedEventArgs e)
        {
            guide.Steps.Add(new() { Position = guide.Steps.Count + 1 });
        }

        private void RemoveStep(object sender, RoutedEventArgs e)
        {
            guide.Steps.Remove(selectedStep);
            for (int i = 0; i < guide.Steps.Count; i++)
            {
                guide.Steps[i].Position = i + 1;
            }
        }
    }
}
