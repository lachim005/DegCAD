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
        private static int editorCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            Settings.LoadSettings();

            cmdPallete.GenerateCommands(this);
            cmdPallete.ShowButtons(FileType.None);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show($"{e.ExceptionObject}");

            openTabs.Add(new HomeTab());

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
                if (openTabs.Count == 0)
                {
                    openTabs.Add(new HomeTab());
                    editorTabs.SelectedIndex = 0;
                }
                return;
            }

            ActiveTab = openTabs[editorTabs.SelectedIndex];

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
            } else
            {
                cmdPallete.ShowButtons(FileType.None);
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
                    return;
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

        public void ChangeSkin(Skin skin)
        {
            if (Application.Current is not App app) return;

            var prevSkin = App.Skin;
            app.ChangeSkin(skin);
            if (prevSkin == skin) return;

            foreach (var tab in openTabs)
            {
                tab.SwapWhiteAndBlack();
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
            if (!Settings.AlertNewVersions) return;
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
