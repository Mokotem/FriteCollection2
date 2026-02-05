using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace FriteCollection2.Entity;

/// <summary>
/// Object.
/// </summary>
public class Text : IDraw
{
    public StringRenderer Renderer;

    public string Edit
    {
        get => Renderer.Text;
        set
        {
            if (Renderer.Text.Length != value.Length)
                this._scale = StringRenderer.Evaluate(value);
            Renderer.Text = value;
        }
    }

    private Point _scale;
    public Point Scale => _scale;
    public Point Position;
    public bool outline = true;

    public float Width => _scale.X;
    public float Height => _scale.Y;

    public Color Background;

    public void SetPosition(Point pos, Bounds b)
    {
        Position = pos + BoundFunc.BoundToPoint(b, Space.parent.Width, Space.parent.Height);
        Position.X -= (int)float.Round(_scale.X / 2f);
        Position.Y -= (int)float.Round(_scale.Y / 2f);
    }

    public Text(string value)
    {
        this.Renderer = new StringRenderer(value);
        this._scale = StringRenderer.Evaluate(value);
        Background = Color.Black;
    }

    public void Draw(in SpriteBatch batch)
    {
        if (!Renderer.hide)
        {

            Point pos = Position - Space.Camera;

            if (outline)
            {
                foreach (Point r in ObjectOutline.outLinePositions)
                {
                    Renderer.Draw(in batch, pos + r);
                }
            }

            Renderer.Draw(in batch, pos);
        }
    }

    public override string ToString()
    {
        return "Text " + Renderer.Text + " (" + Renderer.ToString() + ")";
    }
}