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
    /// Interaction logic for EditorGuideView.xaml
    /// </summary>
    public partial class EditorGuideView : UserControl, IChangesWithDarkMode
    {
        Timeline clonedTl;
        Guide guide;
        ViewPort vp;
        GuideStep selectedStep;
        /// <summary>
        /// Creates an editor guide view
        /// </summary>
        public EditorGuideView(ViewPort evp, Guide g) : this(evp, g, g.Steps[0])
        {
            topBar.Visibility = Visibility.Visible;
            fullscreenBtn.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Creates a fullscreen guide view
        /// </summary>
        public EditorGuideView(ViewPort evp, Guide g, GuideStep step)
        {
            InitializeComponent();

            selectedStep = step;
            guide = g;
            stepsProgressBar.Maximum = g.Steps.Count;


            vp = evp.Clone();

            clonedTl = vp.Timeline;
            clonedTl.UndoneCommands.Clear();

            vpBorder.Child = vp;

            stepButtonsIc.ItemsSource = guide.Steps;
            stepDisplay.DataContext = selectedStep;

            topBar.Visibility = Visibility.Collapsed;
            fullscreenBtn.Visibility = Visibility.Collapsed;
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
            if (topBar.Visibility != Visibility.Collapsed)
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

            if (step.Position == 1)
                prevStepBtn.IsEnabled = false;
            else
                prevStepBtn.IsEnabled = true;

            if (step.Position == guide.Steps.Count)
                nextStepBtn.IsEnabled = false;
            else
                nextStepBtn.IsEnabled = true;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SelectStep(selectedStep);
        }

        private void PrevStep(object sender, RoutedEventArgs e)
        {
            SelectStep(guide.Steps[selectedStep.Position - 2]);
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            SelectStep(guide.Steps[selectedStep.Position]);
        }

        private void ShowFullscreen(object sender, RoutedEventArgs e)
        {
            var step = selectedStep;
            //Selects the last step to pass the whole timeline
            SelectStep(guide.Steps.Last());

            EditorGuideView egv = new(vp.Clone(), guide, step);
            FullscreenPresenter fs = new(egv);
            fs.Owner = Window.GetWindow(this);
            fs.ShowDialog();

            SelectStep(egv.selectedStep);
        }
        public void SwapWhiteAndBlack()
        {
            vp.SwapWhiteAndBlack();
        }
    }
}
