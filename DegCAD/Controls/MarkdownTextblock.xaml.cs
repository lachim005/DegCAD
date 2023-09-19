using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DegCAD.Controls
{
    /// <summary>
    /// Interaction logic for MarkdownTextblock.xaml
    /// </summary>
    public partial class MarkdownTextblock : UserControl
    {


        public string Text
        {
            get 
            { 
                return (string)GetValue(TextProperty); 
            }
            set 
            { 
                SetValue(TextProperty, value);
                UpdateText();
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MarkdownTextblock),
                new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnFirstPropertyChanged)));

        private static void OnFirstPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not MarkdownTextblock mtb) return;
            mtb.UpdateText();
        }

        public MarkdownTextblock()
        {
            InitializeComponent();
            
        }

        public void UpdateText()
        {
            string t = Text + ' ';
            innerTextBlock.Inlines.Clear();
            Run currentRun = new();
            char mode = 'd';
            for (int i = 0; i < t.Length-1; i++)
            {
                //Start heading
                if (t[i] == '#' && t[i+1] == ' ')
                {
                    i++;
                    currentRun.Text += '\n';
                    innerTextBlock.Inlines.Add(currentRun);
                    mode = 'h';
                    currentRun = new() { FontSize = 24, FontWeight = FontWeights.Bold };
                    continue;
                }
                //Star
                if (mode == 'd' && t[i] == '*')
                {
                    innerTextBlock.Inlines.Add(currentRun);
                    //Start bold
                    if (t[i+1] == '*')
                    {
                        i++;
                        mode = 'b';
                        currentRun = new() { FontWeight = FontWeights.Bold };
                        continue;
                    }
                    //Start italic
                    mode = 'i';
                    currentRun = new() { FontStyle = FontStyles.Italic };
                    continue;
                }
                //End bold
                if (mode == 'b' && t[i] == '*' && t[i+1] == '*')
                {
                    i++;
                    innerTextBlock.Inlines.Add(currentRun);
                    mode = 'd';
                    currentRun = new();
                    continue;
                }
                //End italic
                if (mode == 'i' && t[i] == '*')
                {
                    innerTextBlock.Inlines.Add(currentRun);
                    mode = 'd';
                    currentRun = new();
                    continue;
                }
                //End header
                if (mode == 'h' && t[i] == '\n') 
                {
                    currentRun.Text += '\n';
                    innerTextBlock.Inlines.Add(currentRun);
                    currentRun = new();
                    mode = 'd';
                    continue;
                }
                currentRun.Text += t[i];
            }
            innerTextBlock.Inlines.Add(currentRun);
        }
    }
}
