using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FriteCollection2.UI;

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

    internal static float _defaultolayer;
    public static short DefaultOutlineLayer
    {
        get => FromLayer(_defaultolayer);
        set => _defaultolayer = ToLayer(value);
    }

    public static void SetDefault(Color value)
    {
        OutlineRenderer._default = value;
    }

    private float _oolayer;
    public float OutlineDepth => _oolayer;
    public short OutlineLayer
    {
        get => FromLayer(_oolayer);
        set => _oolayer = ToLayer(value);
    }

    public void StickOutline()
    {
        this._oolayer = _layer + 0.0001f;
    }

    public OutlineRenderer(UI.UI parent) : base(parent)
    {
        _oolayer = _defaultolayer;
        this.OutlineColor = _default;
    }


    public OutlineRenderer(short layer) : base(layer)
    {
        _oolayer = _defaultolayer;
        this.OutlineColor = _default;
    }

    public OutlineRenderer(UI.UI parent, Texture2D texture, Color outline) : base(parent)
    {
        this.OutlineColor = outline;
        _oolayer = _defaultolayer;
    }

    public OutlineRenderer(UI.UI parent, Texture2D texture) : base(parent, texture)
    {
        this.OutlineColor = _default;
        _oolayer = _defaultolayer;
    }

    public OutlineRenderer(UI.UI parent, Color color) : base(parent, color)
    {
        this.OutlineColor = _default;
        _oolayer = _defaultolayer;
    }

    public Color OutlineColor;
    public bool outline = true;

    public void DrawOutline(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c, float layer)
    {
        if (!hide)
        {
            foreach (Point p in outLinePositions)
            {
                batch.Draw(Texture,
                    new Rectangle(rectangle.Location + p, rectangle.Size),
                    null, c, rotation, centerPoint, effect, layer);
            }
        }
    }

    public void DrawOutline(SpriteBatch batch, Rectangle rectangle)
    {
        this.DrawOutline(batch, rectangle, Vector2.Zero, 0f, OutlineColor, _oolayer);
    }

    public void DrawOutline(SpriteBatch batch, Rectangle rectangle, Color c, float layer)
    {
        this.DrawOutline(batch, rectangle, Vector2.Zero, 0f, c, layer);
    }

    public void DrawOutline(SpriteBatch batch, Rectangle rectangle, Color c)
    {
        this.DrawOutline(batch, rectangle, Vector2.Zero, 0f, c, _layer);
    }

    public void DrawBody(SpriteBatch batch, Rectangle rectangle)
    {
        if (!hide)
        batch.Draw(base.Texture, rectangle, null, Color, 0f, Vector2.Zero, effect, _layer);
    }

    public override void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c)
    {
        if (outline)
            DrawOutline(batch, rectangle, centerPoint, rotation, OutlineColor, _oolayer);
        base.Draw(batch, rectangle, centerPoint, rotation, c);
    }
}