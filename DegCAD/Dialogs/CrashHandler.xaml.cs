using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for CrashHandler.xaml
    /// </summary>
    public partial class CrashHandler : Window
    {
        DispatcherUnhandledExceptionEventArgs e;

        public CrashHandler(DispatcherUnhandledExceptionEventArgs e)
        {
            InitializeComponent();

            this.e = e;
            exceptionTbx.Text = e.Exception.ToString();
        }

        public static void Open(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CrashHandler ch = new(e);
            ch.ShowDialog();
        }

        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void ExitProgramClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContinueRunningClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("Program nemusí kvůli chybě fungovat správně.\nDoporučujeme ho co nejdříve restartovat.", "Varování", img: MessageBoxImage.Warning);
            this.e.Handled = true;
        }
    }
}
