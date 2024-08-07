using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MultiFile.Commands
{
    internal class AddImageCommand : IMFCommand
    {
        public MFItem? Execute()
        {
            OpenFileDialog ofd = new();

            ofd.Filter = "Všechny obrázky|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Obrázky formátu PNG|*.png|Obrázky formátu JPG|*.jpg;*.jpeg|Obrázky typu GIF|*.gif|Obrázky typu BMP|*.bmp|Všechny soubory|*.*";
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() != true) return null;

            try
            {
                MFImage img = new(ofd.FileName);
                return img;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Při otevírání obrázku se vyskytla chyba:\n\n" + ex.Message, "Chyba", img: MessageBoxImage.Error); 
            }
            return null;
        }
    }
}
