using Microsoft.Xna.Framework;
using System.Collections;

namespace FriteCollection2;

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

    public static int AlignToInt(Align a, int width)
    {
        return a switch
        {
            Align.Left => 0,
            Align.Center => rounds2(width / 2f),
            Align.Right => width,
            _ => 0
        };
    }

    public static Vector2 BoundToVector(Bounds b, float width, float height)
    {
        return b switch
        {
            Bounds.TopLeft => new Vector2(0, 0),
            Bounds.Top => new Vector2(width / 2f, 0),
            Bounds.TopRight => new Vector2(width, 0),

            Bounds.Left => new Vector2(0, height / 2f),
            Bounds.Center => new Vector2(width / 2f, height / 2f),
            Bounds.Right => new Vector2(width, height / 2f),

            Bounds.BottomLeft => new Vector2(0, height),
            Bounds.Bottom => new Vector2(width / 2f, height),
            Bounds.BottomRight => new Vector2(width, height),

            _ => throw new System.Exception("aaaaaa")
        };
    }

    public static Point BoundToPoint(Bounds b, int width, int height)
    {
        return b switch
        {
            Bounds.TopLeft => new Point(0, 0),
            Bounds.Top => new Point(rounds2(width), 0),
            Bounds.TopRight => new Point(width, 0),

            Bounds.Left => new Point(0, rounds2(height)),
            Bounds.Center => new Point(rounds2(width), rounds2(height)),
            Bounds.Right => new Point(width, rounds2(height / 2f)),

            Bounds.BottomLeft => new Point(0, height),
            Bounds.Bottom => new Point(rounds2(width), height),
            Bounds.BottomRight => new Point(width, height),

            _ => throw new System.Exception("aaaaaa")
        };
    }

    public static Bounds Mirror(Bounds b)
    {
        return (b) switch
        {
            Bounds.TopLeft => Bounds.BottomLeft,
            Bounds.Top => Bounds.Bottom,
            Bounds.TopRight => Bounds.BottomRight,

            Bounds.BottomLeft => Bounds.TopLeft,
            Bounds.Bottom => Bounds.Top,
            Bounds.BottomRight => Bounds.TopRight,

            _ => b
        };
    }

    public static Align BoundsToAlignX(Bounds bound)
    {
        return bound switch
        {
            Bounds.TopLeft or Bounds.Left or Bounds.BottomLeft => Align.Left,
            Bounds.Top or Bounds.Center or Bounds.Bottom => Align.Center,
            _ => Align.Right,
        };
    }

    public static Align BoundsToAlignY(Bounds bound)
    {
        return bound switch
        {
            Bounds.Top or Bounds.TopLeft or Bounds.TopRight => Align.Left,
            Bounds.Left or Bounds.Center or Bounds.Right => Align.Center,
            _ => Align.Right,
        };
    }

    private static int rounds2(float value) => (int)float.Round(value / 2f);
}
