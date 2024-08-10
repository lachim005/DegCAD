using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    internal class ChangeStyle : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Změnit styl";
            esb.CommandHelp = "Vyberte prvek, kterému chcete změnit styl";

            GeometryElement? highlightedItem = null;
            GeometryElement? highlightedItemClone = null;
            var itemIndex = await inputMgr.GetItem((pt, item) =>
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

            var curStyle = inputMgr.StyleSelector.CurrentStyle;
            var item = inputMgr.Snapper.Timeline.CommandHistory[itemIndex.Item1].Items[itemIndex.Item2];

            if (item is not GeometryElement ge) return null;
            ge.Style = curStyle;

            return null;
        }
    }
}
