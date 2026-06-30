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
    public static float DefaultOutlineDepth => _defaultolayer;

    public static void SetDefault(Color value)
    {
        OutlineRenderer._default = value;
    }

    private float _oolayer;
    public float OutlineDepth => _oolayer;
    private bool hasComonLayer = true;

    public short OutlineLayer
    {
        get => FromLayer(_oolayer);
        set
        {
            _oolayer = ToLayer(value);
            hasComonLayer = false;
        }
    }

    public void StickOutline()
    {
        this._oolayer = _layer + 0.001f;
        hasComonLayer = false;
    }
    public OutlineRenderer() : this(0, _defaultTexture, _defaultColor) { }
    public OutlineRenderer(short layer) : this(layer, _defaultTexture, _defaultColor) { }
    public OutlineRenderer(Texture2D texture) : this(0, texture, _defaultColor) { }
    public OutlineRenderer(Color color) : this(0, _defaultTexture, color) { }
    public OutlineRenderer(UI.UI parent) : this(parent, _defaultTexture, _defaultColor) { }
    public OutlineRenderer(UI.UI parent, Texture2D texture) : this(parent, texture, _defaultColor) { }
    public OutlineRenderer(UI.UI parent, Color color) : this(parent, _defaultTexture, color) { }

    public OutlineRenderer(Texture2D texture, Color color) : base(texture, color)
    { OutlineColor = _default; }
    public OutlineRenderer(UI.UI parent, Texture2D texture, Color color) : base(parent, texture, color)
    { OutlineColor = _default; }
    public OutlineRenderer(short layer, Texture2D texture, Color color) : base(layer, texture, color)
    { OutlineColor = _default; }

    public Color OutlineColor
    {
        get => field;
        set
        {
            field = value;
            outline = true;
        }
    }
    public bool outline = true;

    public void DrawOutline(SpriteBatch batch, Rectangle rectangle, Rectangle? sub, Vector2 centerPoint, float rotation, Color c, float layer)
    {
        if (!hide)
        {
            if (hasComonLayer)
                layer = _defaultolayer;

            foreach (Point p in outLinePositions)
            {
                batch.Draw(_texture,
                    new Rectangle(rectangle.Location + p + offset.Location, rectangle.Size + offset.Size),
                    sub, c, rotation, centerPoint, effect, layer);
            }
        }
    }

    public void DrawOutline(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c, float layer)
    {
        if (!hide)
        {
            if (hasComonLayer)
                layer = _defaultolayer;

            foreach (Point p in outLinePositions)
            {
                batch.Draw(_texture,
                    new Rectangle(rectangle.Location + p + offset.Location, rectangle.Size + offset.Size),
                    SubRect, c, rotation, centerPoint, effect, layer);
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
        this.DrawOutline(batch, rectangle, Vector2.Zero, 0f, c, _oolayer);
    }

    public void DrawBody(SpriteBatch batch, Rectangle rectangle)
    {
        if (!hide)
            batch.Draw(base._texture, rectangle, SubRect, Color, 0f, Vector2.Zero, effect, _layer);
    }

    public void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        this.Draw(batch, rectangle, centerPoint, rotation, Color);
    }

    public override void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c)
    {
        if (outline)
            DrawOutline(batch, rectangle, centerPoint, rotation, OutlineColor, _oolayer);
        base.Draw(batch, rectangle, centerPoint, rotation, c);
    }
}