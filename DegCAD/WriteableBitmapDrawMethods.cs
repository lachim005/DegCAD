using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;


namespace DegCAD
{
    public static class WriteableBitmapDrawMethods
    {
        #region Circle
        public static void DrawCircle(this WriteableBitmap bitmap, Vector2 center, int radius, Color color)
        {
            // Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            using var context = bitmap.GetBitmapContext();

            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = colorNumber;
            }

            while (x >= y)
            {
                SetPixel((int)center.X + x, (int)center.Y + y);
                SetPixel((int)center.X - x, (int)center.Y + y);
                SetPixel((int)center.X + x, (int)center.Y - y);
                SetPixel((int)center.X - x, (int)center.Y - y);
                SetPixel((int)center.X + y, (int)center.Y + x);
                SetPixel((int)center.X - y, (int)center.Y + x);
                SetPixel((int)center.X + y, (int)center.Y - x);
                SetPixel((int)center.X - y, (int)center.Y - x);

                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
        public static void DrawCircleDashed(this WriteableBitmap bitmap, Vector2 center, int radius, Color color, int dashLength = 10)
        {
            // Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            int dashCounter = 0;
            bool draw = true;
            using var context = bitmap.GetBitmapContext();

            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y, bool draw)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = draw ? colorNumber : 0;
            }

            while (x >= y)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }

                SetPixel((int)center.X + x, (int)center.Y + y, draw);
                SetPixel((int)center.X - x, (int)center.Y + y, draw);
                SetPixel((int)center.X + x, (int)center.Y - y, draw);
                SetPixel((int)center.X - x, (int)center.Y - y, draw);
                SetPixel((int)center.X + y, (int)center.Y + x, draw);
                SetPixel((int)center.X - y, (int)center.Y + x, draw);
                SetPixel((int)center.X + y, (int)center.Y - x, draw);
                SetPixel((int)center.X - y, (int)center.Y - x, draw);

                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
        public static void DrawCircleDotDash(this WriteableBitmap bitmap, Vector2 center, int radius, Color color, int dashLength = 10)
        {
            // Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            int dashCounter = 0;
            bool draw = true;

            using var context = bitmap.GetBitmapContext();

            int ColorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y, bool draw)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = draw ? ColorNumber : 0;
            }

            while (x >= y)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }
                bool shouldDraw = draw || dashCounter == dashLength / 2;
                SetPixel((int)center.X + x, (int)center.Y + y, shouldDraw);
                SetPixel((int)center.X - x, (int)center.Y + y, shouldDraw);
                SetPixel((int)center.X + x, (int)center.Y - y, shouldDraw);
                SetPixel((int)center.X - x, (int)center.Y - y, shouldDraw);
                SetPixel((int)center.X + y, (int)center.Y + x, shouldDraw);
                SetPixel((int)center.X - y, (int)center.Y + x, shouldDraw);
                SetPixel((int)center.X + y, (int)center.Y - x, shouldDraw);
                SetPixel((int)center.X - y, (int)center.Y - x, shouldDraw);

                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
        #endregion

        #region Arc
        public static void DrawArc(this WriteableBitmap bitmap, Vector2 center, int radius, double startAngle, double endAngle, Color color)
        {
            //Uses the same algorithm to draw the circle but draws the pixels
            //only if they are between the two angles

            if (endAngle <= startAngle) endAngle += Math.PI * 2;


            //Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            //Prepares some values for the drawing to be fast
            using var context = bitmap.GetBitmapContext();
            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;


            unsafe void SetPixel(int x, int y)
            {
                //Checks if the pixel is inside of the canvas
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;

                //Calculates the angle of the current pixel
                var angle = Math.Atan((y - center.Y) / (x - center.X));

                //Adjust the angle depending on the quadrant
                if (x - center.X < 0) angle += Math.PI;
                else if (y - center.Y < 0) angle += Math.PI * 2;

                //Adds 2 pi so the angle goes over zero degrees
                if (angle < startAngle) angle += Math.PI * 2;

                //If the angle is not between the the input angles, doesn't draw the pixel
                if (angle > endAngle) return;

                //Draws the pixel
                context.Pixels[x + context.Width * y] = colorNumber;
            }

            //Bresenham's algorithm
            while (x >= y)
            {
                SetPixel((int)center.X + x, (int)center.Y + y);
                SetPixel((int)center.X - x, (int)center.Y + y);
                SetPixel((int)center.X + x, (int)center.Y - y);
                SetPixel((int)center.X - x, (int)center.Y - y);
                SetPixel((int)center.X + y, (int)center.Y + x);
                SetPixel((int)center.X - y, (int)center.Y + x);
                SetPixel((int)center.X + y, (int)center.Y - x);
                SetPixel((int)center.X - y, (int)center.Y - x);

                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
        public static void DrawArcDashed(this WriteableBitmap bitmap, Vector2 center, int radius, double startAngle, double endAngle, Color color, int dashLength = 10)
        {
            //Uses the same algorithm to draw the circle but draws the pixels
            //only if they are between the two angles

            if (endAngle <= startAngle) endAngle += Math.PI * 2;


            //Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            //Prepares some values for the drawing to be fast
            int dashCounter = 0;
            bool draw = true;
            using var context = bitmap.GetBitmapContext();
            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;


            unsafe void SetPixel(int x, int y, bool draw)
            {
                //Checks if the pixel is inside of the canvas
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;

                //Calculates the angle of the current pixel
                var angle = Math.Atan((y - center.Y) / (x - center.X));

                //Adjust the angle depending on the quadrant
                if (x - center.X < 0) angle += Math.PI;
                else if (y - center.Y < 0) angle += Math.PI * 2;

                //Adds 2 pi so the angle goes over zero degrees
                if (angle < startAngle) angle += Math.PI * 2;

                //If the angle is not between the the input angles, doesn't draw the pixel
                if (angle > endAngle) return;

                //Draws the pixel
                context.Pixels[x + context.Width * y] = draw ? colorNumber : 0;
            }

            //Bresenham's algorithm
            while (x >= y)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }

                SetPixel((int)center.X + x, (int)center.Y + y, draw);
                SetPixel((int)center.X - x, (int)center.Y + y, draw);
                SetPixel((int)center.X + x, (int)center.Y - y, draw);
                SetPixel((int)center.X - x, (int)center.Y - y, draw);
                SetPixel((int)center.X + y, (int)center.Y + x, draw);
                SetPixel((int)center.X - y, (int)center.Y + x, draw);
                SetPixel((int)center.X + y, (int)center.Y - x, draw);
                SetPixel((int)center.X - y, (int)center.Y - x, draw);


                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
        public static void DrawArcDotDash(this WriteableBitmap bitmap, Vector2 center, int radius, double startAngle, double endAngle, Color color, int dashLength = 10)
        {
            //Uses the same algorithm to draw the circle but draws the pixels
            //only if they are between the two angles

            if (endAngle <= startAngle) endAngle += Math.PI * 2;


            //Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            //Prepares some values for the drawing to be fast
            int dashCounter = 0;
            bool draw = true;
            using var context = bitmap.GetBitmapContext();
            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;


            unsafe void SetPixel(int x, int y, bool draw)
            {
                //Checks if the pixel is inside of the canvas
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;

                //Calculates the angle of the current pixel
                var angle = Math.Atan((y - center.Y) / (x - center.X));

                //Adjust the angle depending on the quadrant
                if (x - center.X < 0) angle += Math.PI;
                else if (y - center.Y < 0) angle += Math.PI * 2;

                //Adds 2 pi so the angle goes over zero degrees
                if (angle < startAngle) angle += Math.PI * 2;

                //If the angle is not between the the input angles, doesn't draw the pixel
                if (angle > endAngle) return;

                //Draws the pixel
                context.Pixels[x + context.Width * y] = draw ? colorNumber : 0;
            }

            //Bresenham's algorithm
            while (x >= y)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }

                bool shouldDraw = draw || dashCounter == dashLength / 2;
                SetPixel((int)center.X + x, (int)center.Y + y, shouldDraw);
                SetPixel((int)center.X - x, (int)center.Y + y, shouldDraw);
                SetPixel((int)center.X + x, (int)center.Y - y, shouldDraw);
                SetPixel((int)center.X - x, (int)center.Y - y, shouldDraw);
                SetPixel((int)center.X + y, (int)center.Y + x, shouldDraw);
                SetPixel((int)center.X - y, (int)center.Y + x, shouldDraw);
                SetPixel((int)center.X + y, (int)center.Y - x, shouldDraw);
                SetPixel((int)center.X - y, (int)center.Y - x, shouldDraw);


                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }
        #endregion

        #region Line
        public static void DrawLineSolid(this WriteableBitmap bitmap, int x0, int y0, int x1, int y1, Color color)
        {
            using var context = bitmap.GetBitmapContext();

            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = colorNumber;
            }

            //Prepare values for Bresenham's algorithm
            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;


            while (true)
            {
                SetPixel(x0, y0);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * error;
                if (e2 >= dy)
                {
                    if (x0 == x1) break;
                    error += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    if (y0 == y1) break;
                    error += dx;
                    y0 += sy;
                }
            }
        }
        public static void DrawLineDashed(this WriteableBitmap bitmap, int x0, int y0, int x1, int y1, Color color, int dashLength = 10)
        {
            int dashCounter = 0;
            bool draw = true;
            using var context = bitmap.GetBitmapContext();

            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y, bool draw)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = draw ? colorNumber : 0;
            }

            //Prepare values for Bresenham's algorithm
            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;


            while (true)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }

                SetPixel(x0, y0, draw);

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * error;
                if (e2 >= dy)
                {
                    if (x0 == x1) break;
                    error += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    if (y0 == y1) break;
                    error += dx;
                    y0 += sy;
                }
            }
        }
        public static void DrawLineDotDash(this WriteableBitmap bitmap, int x0, int y0, int x1, int y1, Color color, int dashLength = 10)
        {
            int dashCounter = 0;
            bool draw = true;
            using var context = bitmap.GetBitmapContext();

            int colorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y, bool draw)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = draw ? colorNumber : 0;
            }

            //Prepare values for Bresenham's algorithm
            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;


            while (true)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }

                SetPixel(x0, y0, draw || dashCounter == dashLength / 2);

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * error;
                if (e2 >= dy)
                {
                    if (x0 == x1) break;
                    error += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    if (y0 == y1) break;
                    error += dx;
                    y0 += sy;
                }
            }
        }
        #endregion

        #region Thick line
        public static void DrawLineSolidThick(this WriteableBitmap bitmap, int x0, int y0, int x1, int y1, Color color, int thickness = 2)
        {
            bitmap.DrawLineSolid(x0, y0, x1, y1, color);
            int dy = Math.Abs(y1 - y0);
            int dx = Math.Abs(x0 - x1);
            if (dx < dy)
            {
                for (int i = 1; i < thickness; i++)
                {
                    bitmap.DrawLineSolid(x0 - i, y0, x1 - i, y1, color);
                    bitmap.DrawLineSolid(x0 + i, y0, x1 + i, y1, color);
                }
            }
            else
            {
                for (int i = 1; i < thickness; i++)
                {
                    bitmap.DrawLineSolid(x0, y0 - i, x1, y1 - i, color);
                    bitmap.DrawLineSolid(x0, y0 + i, x1, y1 + i, color);
                }
            }
        }
        public static void DrawLineDashedThick(this WriteableBitmap bitmap, int x0, int y0, int x1, int y1, Color color, int thickness = 2, int dashLength = 10)
        {
            bitmap.DrawLineDashed(x0, y0, x1, y1, color, dashLength);
            int dy = Math.Abs(y1 - y0);
            int dx = Math.Abs(x0 - x1);
            if (dx < dy)
            {
                for (int i = 1; i < thickness; i++)
                {
                    bitmap.DrawLineDashed(x0 - i, y0, x1 - i, y1, color, dashLength);
                    bitmap.DrawLineDashed(x0 + i, y0, x1 + i, y1, color, dashLength);
                }
            }
            else
            {
                for (int i = 1; i < thickness; i++)
                {
                    bitmap.DrawLineDashed(x0, y0 - i, x1, y1 - i, color, dashLength);
                    bitmap.DrawLineDashed(x0, y0 + i, x1, y1 + i, color, dashLength);
                }
            }
        }
        public static void DrawLineDotDashThick(this WriteableBitmap bitmap, int x0, int y0, int x1, int y1, Color color, int thickness = 2, int dashLength = 10)
        {
            bitmap.DrawLineDotDash(x0, y0, x1, y1, color, dashLength);
            int dy = Math.Abs(y1 - y0);
            int dx = Math.Abs(x0 - x1);
            if (dx < dy)
            {
                for (int i = 1; i < thickness; i++)
                {
                    bitmap.DrawLineDotDash(x0 - i, y0, x1 - i, y1, color, dashLength);
                    bitmap.DrawLineDotDash(x0 + i, y0, x1 + i, y1, color, dashLength);
                }
            }
            else
            {
                for (int i = 1; i < thickness; i++)
                {
                    bitmap.DrawLineDotDash(x0, y0 - i, x1, y1 - i, color, dashLength);
                    bitmap.DrawLineDotDash(x0, y0 + i, x1, y1 + i, color, dashLength);
                }
            }
        } 
        #endregion
    }
}
