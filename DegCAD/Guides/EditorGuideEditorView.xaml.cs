using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public partial class EditorGuideEditorView : UserControl, IChangesWithDarkMode
    {
        private Guide guide;
        private ViewPort vp;
        private Timeline clonedTl;

        private GuideStep selectedStep = new();

        public EditorGuideEditorView(ViewPort evp, Guide g)
        {

            InitializeComponent();

            stepEditor.DataContext = selectedStep;
            guide = g;

            vp = evp.Clone();

            clonedTl = vp.Timeline;
            clonedTl.UndoneCommands.Clear();
            
            vpBorder.Child = vp;

            stepsIc.ItemsSource = guide.Steps;
        }

        private void StepButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button senderBtn) return;
            if (senderBtn.DataContext is not GuideStep step) return;

            SelectStep(step);
        }

        private void SelectStep(GuideStep step)
        {
            stepsIc.UpdateLayout();
            for (int i = 0; i < stepsIc.Items.Count; i++)
            {
                var cp = (ContentPresenter)stepsIc.ItemContainerGenerator.ContainerFromIndex(i);
                var it = cp.ContentTemplate.FindName("stepButton", cp);
                if (it is not Button btn) break;
                btn.IsEnabled = (step.Position - 1 == i) ? false : true;
            }

            stepEditor.DataContext = step;
            selectedStep = step;
            stepEditor.Visibility = Visibility.Visible;
            UpdateDrawingState();

            guide.LastEditedStep = guide.Steps.IndexOf(step);
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

        private void AddStep(object sender, RoutedEventArgs e)
        {
            GuideStep newStep = new() { Position = guide.Steps.Count + 1 };
            guide.Steps.Add(newStep);
            SelectStep(newStep);
        }

        private void RemoveStep(object sender, RoutedEventArgs e)
        {
            int reselectIndex = guide.Steps.IndexOf(selectedStep) - 1;
            if (reselectIndex < 0) reselectIndex = 0;
            
            guide.Steps.Remove(selectedStep);

            if (guide.Steps.Count == 0)
                AddStep(this, new());

            for (int i = 0; i < guide.Steps.Count; i++)
            {
                guide.Steps[i].Position = i + 1;
            }

            SelectStep(guide.Steps[reselectIndex]);
        }

        private void DecrementItemCount(object sender, RoutedEventArgs e)
        {
            if (selectedStep.Items < 1) return;
            selectedStep.Items--;
            UpdateDrawingState();
        }

        private void IncrementItemCount(object sender, RoutedEventArgs e)
        {
            selectedStep.Items++;
            UpdateDrawingState();
        }

        public void SwapWhiteAndBlack()
        {
            vp.SwapWhiteAndBlack();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (guide.Steps.Count > 0)
            {
                SelectStep(guide.Steps[Math.Clamp(guide.LastEditedStep, 0, guide.Steps.Count - 1)]);
            }
        }

        private void CenterScreenClick(object sender, RoutedEventArgs e)
        {
            vp.CenterContent();
        }
    }
}
