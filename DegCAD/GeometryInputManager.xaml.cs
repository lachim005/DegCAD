using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DegCAD
{
    /// <summary>
    /// Interaction logic for GeometryInputManager.xaml
    /// </summary>
    public class GeometryInputManager
    {
        public ViewPort ViewPort { get; set; }
        public Snapper Snapper { get; set; }
        public Style PreviewStyle { get; set; } = new Style() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
        public StyleSelector StyleSelector { get; set; }

        private SemaphoreSlim inputSemaphore = new(1, 1);

        public GeometryInputManager(ViewPort viewPort, Snapper snapper, StyleSelector styleSelector)
        {
            ViewPort = viewPort;
            Snapper = snapper;
            StyleSelector = styleSelector;
        }

        /// <summary>
        /// Gets a point from the user
        /// </summary>
        /// <param name="preview">Action that will get executed to redraw the preview of the inputed point</param>
        public async Task<Vector2> GetPoint(Action<Vector2> preview, Vector2[]? points = null, ParametricLine2[]? lines = null, Circle2[]? circles = null, Predicate<Vector2>? predicate = null)
        {
            await inputSemaphore.WaitAsync();

            //Redraws the preview of the point
            void DrawPreview(Vector2 canvasPos)
            {
                Vector2 snapPos = Snapper.Snap(canvasPos, points, lines, circles);
                if (predicate is not null && !predicate(snapPos)) return;
                preview(snapPos);
            }

            //Saves the previewPoint handler so it can be unasigned later
            EventHandler<VPMouseEventArgs> previewPoint = (s, e) =>
            {
                DrawPreview(e.CanvasPos);
            };

            //Redraws the preview when the user moves the mouse or zooms/pans the viewport
            ViewPort.VPMouseMoved += previewPoint;
            ViewPort.ViewportChanged += previewPoint;

            //Used to await the click
            TaskCompletionSource<Vector2?> result = new();

            //Saves the handler so it can be unasigned
            MouseButtonEventHandler viewPortClick = (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Vector2 res = Mouse.GetPosition(ViewPort);
                    Vector2 snapCanvasPos = Snapper.Snap(ViewPort.ScreenToCanvas(res), points, lines, circles);

                    if (predicate is not null && !predicate(snapCanvasPos)) return;

                    result.SetResult(snapCanvasPos);
                }
            };

            ViewPort.PreviewMouseDown += viewPortClick;

            //Draws the preview so it doesn't appear after the user moves their mouse
            DrawPreview(ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort)));

            //Saves the previewPoint handler so it can be unasigned later
            KeyEventHandler cancelCommand = (s, e) =>
            {
                if (e.Key != Key.Escape) return;
                result.SetResult(null);
            };
            var win = Window.GetWindow(ViewPort);
            win.PreviewKeyDown += cancelCommand;

            //Awaits the user click
            Vector2? mposClick = await result.Task;

            //Unasignes all events and clears the preview
            ViewPort.VPMouseMoved -= previewPoint;
            ViewPort.PreviewMouseDown -= viewPortClick;
            ViewPort.ViewportChanged -= previewPoint;
            win.PreviewKeyDown -= cancelCommand;

            inputSemaphore.Release();

            if (mposClick is null) throw new CommandCanceledException();

            return mposClick.Value;
        }

        public async Task<(Vector2, bool)> GetPointWithPlane(Action<Vector2, bool> preview, bool defaultPlane = false, Vector2[]? points = null, ParametricLine2[]? lines = null, Circle2[]? circles = null, Predicate<Vector2>? predicate = null)
        {
            await inputSemaphore.WaitAsync();

            bool plane = defaultPlane;

            //Redraws the preview of the point
            void DrawPreview(Vector2 canvasPos)
            {
                Vector2 snapPos = Snapper.Snap(canvasPos, points, lines, circles);
                if (predicate is not null && !predicate(snapPos)) return;
                preview(snapPos, plane);
            }

            //Saves the previewPoint handler so it can be unasigned later
            EventHandler<VPMouseEventArgs> previewPoint = (s, e) =>
            {
                DrawPreview(e.CanvasPos);
            };

            //Redraws the preview when the user moves the mouse or zooms/pans the viewport
            ViewPort.VPMouseMoved += previewPoint;
            ViewPort.ViewportChanged += previewPoint;

            //Used to await the click
            TaskCompletionSource<Vector2?> result = new();

            //Saves the handler so it can be unasigned
            MouseButtonEventHandler viewPortClick = (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    //Accepts the point
                    Vector2 res = Mouse.GetPosition(ViewPort);
                    Vector2 snapCanvasPos = Snapper.Snap(ViewPort.ScreenToCanvas(res), points, lines, circles);

                    if (predicate is not null && !predicate(snapCanvasPos)) return;

                    result.SetResult(snapCanvasPos);
                } else if (e.ChangedButton == MouseButton.Right)
                {
                    //Switches the plane
                    plane = !plane;
                    DrawPreview(ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort)));
                }
            };

            ViewPort.PreviewMouseDown += viewPortClick;

            //Draws the preview so it doesn't appear after the user moves their mouse
            DrawPreview(ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort)));

            //Saves the previewPoint handler so it can be unasigned later
            KeyEventHandler cancelCommand = (s, e) =>
            {
                if (e.Key != Key.Escape) return;
                result.SetResult(null);
            };
            var win = Window.GetWindow(ViewPort);
            win.PreviewKeyDown += cancelCommand;

            //Awaits the user click
            Vector2? mposClick = await result.Task;

            //Unasignes all events and clears the preview
            ViewPort.VPMouseMoved -= previewPoint;
            ViewPort.PreviewMouseDown -= viewPortClick;
            ViewPort.ViewportChanged -= previewPoint;
            win.PreviewKeyDown -= cancelCommand;

            inputSemaphore.Release();

            if (mposClick is null) throw new CommandCanceledException();

            return (mposClick.Value, plane);
        }

        /// <summary>
        /// Gets a line from the user
        /// </summary>
        public async Task<ParametricLine2> GetLine(Action<Vector2, ParametricLine2?> preview)
        {
            await inputSemaphore.WaitAsync();

            //Saves the previewPoint handler so it can be unasigned later
            EventHandler<VPMouseEventArgs> previewPoint = (s, e) =>
            {
                Vector2 snapPos = Snapper.Snap(e.CanvasPos);
                ParametricLine2? selectedLine = Snapper.SelectLine(e.CanvasPos);
                preview(snapPos, selectedLine);
            };

            //Redraws the preview when the user moves the mouse or zooms/pans the viewport
            ViewPort.VPMouseMoved += previewPoint;
            ViewPort.ViewportChanged += previewPoint;

            //Used to await the click
            TaskCompletionSource<ParametricLine2?> result = new();

            //Saves the handler so it can be unasigned
            MouseButtonEventHandler viewPortClick = (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Vector2 res = ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort));
                    ParametricLine2? selectedLine = Snapper.SelectLine(res);
                    //If the user didn't click on a line, ignores the click
                    if (selectedLine is null)
                        return;
                    result.SetResult((ParametricLine2)selectedLine);
                }
            };

            ViewPort.PreviewMouseDown += viewPortClick;

            //Draws the preview so it doesn't appear after the user moves their mouse
            Vector2 mousePos = ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort));
            preview(mousePos, Snapper.SelectLine(mousePos));

            //Saves the previewPoint handler so it can be unasigned later
            KeyEventHandler cancelCommand = (s, e) =>
            {
                if (e.Key != Key.Escape) return;
                result.SetResult(null);
            };
            var win = Window.GetWindow(ViewPort);
            win.PreviewKeyDown += cancelCommand;

            //Awaits the user click
            var mposClick = await result.Task;

            //Unasignes all events and clears the preview
            ViewPort.VPMouseMoved -= previewPoint;
            ViewPort.PreviewMouseDown -= viewPortClick;
            ViewPort.ViewportChanged -= previewPoint;
            win.PreviewKeyDown -= cancelCommand;

            inputSemaphore.Release();

            if (mposClick is null) throw new CommandCanceledException();

            return mposClick.Value;
        }

        /// <summary>
        /// Gets an index of a mongeitem from the user
        /// </summary>
        public async Task<(int, int)> GetItem(Action<Vector2, IMongeItem?> preview)
        {
            await inputSemaphore.WaitAsync();

            //Saves the previewPoint handler so it can be unasigned later
            EventHandler<VPMouseEventArgs> previewPoint = (s, e) =>
            {
                Vector2 snapPos = Snapper.Snap(e.CanvasPos);
                var selectedItem = Snapper.SelectItem(e.CanvasPos);
                if (selectedItem is null)
                {
                    preview(snapPos, null);
                    return;
                }
                preview(snapPos, Snapper.Timeline.CommandHistory[selectedItem.Value.Item1].Items[selectedItem.Value.Item2]);
            };

            //Redraws the preview when the user moves the mouse or zooms/pans the viewport
            ViewPort.VPMouseMoved += previewPoint;
            ViewPort.ViewportChanged += previewPoint;

            //Used to await the click
            TaskCompletionSource<(int, int)?> result = new();

            //Saves the handler so it can be unasigned
            MouseButtonEventHandler viewPortClick = (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Vector2 res = ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort));
                    var selectedItem = Snapper.SelectItem(res);
                    //If the user didn't click on a line, ignores the click
                    if (selectedItem is null)
                        return;
                    result.SetResult(selectedItem.Value);
                }
            };

            ViewPort.PreviewMouseDown += viewPortClick;

            //Draws the preview so it doesn't appear after the user moves their mouse
            Vector2 mousePos = ViewPort.ScreenToCanvas(Mouse.GetPosition(ViewPort));
            preview(mousePos, null);

            //Saves the previewPoint handler so it can be unasigned later
            KeyEventHandler cancelCommand = (s, e) =>
            {
                if (e.Key != Key.Escape) return;
                result.SetResult(null);
            };
            var win = Window.GetWindow(ViewPort);
            win.PreviewKeyDown += cancelCommand;

            //Awaits the user click
            var mposClick = await result.Task;

            //Unasignes all events and clears the preview
            ViewPort.VPMouseMoved -= previewPoint;
            ViewPort.PreviewMouseDown -= viewPortClick;
            ViewPort.ViewportChanged -= previewPoint;
            win.PreviewKeyDown -= cancelCommand;

            inputSemaphore.Release();

            if (mposClick is null) throw new CommandCanceledException();

            return mposClick.Value;
        }
    }
}
