using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace FriteCollection2.Entity;

/// <summary>
/// Object.
/// </summary>
public class Text : IDraw, ILayer
{
    public string Edit
    {
        get => TextRenderer.Text;
        set
        {
            if (TextRenderer.Text.Length != value.Length)
                this.rect.Size = StringRenderer.Evaluate(value);
            TextRenderer.Text = value;
        }
    }

    private Rectangle rect;
    public Point Position
    {
        get => rect.Location;
        set
        {
            rect.Location = value;
        }
    }
    public int PositionX
    {
        get => rect.X;
        set
        {
            rect.X = value;
        }
    }
    public int PositionY
    {
        get => rect.Y;
        set
        {
            rect.X = value;
        }
    }
    public Point Scale => rect.Size;

    public float Width => rect.Width;
    public float Height => rect.Height;

    public readonly OutlineRenderer Renderer;
    private readonly StringRenderer TextRenderer;

    public Color TextColor
    {
        get => TextRenderer.Color;
        set
        {
            TextRenderer.Color = value;
        }
    }

    public short Layer
    {
        get => Renderer.Layer;
        set
        {
            Renderer.Layer = value;
            TextRenderer.Layer = (short)(value - 1);
        }
    }

    public void SetPosition(Point pos, Bounds center, Bounds origin)
    {
        Position = pos + BoundFunc.BoundToPoint(origin, Space.parent.Width, Space.parent.Height)
            - BoundFunc.BoundToPoint(center, rect.Width, rect.Height);
    }

    public Text(string value)
    {
        this.TextRenderer = new StringRenderer(value);
        this.TextRenderer.outline = false;
        this.Renderer = new OutlineRenderer(0);
        this.Renderer.Color = TextureRenderer._defaultColor;
        TextRenderer.Layer = -10;
        this.rect.Size = StringRenderer.Evaluate(value);
    }

    public void Draw(SpriteBatch batch)
    {
        if (!Renderer.hide)
        {
            Renderer.Draw(batch,
                new Rectangle(new Point(Position.X - 2 - Space.Camera.X, Position.Y - 2 - Space.Camera.Y),
                new Point(StringRenderer.Aspect.X * TextRenderer.Text.Length + 3, 9)));
            TextRenderer.Draw(batch, new Point(rect.X - Space.Camera.X, rect.Y - Space.Camera.Y - 2));
        }
    }

    public override string ToString()
    {
        return "Text '" + TextRenderer.Text + "' ()";
    }
}