﻿using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Hide : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Skrýt";
            esb.CommandHelp = "Vyberte prvek, který chcete skrýt";

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

            return new(new[] { new HideModification(item.Item1, item.Item2) });
        }
    }
}
