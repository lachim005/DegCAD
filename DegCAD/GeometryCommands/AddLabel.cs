using DegCAD.Dialogs;
using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.GeometryCommands
{
    public class AddLabel : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Pojmenovat";
            esb.CommandHelp = "Vyberte prvek, který chcete pojmenovat";

            Vector2 lastPt = (0,0);

            GeometryElement? highlightedItem = null;
            GeometryElement? highlightedItemClone = null;
            var item = await inputMgr.GetItem((pt, item) =>
            {
                lastPt = pt;
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

            if (highlightedItem is Label)
            {
                MessageBox.Show(Window.GetWindow(vpl.Canvas), "Štítky nemůžete pojmenovat", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            esb.CommandHelp = "Zadejte název";
            var lid = new LabelInput();
            lid.ShowDialog();

            if (highlightedItem is null) return null;
            if (!lid.Canceled)
            {
                return new(new[] { new Label(lid.LabelText, lid.Subscript, lid.Superscript, lastPt, inputMgr.StyleSelector.CurrentStyle, highlightedItem.CloneElement(), vpl, lid.TextSize) });
            }

            return null;
        }
    }
}
