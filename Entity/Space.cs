
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace FriteCollection2.Entity;

public class Space
{
    private static readonly Space spacezero = new Space(0, 0);
    public static Space Zero => spacezero;

    public static Point Camera = Point.Zero;

    internal static Rectangle parent;

    public Vector2 Position;
    public Vector2 Scale;


    public Space(int width, int height)
    {
        Position = Vector2.Zero;
        Scale = new Vector2(width, height);
    }

    public Space(Texture2D texture) : this(texture.Width, texture.Height)
    {

    }

    public Space() : this(30, 30)
    {

    }

    public RectangleF ToRectangleF() => new RectangleF(Position, Scale);
    public Rectangle ToRectangle() => new Rectangle(
        ToScreenX(),
        ToScreenY(),
        (int)float.Round(Scale.X),
        (int)float.Round(Scale.Y)
    );

    public Rectangle ToScreen() => new Rectangle(
        ToScreenX(),
        ToScreenY(),
        (int)float.Round(Scale.X),
        (int)float.Round(Scale.Y)
    );

    public static int ToScreenX(float posX) => (int)float.Round(posX) - Camera.X;

    public int ToScreenX() => (int)float.Round(Position.X) - Camera.X;
    public int ToScreenY() => (int)float.Round(Position.Y) - Camera.Y;

    public static Vector2 FromScreen(Point p)
    {
        return new Vector2(p.X + Camera.X, p.Y + Camera.Y);
    }

    public virtual void SetPosition(Vector2 pos, Bounds centerPoint)
    {
        Position = pos - BoundFunc.BoundToVector(centerPoint, Scale.X, Scale.Y);
    }

    public virtual void SetPosition(Vector2 pos, Bounds centerPoint, Bounds origin)
    {
        Position = pos - BoundFunc.BoundToVector(centerPoint, Scale.X, Scale.Y)
                       + BoundFunc.BoundToPoint(origin, parent.Width, parent.Height).ToVector2();
    }

    public float GetPosX(Align origin, Align centerPos)
    {
        return BoundFunc.AlignToFloat(origin, parent.Width) - BoundFunc.AlignToFloat(centerPos, Scale.X);
    }

    public float GetPosY(Align origin, Align centerPos)
    {
        return BoundFunc.AlignToFloat(origin, parent.Height) - BoundFunc.AlignToFloat(centerPos, Scale.Y);
    }

    public Vector2 GetPos(Bounds origin, Bounds centerPos)
    {
        return BoundFunc.BoundToPoint(origin, parent.Width, parent.Height).ToVector2()
             - BoundFunc.BoundToVector(centerPos, Scale.X, Scale.Y);
    }

    public Vector2 GetPos(Bounds centerPos)
    {
        return BoundFunc.BoundToPoint(Bounds.Center, parent.Width, parent.Height).ToVector2()
             - BoundFunc.BoundToVector(centerPos, Scale.X, Scale.Y);
    }

    public Vector2 CenterPoint => new Vector2(CenterPointX, CenterPointY);
    public float CenterPointX => Position.X + (Scale.X / 2f);
    public float CenterPointY => Position.Y + (Scale.Y / 2f);

    public float X
    {
        get => Position.X;
        set => Position.X = value;
    }
    public float Y
    {
        get => Position.Y;
        set => Position.Y = value;
    }

    public float W
    {
        get => Scale.X;
        set => Scale.X = value;
    }
    public float H
    {
        get => Scale.Y;
        set => Scale.Y = value;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode() + (Scale.GetHashCode() * 16_777_216);
    }

    public override bool Equals(object obj)
    {
        if (obj is Space)
        {
            Space sp = (Space)obj;
            return Scale == sp.Scale && Position == sp.Position;
        }
        return false;
    }

    public override string ToString()
    {
        return "Transform (position:" + Position.ToString() + ", scale:" + Scale.ToString() + ")";
    }
}


public interface ILayer
{
    public short Layer
    {
        get;
        set;
    }
}
