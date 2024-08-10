using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.GeometryCommands
{
    public class StylePicker : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Vybrat styl";
            esb.CommandHelp = "Vyberte prvek, jehož styl chcete vybrat";


            GeometryElement? highlightedItem = null;
            GeometryElement? highlightedItemClone = null;
            var item = await inputMgr.GetItem((pt, item) =>
            {
                if (item is null)
                {
                    highlightedItem = null;
                    highlightedItemClone?.RemoveFromViewportLayer();
                    return;
                }
                if (!ReferenceEquals(highlightedItem, item))
                {
                    highlightedItemClone?.RemoveFromViewportLayer();
                    highlightedItem = item;
                    highlightedItemClone = item.CloneElement();
                    highlightedItemClone.Style = Style.HighlightStyle;
                    highlightedItemClone.AddToViewportLayer(previewVpl);
                }

                highlightedItemClone?.Draw();
            });


            highlightedItemClone?.RemoveFromViewportLayer();

            if (highlightedItem is null) return null;

            var ss = inputMgr.StyleSelector;

            ss.SelectedThickness = highlightedItem.Style.Thickness;
            ss.SelectedLineType = highlightedItem.Style.LineStyle;
            ss.CurrentColor = highlightedItem.Style.Color;

            return null;
        }
    }
}
