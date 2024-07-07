using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile.History
{
    public class PagePaperSizeState : IMFHistoryState
    {
        MFPage page;

        public double width;
        public double height;

        public PagePaperSizeState(MFPage page)
        {
            this.page = page;

            width = page.PaperWidth;
            height = page.PaperHeight;
        }

        public void ApplyState()
        {
            page.PaperWidth = width;
            page.PaperHeight = height;

            page.Editor?.SelectPage(page);
        }

        public void Dispose() { }

        public IMFHistoryState GetOpositeState()
        {
            return new PagePaperSizeState(page);
        }
    }
}
