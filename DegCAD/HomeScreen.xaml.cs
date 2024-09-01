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
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        MainWindow mw;
        public HomeScreen(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            // Sets the greeting depending on the time of day
            greetingsTextblock.Text = DateTime.Now.Hour switch
            {
                (< 4) or (>= 18) => "Dobrý večer,",
                < 11 => "Dobré ráno,",
                < 13 => "Dobré poledne,",
                _ => "Dobré odpoledne,"
            } + " vítejte v DegCADu!";

            recentFilesIC.ItemsSource = Settings.RecentFiles.Files;

            OOBENewFile.OpenIfTrue(ref Settings.OOBEState.newFile);
            notificationIc.ItemsSource = mw.Notifications;
            mw.Notifications.CollectionChanged += NotificationsChanged;
            UpdateNotifications();
        }

        private void NotificationsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateNotifications();   
        }
        private void UpdateNotifications()
        {
            if (Settings.FetchingNotificationsFailed)
            {
                notificationsPlaceholder.Visibility = Visibility.Visible;
                notificationsPlaceholder.Text = "Při stahování oznámění se vyskytla chyba";
                return;
            }
            if (mw.Notifications.Count == 0)
            {
                notificationsPlaceholder.Visibility = Visibility.Visible;
                notificationsPlaceholder.Text = "Zatím tu nic není";
                return;
            }
            notificationsPlaceholder.Visibility = Visibility.Collapsed;

            int unSeenCount = 0;
            foreach (var not in mw.Notifications)
            {
                if (!not.Seen) unSeenCount++;
            }
            if (unSeenCount == 0) return;

            notificationBubble.Visibility = Visibility.Visible;
            notificationBubbleText.Text = unSeenCount.ToString();

        }


        private void NewPlaneClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(mw, ProjectionType.Plane));
        }

        private void NewMongeClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(mw, ProjectionType.Monge));
        }

        private void NewAxoClick(object sender, RoutedEventArgs e)
        {
            mw.AddEditor(MainWindow.CreateNewEditor(mw, ProjectionType.Axonometry));
        }

        private void NewCompositionClick(object sender, RoutedEventArgs e)
        {
            mw.AddComposition(new(mw));
        }

        private void RemoveRecentFile(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;
            Settings.RecentFiles.RemoveFile(rf.Path);
        }

        private void OpenRecentFileInBackground(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;

            mw.OpenFileAsync(rf.Path, mw, false);
        }

        private void RecentFileClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;

            mw.OpenFileAsync(rf.Path, mw);
        }

        private void RecentFilePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Open the recent file in the background on middle click
            if (e.ChangedButton != MouseButton.Middle) return;
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not RecentFile rf) return;

            mw.OpenFileAsync(rf.Path, mw, false);
            e.Handled = true;
        }

        private void ClearRecentFilesClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(mw, "Opravdu chcete vymazat historii souborů?", "DegCAD", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Settings.RecentFiles.Clear();
            }
        }

        private void OpenNotifications(object sender, RoutedEventArgs e)
        {
            if (notificationsPanel.Visibility == Visibility.Visible)
            {
                notificationsPanel.Visibility = Visibility.Collapsed;
                return;
            }
            notificationsPanel.Visibility = Visibility.Visible;
            notificationBubble.Visibility = Visibility.Collapsed;

            if (Settings.FetchingNotificationsFailed) return;

            Settings.SeenNotifications.Clear();
            foreach (var not in mw.Notifications)
            {
                not.Seen = true;
                Settings.SeenNotifications.Add(not.GUID);
            }
        }

        private void NotificationButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not Notification not) return;
            Process.Start(new ProcessStartInfo() { FileName = not.ButtonLink, UseShellExecute = true });
        }
    }
}
