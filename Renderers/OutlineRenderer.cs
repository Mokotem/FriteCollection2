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
        _oolayer = _olayer;
    }

    public OutlineRenderer(Texture2D texture) : base(texture)
    {
        this.OutlineColor = _default;
        _oolayer = _olayer;
    }

    public OutlineRenderer(Color color) : base(color)
    {
        this.OutlineColor = _default;
        _oolayer = _olayer;
    }

    public Color OutlineColor;
    public bool outline = true;

    public void DrawOutline(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        if (outline)
        {
            foreach(Point p in outLinePositions)
            {
                batch.Draw(Texture,
                    new Rectangle(rectangle.Location + p, rectangle.Size),
                    null, OutlineColor, rotation, centerPoint, effect, _oolayer);
            }
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

    public override void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c)
    {
        DrawOutline(in batch, rectangle, centerPoint, rotation);
        base.Draw(batch, rectangle, centerPoint, rotation, c);
    }
}