using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace FriteCollection2.Entity;

/// <summary>
/// Object.
/// </summary>
public class Object : Space, IDraw
{
    public Renderer Renderer;

    public static float outlineLayer = 0.55f;
    public float outLayer = Object.outlineLayer;

    public Object() : base()
    {
        Renderer = new Renderer();
    }

    public Object(Texture2D texture) : this()
    {
        Renderer.Texture = texture;
    }

    public void DrawOutline(in SpriteBatch batch)
    {
        if (Renderer.outline)
        {
            Point pos = new Point((int)(float.Round(Position.X) - Camera.X),
                    (int)(float.Round(Position.Y) - Camera.Y));

            Point scale = new Point((int)float.Round(Scale.X),
                    (int)float.Round(Scale.Y));

            foreach (Point r in Renderer.outLinePositions)
            {
                batch.Draw
                (
                    Renderer.Texture,
                    new Rectangle(pos + r, scale),
                    null,
                    Renderer.outlineColor,
                    rotation,
                    centerPoint,
                    Renderer.effect,
                    outLayer
                );
            }
        }
    }

    public void DrawBody(in SpriteBatch batch)
    {
        Point pos = new Point((int)(float.Round(Position.X) - Camera.X),
                    (int)(float.Round(Position.Y) - Camera.Y));

        Point scale = new Point((int)float.Round(Scale.X),
                    (int)float.Round(Scale.Y));

        batch.Draw
        (
            Renderer.Texture,
            new Rectangle(pos, scale),
            null,
            Renderer.Color,
            rotation,
            centerPoint,
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
            return base.Equals((Object)obj)
                && Renderer.Equals(((Object)obj).Renderer);
        }
        return false;
    }

    public override string ToString()
    {
        return "Object (" + base.ToString() + ", " + Renderer.ToString() + ")";
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

    private string txt;

    public string Edit
    {
        get => txt;
        set
        {
            if (txt.Length != value.Length)
                this._scale = EvaluateText(value, 4, 6);
            txt = value;
        }
    }

    public float factor = 1f;
    private Point _scale;
    public Point Scale => _scale;
    public Point Position;

    public float Width => _scale.X * factor;
    public float Height => _scale.Y * factor;

    public Renderer Renderer;
    public Color Background;

    public void SetPosition(Point pos, Bounds b)
    {
        Position = pos + BoundFunc.BoundToPoint(b, Space.width, Space.height);
        Position.X -= (int)float.Round(_scale.X * factor / 2f);
        Position.Y -= (int)float.Round(_scale.Y * factor / 2f);
    }

    public Text(string value)
    {
        this.Renderer = new Renderer();
        this._scale = EvaluateText(value, 4, 6);
        this.txt = value;
        Background = Color.Black;
    }

    public Text(string value, int factor)
    {
        this.factor = factor;
        this.Renderer = new Renderer();
        this._scale = EvaluateText(value, 4, 6);
        this.txt = value;
        Background = Color.Black;
    }

    public void Draw(in SpriteBatch batch)
    {
        if (!Renderer.hide)
        {
            if (Renderer.outline)
            {
                foreach (Point r in Renderer.outLinePositions)
                {
                    batch.DrawString(
                        UI.Text.Font,
                        txt,
                        new Vector2(Position.X + 3 - Space.Camera.X + r.X, Position.Y + 1 - Space.Camera.Y + r.Y),
                        Renderer.outlineColor,
                        0f,
                        Vector2.Zero,
                        factor,
                        SpriteEffects.None,
                        Renderer.GetLayer() + 0.01f);
                }
            }

            batch.DrawString(
                FriteCollection2.UI.Text.Font,
                txt,
                new Vector2(Position.X + 3 - Space.Camera.X, Position.Y + 1 - Space.Camera.Y),
                Renderer.Color,
                0f,
                Vector2.Zero,
                factor,
                SpriteEffects.None,
                Renderer.GetLayer());
        }
    }

    public override string ToString()
    {
        return "Text " + txt + " (" + Renderer.ToString() + ")";
    }
}