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
    }
}
