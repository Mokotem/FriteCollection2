using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Entity;

public abstract class Entity
{
    public Space Space = new Space();

    public Renderer Renderer = new Renderer();

    public virtual void Draw() { }
}

/// <summary>
/// Object.
/// </summary>
public class Object : Entity, ICopy<Object>, IDraw
{
    public Object Copy()
    {
        return new()
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy()
        };
    }

    public void DrawOutline()
    {
        if (!Renderer.hide)
        {
            Point pos = new Point((int)(float.Round(Space.Position.X) - Camera.Position.X),
        (int)(float.Round(Space.Position.Y) - Camera.Position.Y));
            Point scale = new Point((int)float.Round(Space.Scale.X),
                    (int)float.Round(Space.Scale.Y));

            if (Renderer.outline)
            {
                foreach (Point r in new Point[8]
                {
                new(-1, 1),
                new(0, 1),
                new(1, 1),

                new(-1, 0),
                new(1, 0),

                new(-1, -1),
                new(0, -1),
                new(1, -1)
                })
                {
                    GameManager.Draw.Batch.Draw
            (
                Renderer.Texture,
                new Rectangle(pos + r, scale),
                null,
                Renderer.outlineColor,
                0,
                Vector2.Zero,
                Renderer.effect,
                Renderer.outlineLayer
            );
                }
            }
        }
    }

    public void DrawBody()
    {
        if (!Renderer.hide)
        {
            Point pos = new Point((int)(float.Round(Space.Position.X) - Camera.Position.X),
        (int)(float.Round(Space.Position.Y) - Camera.Position.Y));
            Point scale = new Point((int)float.Round(Space.Scale.X),
                    (int)float.Round(Space.Scale.Y));
            GameManager.Draw.Batch.Draw
    (
        Renderer.Texture,
        new Rectangle(pos, scale),
        null,
        Renderer.Color,
        0,
        Vector2.Zero,
        Renderer.effect,
        Renderer.GetLayer()
    );
        }
    }

    public override void Draw()
    {
        if (!Renderer.hide)
        {
            DrawOutline();
            DrawBody();
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Object)
        {
            return Space.Equals((obj as Object).Space)
                && Renderer.Equals((obj as Object).Renderer);
        }
        return false;
    }

    public override string ToString()
    {
        return "Object (" + Space.ToString() + ", " + Renderer.ToString() + ")";
    }
}

public class Text : IDraw
{
    public static Point EvaluateText(string txt, byte fw, byte fh)
    {
        Point result = new Point(1, 1);
        ushort i = 0;
        int count = 1;
        while (i < txt.Length - 1)
        {
            if ((txt[i] + txt[i + 1]).Equals("\n"))
            {
                i += 2;
                count = 1;
                ++result.Y;
            }
            else
            {
                ++count;
                if (count > result.X)
                    result.X = count;
                ++i;
            }
        }
        return new Point(result.X * fw, result.Y * fh);
    }

    public readonly string txt;
    public readonly Point Scale;
    public Point Position;

    public Renderer Renderer;
    public Color Background;

    public Text(string value)
    {
        this.Renderer = new Renderer();
        this.Scale = EvaluateText(value, 4, 6);
        ++Scale.X;
        ++Scale.Y;
        this.txt = value;
        Background = Color.Black;
    }

    public void Draw()
    {
        if (!Renderer.hide)
        {
            GameManager.Draw.Batch.Draw
            (
                Renderer.DefaultTexture,
                new Rectangle
                (
                    Position.X - Camera.Position.X,
                    Position.Y - Camera.Position.Y,
                    Scale.X,
                    Scale.Y
                ),
                null,
                Background,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                Renderer.GetLayer() + 0.0001f
                ); 

            GameManager.Draw.Batch.DrawString(
                GameManager.Font,
                txt,
                new Vector2(Position.X + 1 - Camera.Position.X, Position.Y - Camera.Position.Y),
                Renderer.Color,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Renderer.GetLayer());
        }
    }

    public override string ToString()
    {
        return "Text " + txt + " (" + Renderer.ToString() + ")";
    }
}