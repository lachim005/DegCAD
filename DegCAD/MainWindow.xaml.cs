using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The editor that the user has currently open
        /// </summary>
        public Editor? ActiveEditor { get; set; }
        public ITab ActiveTab { get; set; } = new HomeTab();
        public readonly ObservableCollection<ITab> openTabs = new();
        private readonly ObservableCollection<ToastNotification> toastNotifications = new();

        /// <summary>
        /// Used for the number after "Bez názvu" in new document names
        /// </summary>
        private int editorCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            cmdPallete.GenerateCommands(this);
            cmdPallete.ShowButtons(FileType.None);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show($"{e.ExceptionObject}");

            //Open editor is the user opens a file
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                if (System.IO.File.Exists(args[1]))
                {
                    OpenFileAsync(args[1]);
                }
            }

            editorTabs.ItemsSource = openTabs;
            toastNotificationsIc.ItemsSource = toastNotifications;
#if !DEBUG
            CheckForNewVersion();
#endif
        }

        private void TabSwitched(object sender, SelectionChangedEventArgs e)
        {
            //Home or invalid tab got selected
            if (editorTabs.SelectedIndex < 0)
            {
                ActiveEditor = null;
                ActiveTab = new HomeTab();
                homePage.Visibility = Visibility.Visible;
                cmdPallete.ShowButtons(FileType.None);
                return;
            }

            ActiveTab = openTabs[editorTabs.SelectedIndex];
            homePage.Visibility = Visibility.Hidden;

            //Editor tab got selected
            if (ActiveTab is EditorTab et)
            {
                ActiveEditor = et.Editor;
                cmdPallete.ShowButtons((FileType)ActiveEditor.ProjectionType);
            }
            else if (ActiveTab is ConnectedEditorTab cet)
            {
                ActiveEditor = cet.Editor;
                cmdPallete.ShowButtons((FileType)ActiveEditor.ProjectionType);
            }
            else if (ActiveTab is MFEditorTab mf)
            {
                cmdPallete.ShowButtons(FileType.MultiFile);
            }

            ActiveTab.TabSelected();
        }

        private async void EditorTabCloseClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not ITab tab) return;
            if (await CanCloseTab(tab))
                openTabs.Remove(tab);
        }

        private void WindowDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = false;
                return;
            }
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files is null) return;
            e.Handled = true;

            foreach (var file in files)
            {
                OpenFileAsync(file);
            }
        }

        private async void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < openTabs.Count; i++)
            {
                editorTabs.SelectedIndex = i;
                
                if (!(await CanCloseTab(openTabs[i])))
                {
                    e.Cancel = true;
                }
            }
        }

        private void StartTabReorderDrag(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            //Boxes the tab into a tuple because it has trouble with inheritance
            DragDrop.DoDragDrop(f, new Tuple<ITab>((ITab)f.DataContext), DragDropEffects.Move);
        }
        private void TabDrop(object sender, DragEventArgs e)
        {
            //Gets the dragged and dropped tab
            var dragTab = (e.Data.GetData(typeof(Tuple<ITab>)) as Tuple<ITab>)?.Item1;
            if (dragTab is null) return;
            if ((sender as FrameworkElement)?.DataContext is not ITab dropTab) return;

            int dragIndex = openTabs.IndexOf(dragTab);
            int dropIndex = openTabs.IndexOf(dropTab);
            if (dragIndex == -1 || dropIndex == -1) return;
            if (dragIndex == dropIndex) return;

            openTabs.Move(dragIndex, dropIndex);
            editorTabs.SelectedIndex = dropIndex;
        }

        private async void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle) return;
            if (sender is not FrameworkElement f) return;
            if (f.DataContext is not ITab tab) return;
            if (await CanCloseTab(tab))
            {

                openTabs.Remove(tab);
            }
        }

        private async Task<bool> CanCloseTab(ITab tab)
        {
            if (!tab.HasChanges) return true;
            var save = MessageBox.Show(
                $"Chcete uložit soubor {tab.Name}?",
                "DegCAD",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning
            );


            switch (save)
            {
                case MessageBoxResult.Yes:
                    return await tab.Save();
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
            }
            return false;
        }

        private void ChangeSkin(object sender, RoutedEventArgs e)
        {
            if (Application.Current is not App app) return;
            
            app.ChangeSkin((App.Skin == Skin.Light) ? Skin.Dark : Skin.Light);
            foreach (var tab in openTabs)
            {
                if (tab is EditorTab et)
                {
                    et.Editor.viewPort.SwapWhiteAndBlack();
                    et.Editor.styleSelector.SwapWhiteAndBlack();
                }
            }
        }

        private void CloseToastNotification(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (b.DataContext is not ToastNotification tn) return;

            toastNotifications.Remove(tn);
        }

        private void ToastNotificationButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (b.DataContext is not ToastNotification tn) return;

            toastNotifications.Remove(tn);
            tn.ButtonAction();
        }

        private async void CheckForNewVersion()
        {
            try
            {
                using System.Net.Http.HttpClient client = new();
                string res = await client.GetStringAsync("https://degcad.cz/newestVersion.txt");
                Version newestVer = new(res);
                var currentVer = Assembly.GetExecutingAssembly().GetName().Version;
                if (newestVer > currentVer)
                {
                    toastNotifications.Add(new(
                        $"Je k dispozici nová verze DegCADu {newestVer}.\nStáhnout ji můžete na webu degcad.cz.\n", 
                        "Nová verze", 
                        "Stáhnout", 
                        () => Process.Start(new ProcessStartInfo() { FileName = "https://degcad.cz/download.php", UseShellExecute = true })));
                }
            }
            catch
            {

            }
        }
    }
}
