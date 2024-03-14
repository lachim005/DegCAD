using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DegCAD.Controls
{
    public class AspectRationBox : Decorator
    {
        public double AspectWidth
        {
            get { return (double)GetValue(AspectWidthProperty); }
            set { SetValue(AspectWidthProperty, value); }
        }


        // Using a DependencyProperty as the backing store for AspectWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AspectWidthProperty =
            DependencyProperty.Register("AspectWidth", typeof(double), typeof(AspectRationBox), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double AspectHeight
        {
            get { return (double)GetValue(AspectHeightProperty); }
            set { SetValue(AspectHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AspectHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AspectHeightProperty =
            DependencyProperty.Register("AspectHeight", typeof(double), typeof(AspectRationBox), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double Scale => ActualWidth / AspectWidth;


        protected override Size MeasureOverride(Size constraint)
        {
            var scale = Math.Min(constraint.Width / AspectWidth, constraint.Height / AspectHeight);

            var size = new Size(AspectWidth * scale, AspectHeight * scale);

            Child?.Measure(size);

            return size;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Child is null) return new(0, 0);

            var scale = Math.Min(arrangeSize.Width / AspectWidth, arrangeSize.Height / AspectHeight);
            Rect box = new(0,0, AspectWidth * scale, AspectHeight * scale);

            Child.Arrange(box);

            return box.Size;
        }
    }
}
