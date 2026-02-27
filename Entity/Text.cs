using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace FriteCollection2.Entity;

/// <summary>
/// Object.
/// </summary>
public class Text : Renderer, IDraw
{
    private string text;

    public string Edit
    {
        get => text;
        set
        {
            if (text.Length != value.Length)
                this._scale = StringRenderer.Evaluate(value);
            text = value;
        }
    }

    private Point _scale;
    public Point Scale => _scale;
    public Point Position;

    public float Width => _scale.X;
    public float Height => _scale.Y;

    public bool outline = true;
    public Color OutlineColor;

    public void SetPosition(Point pos, Bounds b)
    {
        Position = pos + BoundFunc.BoundToPoint(b, Space.parent.Width, Space.parent.Height);
        Position.X -= (int)float.Round(_scale.X / 2f);
        Position.Y -= (int)float.Round(_scale.Y / 2f);
    }

    public Text(string value) : base(0)
    {
        this.text = value;
        this._scale = StringRenderer.Evaluate(value);
    }

    public void Draw(SpriteBatch batch)
    {
        if (!hide)
        {
            Point pos = Position - Space.Camera;
            if (outline)
            {
                batch.Draw(TextureRenderer.Default, new Rectangle(pos.X - 1, pos.Y + 1, _scale.X + 1, _scale.Y),
                    null,
                    OutlineColor, 0f, Vector2.Zero, effect, _layer + 0.0001f);
            }
            batch.DrawString(StringRenderer.Font, text, pos.ToVector2(), this.Color, 0f,
                Vector2.Zero, 1f, effect, this._layer);
        }
    }

    public override string ToString()
    {
        return "Text " + text + " ()";
    }
}