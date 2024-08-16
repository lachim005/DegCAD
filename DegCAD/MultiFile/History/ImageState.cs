using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.MultiFile.History
{
    public class ImageState : IMFHistoryState
    {
        MFImage image;
        Stretch stretch;

        public ImageState(MFImage img)
        {
            image = img;
            stretch = img.Stretch;
        }

        public void ApplyState()
        {
            image.Stretch = stretch;

            if (image.Container is not MFContainer cont) return;

            cont.Page.Editor?.SelectPage(cont.Page);
            cont.Select();
        }

        public void Dispose()
        {
            
        }

        public IMFHistoryState GetOpositeState()
        {
            return new ImageState(image);
        }
    }
}
