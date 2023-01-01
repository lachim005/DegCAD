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
        public static void DrawCircle(this WriteableBitmap bitmap, Vector2 center, Vector2 pointOnCircle, Color color)
        {
            // Calculate the radius of the circle based on the center and the point on the circle
            int radius = (int)Math.Round(Math.Sqrt(Math.Pow(pointOnCircle.X - center.X, 2) + Math.Pow(pointOnCircle.Y - center.Y, 2)));

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
        public static void DrawCircleDashed(this WriteableBitmap bitmap, Vector2 center, Vector2 pointOnCircle, Color color, int dashLength = 8)
        {
            // Calculate the radius of the circle based on the center and the point on the circle
            int radius = (int)Math.Round(Math.Sqrt(Math.Pow(pointOnCircle.X - center.X, 2) + Math.Pow(pointOnCircle.Y - center.Y, 2)));

            // Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            int dashCounter = 0;
            bool draw = true;
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
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }
                if (draw)
                {
                    SetPixel((int)center.X + x, (int)center.Y + y);
                    SetPixel((int)center.X - x, (int)center.Y + y);
                    SetPixel((int)center.X + x, (int)center.Y - y);
                    SetPixel((int)center.X - x, (int)center.Y - y);
                    SetPixel((int)center.X + y, (int)center.Y + x);
                    SetPixel((int)center.X - y, (int)center.Y + x);
                    SetPixel((int)center.X + y, (int)center.Y - x);
                    SetPixel((int)center.X - y, (int)center.Y - x);
                }
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
        public static void DrawCircleDotDash(this WriteableBitmap bitmap, Vector2 center, Vector2 pointOnCircle, Color color, int dashLength = 8)
        {
            // Calculate the radius of the circle based on the center and the point on the circle
            int radius = (int)Math.Round(Math.Sqrt(Math.Pow(pointOnCircle.X - center.X, 2) + Math.Pow(pointOnCircle.Y - center.Y, 2)));

            // Initialize the variables for the Bresenham's algorithm
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            int dashCounter = 0;
            bool draw = true;

            using var context = bitmap.GetBitmapContext();

            int ColorNumber = -0x1000000 | (color.R << 16) | (color.G << 8) | color.B;

            unsafe void SetPixel(int x, int y)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
                    return;
                context.Pixels[x + context.Width * y] = ColorNumber;
            }

            while (x >= y)
            {
                dashCounter++;
                if (dashCounter == dashLength)
                {
                    dashCounter = 0;
                    draw = !draw;
                }
                if (draw || dashCounter == dashLength / 2)
                {
                    SetPixel((int)center.X + x, (int)center.Y + y);
                    SetPixel((int)center.X - x, (int)center.Y + y);
                    SetPixel((int)center.X + x, (int)center.Y - y);
                    SetPixel((int)center.X - x, (int)center.Y - y);
                    SetPixel((int)center.X + y, (int)center.Y + x);
                    SetPixel((int)center.X - y, (int)center.Y + x);
                    SetPixel((int)center.X + y, (int)center.Y - x);
                    SetPixel((int)center.X - y, (int)center.Y - x);
                }
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

        public static void DrawArc(this WriteableBitmap bitmap, Vector2 center, Vector2 pointOnCircle, double startAngle, double endAngle, Color color)
        {
            //Uses the same algorithm to draw the circle but draws the pixels
            //only if they are between the two angles

            if (endAngle <= startAngle) endAngle += Math.PI * 2;

            //Calculate the radius of the circle
            int radius = (int)(pointOnCircle - center).Length;

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
    }
}
