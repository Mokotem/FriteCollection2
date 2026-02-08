using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2;

public class OutlineRenderer : TextureRenderer
{
    internal static readonly Point[] outLinePositions = new Point[8]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),

        new(-1, 0),
        new(1, 0),

        new(-1, -1),
        new(0, -1),
        new(1, -1)
    };

    internal static Color _default;

    internal static float _olayer;
    public static short DefaultOutlineLayer
    {
        get => FromLayer(_olayer);
        set => _olayer = ToLayer(value);
    }

    public static void SetDefault(Color value)
    {
        OutlineRenderer._default = value;
    }

    private float _oolayer;
    public short OutlineLayer
    {
        get => FromLayer(_oolayer);
        set => _oolayer = ToLayer(value);
    }

    public OutlineRenderer() : base()
    {
        _oolayer = _olayer;
        this.OutlineColor = _default;
    }

    public OutlineRenderer(Texture2D texture, Color outline) : base()
    {
        this.OutlineColor = outline;
    }

    public OutlineRenderer(Color color) : base(color)
    {
        this.OutlineColor = _default;
    }

    public Color OutlineColor;
    public bool outline = true;

    public void DrawOutline(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        if (outline)
        {
            batch.Draw(_defaultTexture,
                new Rectangle(rectangle.X - 1, rectangle.Y - 1, rectangle.Width + 2, rectangle.Height + 2), null,
                OutlineColor, rotation, centerPoint, SpriteEffects.None, _oolayer);
        }
    }

    public void DrawOutline(in SpriteBatch batch, Rectangle rectangle)
    {
        DrawOutline(in batch, rectangle, Vector2.Zero, 0f);
    }

    public void DrawBody(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        base.Draw(batch, rectangle, centerPoint, rotation);
    }

    public void DrawBody(in SpriteBatch batch, Rectangle rectangle)
    {
        base.Draw(batch, rectangle, Vector2.Zero, 0f);
    }

    public override void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        if (outline)
        {
            batch.Draw(_defaultTexture,
                new Rectangle(rectangle.X - 1, rectangle.Y - 1, rectangle.Width + 2, rectangle.Height + 2), null,
                OutlineColor, rotation, centerPoint, SpriteEffects.None, _oolayer);
        }
        base.Draw(batch, rectangle, centerPoint, rotation);
    }
}