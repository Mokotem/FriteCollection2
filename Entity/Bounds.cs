using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity;

public static class BoundFunc
{
    public static float AlignToFloat(Align a, float width)
    {
        return a switch
        {
            Align.Left => 0,
            Align.Center => width / 2f,
            Align.Right => width,
            _ => 0
        };
    }

    public static Vector2 BoundToVector(Bounds b, float width, float height)
    {
        return new Vector2((width * b.x) / 2f, (height * b.y) / 2f);
    }

    public static Point BoundToPoint(Bounds b, int width, int height)
    {
        return new Point((int)float.Round((width * b.x) / 2f), (int)float.Round((height * b.y) / 2f));
    }

    private static int rounds2(float value) => (int)float.Round(value / 2f);
}
