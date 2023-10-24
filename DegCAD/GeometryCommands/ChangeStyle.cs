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

            IMongeItem? highlightedItem = null;
            IMongeItem? highlightedItemClone = null;
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
                    highlightedItemClone = item.Clone();
                    highlightedItemClone.Style = Style.HighlightStyle;
                    highlightedItemClone.AddToViewportLayer(previewVpl);
                }

                highlightedItemClone?.Draw();
            });
            highlightedItemClone?.RemoveFromViewportLayer();

            var curStyle = inputMgr.StyleSelector.CurrentStyle;
            inputMgr.Snapper.Timeline.CommandHistory[item.Item1].Items[item.Item2].Style = curStyle;

            return null;
        }
    }
}
