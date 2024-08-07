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

namespace DegCAD.MultiFile
{
    /// <summary>
    /// Interaction logic for MFImage.xaml
    /// </summary>
    public partial class MFImage : MFItem
    {
        public BitmapImage imageSource;

        public MFImage(string path)
        {
            InitializeComponent();

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
            return new MFImage(imageSource.Clone());
        }

        public override void SwapWhiteAndBlack()
        {
            
        }

        public override void ViewUpdated(double offsetX, double offsetY, double scale)
        {
            
        }
    }
}
