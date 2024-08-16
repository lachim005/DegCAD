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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFImage.xaml
    /// </summary>
    public partial class MFImage : MFItem, INotifyPropertyChanged
    {
        private Stretch _stretch = Stretch.Uniform;

        public BitmapImage imageSource;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Stretch Stretch
        {
            get => _stretch; 
            set
            {
                _stretch = value;
                PropertyChanged?.Invoke(this, new(nameof(Stretch)));
            }
        }

        public int StretchIndex
        {
            get
            {
                return _stretch switch
                {
                    Stretch.UniformToFill => 1,
                    Stretch.Fill => 2,
                    _ => 0
                };
            }
            set
            {
                Stretch = value switch
                {
                    1 => Stretch.UniformToFill,
                    2 => Stretch.Fill,
                    _ => Stretch.Uniform,
                };
            }
        }

        public MFImage(string path)
        {
            InitializeComponent();
            DataContext = this;

            imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.UriSource = new(path);
            imageSource.CacheOption = BitmapCacheOption.OnLoad;
            imageSource.EndInit();

            imgControl.Source = imageSource;
        }

        public MFImage(BitmapImage image)
        {
            InitializeComponent();

            imageSource = image.Clone();
            imgControl.Source = image;
        }

        public override MFItem Clone()
        {
            return new MFImage(imageSource.Clone()) { Stretch = Stretch };
        }

        public override void SwapWhiteAndBlack()
        {
            
        }

        public override void ViewUpdated(double offsetX, double offsetY, double scale)
        {
            
        }
    }
}
