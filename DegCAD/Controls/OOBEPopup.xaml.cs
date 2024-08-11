using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for OOBEPopup.xaml
    /// </summary>
    public partial class OOBEPopup : Popup
    {
        private Window window;

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(OOBEPopup), new PropertyMetadata(""));

        public string Body
        {
            get { return (string)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(string), typeof(OOBEPopup), new PropertyMetadata(""));


        public ArrowLocation ArrowPosition
        {
            get { return (ArrowLocation)GetValue(ArrowPositionProperty); }
            set 
            { 
                SetValue(ArrowPositionProperty, value);
                
            }
        }

        public static readonly DependencyProperty ArrowPositionProperty =
            DependencyProperty.Register("ArrowPosition", typeof(ArrowLocation), typeof(OOBEPopup), new PropertyMetadata(ArrowLocation.None, ArrowPropertyChanged));

        private static void ArrowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not OOBEPopup popup) return;
            if (e.NewValue is not ArrowLocation pos) return;

            popup.arrowLeft.Visibility = Visibility.Collapsed;
            popup.arrowTop.Visibility = Visibility.Collapsed;
            popup.arrowRight.Visibility = Visibility.Collapsed;
            popup.arrowBottom.Visibility = Visibility.Collapsed;
            switch (pos)
            {
                case ArrowLocation.Left:
                    popup.arrowLeft.Visibility = Visibility.Visible;
                    break;
                case ArrowLocation.Top:
                    popup.arrowTop.Visibility = Visibility.Visible;
                    break;
                case ArrowLocation.Right:
                    popup.arrowRight.Visibility = Visibility.Visible;
                    break;
                case ArrowLocation.Bottom:
                    popup.arrowBottom.Visibility = Visibility.Visible;
                    break;
            }
        }

        public OOBEPopup()
        {
            DataContext = this;
            InitializeComponent();
        }

        public void OpenIfTrue(ref bool open)
        {
            if (!open) return;
            open = false;
            IsOpen = true;
        }

        private void WindowClick(object sender, MouseButtonEventArgs e)
        {
            IsOpen = false;
        }

        public enum ArrowLocation
        {
            Left, Top, Right, Bottom, None
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            window = Window.GetWindow(this);
            window.PreviewMouseLeftButtonUp += WindowClick;
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            window.PreviewMouseLeftButtonUp -= WindowClick;
        }
    }
}
