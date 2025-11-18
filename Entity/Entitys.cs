using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace FriteCollection2.Entity;

/// <summary>
/// Object.
/// </summary>
public class Object : ICopy<Object>, IDraw
{
    public Space Space = new Space();
    public Renderer Renderer = new Renderer();

    public Object Copy()
    {
        return new()
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy()
        };
    }

    public void DrawOutline(in SpriteBatch batch)
    {
        if (Renderer.outline)
        {
            Point pos = new Point((int)(float.Round(Space.Position.X) - Space.Camera.X),
                    (int)(float.Round(Space.Position.Y) - Space.Camera.Y));

            Point scale = new Point((int)float.Round(Space.Scale.X),
                    (int)float.Round(Space.Scale.Y));

            foreach (Point r in Renderer.outLinePositions)
            {
                batch.Draw
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

    public void DrawBody(in SpriteBatch batch)
    {
        Point pos = new Point((int)(float.Round(Space.Position.X) - Space.Camera.X),
                    (int)(float.Round(Space.Position.Y) - Space.Camera.Y));

        Point scale = new Point((int)float.Round(Space.Scale.X),
                    (int)float.Round(Space.Scale.Y));

        batch.Draw
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

    public void Draw(in SpriteBatch batch)
    {
        if (!Renderer.hide)
        {
            DrawOutline(in batch);
            DrawBody(in batch);
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Object)
        {
            return Space.Equals(((Object)obj).Space)
                && Renderer.Equals(((Object)obj).Renderer);
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
        while (i < txt.Length)
        {
            if (txt[i].Equals('\n'))
            {
                i += 2;
                count = 1;
                result.Y++;
            }
            else
            {
                count++;
                if (count > result.X)
                    result.X = count;
                i++;
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
        Scale.X++;
        Scale.Y++;
        this.txt = value;
        Background = Color.Black;
    }

    public void Draw(in SpriteBatch batch)
    {
        if (!Renderer.hide)
        {
            batch.Draw
            (
                Renderer.DefaultTexture,
                new Rectangle
                (
                    Position.X - Space.Camera.X,
                    Position.Y - Space.Camera.Y,
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

            batch.DrawString(
                FriteCollection2.UI.Text.Font,
                txt,
                new Vector2(Position.X + 1 - Space.Camera.X, Position.Y + 1 - Space.Camera.Y),
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