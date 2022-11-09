namespace DegCAD.DrawableItems
{
    internal class Point : IMongeItem
    {
        public double X { get; set; }
        public double Y { get; set; } = double.NaN;
        public double Z { get; set; } = double.NaN;
        public Style Style { get; set; } = Style.Default;
        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public void Draw(GeometryDrawer gd)
        {
            //Y point cross
            if (!double.IsNaN(Y))
            {
                gd.DrawLine((X - .2, Y), (X + .2, Y), Style);
                gd.DrawLine((X, Y - .2), (X, Y + .2), Style);
            }
            //Z point cross
            if (!double.IsNaN(Z))
            {
                gd.DrawLine((X - .2, -Z), (X + .2, -Z), Style);
                gd.DrawLine((X, -Z - .2), (X, -Z + .2), Style);
            }
        }
    }
}
