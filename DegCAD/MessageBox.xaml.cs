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
using System.Windows.Shapes;

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public MessageBox(string message, string title, MessageBoxButton btn, MessageBoxImage img)
        {
            InitializeComponent();

            textbox.Text = message;
            Title = title;

            switch (btn)
            {
                case MessageBoxButton.OK:
                    okBtn.Visibility = Visibility.Visible;
                    okBtn.Focus();
                    break;
                case MessageBoxButton.OKCancel: 
                    okBtn.Visibility = Visibility.Visible;
                    cancelBtn.Visibility = Visibility.Visible;
                    okBtn.Focus();
                    break;
                case MessageBoxButton.YesNoCancel:
                    yesBtn.Visibility = Visibility.Visible;
                    noBtn.Visibility = Visibility.Visible;
                    cancelBtn.Visibility = Visibility.Visible;
                    yesBtn.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    yesBtn.Visibility = Visibility.Visible;
                    noBtn.Visibility = Visibility.Visible;
                    yesBtn.Focus();
                    break;
            }


            switch ((int)img)
            {
                case 0:
                    iconContainer.Visibility = Visibility.Collapsed;
                    break;
                case 16:// (x)
                    dangerIcon.Visibility = Visibility.Visible;
                    break;
                case 32:// (?)
                    questionIcon.Visibility = Visibility.Visible;
                    break;
                case 48:// /!\
                    warningIcon.Visibility = Visibility.Visible;
                    break;
                case 64:// (i)
                    informationIcon.Visibility = Visibility.Visible;
                    break;
            }
        }

        public static MessageBoxResult Show(string message, string title = "DegCAD", MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)
        {
            MessageBox mb = new(message, title, btn, img);
            mb.ShowDialog();
            return mb.Result;
        }
        public async static Task<MessageBoxResult> ShowAsync(string message, string title = "DegCAD", MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)
        {
            MessageBox mb = new(message, title, btn, img);
            await mb.Dispatcher.BeginInvoke(new Action(() => mb.ShowDialog()));
            return mb.Result;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void YesClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void NoClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                return;
            }
        }
    }
}
