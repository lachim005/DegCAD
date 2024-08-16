using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace DegCAD.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            versionTb.Text = $"Verze: {version}";
        }


        public static void Open(Window owner)
        {
            AboutDialog dialog = new(owner);
            dialog.ShowDialog();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
