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
        private static bool lastHighlightCurrentStep = false;

        Timeline clonedTl;
        Guide guide;
        ViewPort vp;
        GuideStep selectedStep;
        /// <summary>
        /// Creates an editor guide view
        /// </summary>
        public EditorGuideView(ViewPort evp, Guide g) : this(evp, g, g.Steps[Math.Clamp(g.LastViewedStep, 0, g.Steps.Count - 1)])
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

            if (lastHighlightCurrentStep)
            {
                highlightStepCbx.IsChecked = true;
            }
        }

        private void StepButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (b.DataContext is not GuideStep step) return;

            SelectStep(step);
        }

        private void SelectStep(GuideStep step)
        {
            if (highlightStepCbx.IsChecked == true)
            {
                HighlightCurrentStep(false);
            }

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

            guide.LastViewedStep = guide.Steps.IndexOf(step);

            if (highlightStepCbx.IsChecked == true)
            {
                HighlightCurrentStep(true);
            }
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
            highlightStepCbx.IsChecked = egv.highlightStepCbx.IsChecked;
            vp.OffsetX = egv.vp.OffsetX;
            vp.OffsetY = egv.vp.OffsetY;
            vp.Redraw();
        }
        public void SwapWhiteAndBlack()
        {
            vp.SwapWhiteAndBlack();
        }

        private void HighlightCurrentStep(bool highlight)
        {
            int sum = 1;
            foreach (var step in guide.Steps)
            {
                if (ReferenceEquals(step, selectedStep)) break;
                sum += step.Items;
            }

            for (int i = sum == 1 ? -1 : 0; i < selectedStep.Items; i++)
            {
                if (i + sum >= vp.Timeline.CommandHistory.Count)
                {
                    return;
                }

                foreach (var item in vp.Timeline.CommandHistory[i + sum].Items)
                {
                    if (item is not GeometryElement ge) continue;
                    ge.IsHighlighted = highlight;
                }
            }
        }

        private void HighlightCurrentStepChecked(object sender, RoutedEventArgs e)
        {
            HighlightCurrentStep(true);
            vp.AllowLabelInteractions = false;
            lastHighlightCurrentStep = true;
        }

        private void HighlightCurrentStepUnchecked(object sender, RoutedEventArgs e)
        {
            HighlightCurrentStep(false);
            vp.AllowLabelInteractions = true;
            lastHighlightCurrentStep = false;
        }
    }
}
