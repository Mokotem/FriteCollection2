using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity;

public abstract class BaseRenderer
{
    private static Color _defaultColor = Color.White;
    public static void ChangeDefaultColor(Color value)
    {
        _defaultColor = value;
    }

    private const short LayerFloor = -1000, LayerRoof = 1000;
    private const float LayerRange = LayerRoof - LayerFloor;
    public static float ToLayer(short value)
    {
#if DEBUG
        if (value > LayerRoof || value < LayerFloor)
            throw new System.Exception(value + " out of range (" + LayerFloor + ", " + LayerRoof + ")");
#endif
        return (value - LayerFloor) / LayerRange;
    }

    public static short FromLayer(float value)
    {
#if DEBUG
        if (value > 1f || value < 0f)
            throw new System.Exception(value + " out of range (0f, 1f)");
#endif
        return (short)float.Round((value * LayerRange) + LayerFloor);
    }

    public Color Color;
    public SpriteEffects effect = SpriteEffects.None;
    internal float _layer = 0.5f;

    public bool hide = false;

    public short Layer
    {
        get => FromLayer(_layer);
        set => _layer = ToLayer(value);
    }

    public BaseRenderer()
    {
        Color = _defaultColor;
    }

    public BaseRenderer(Color color)
    {
        this.Color = color;
    }
}

public class TextureRenderer : BaseRenderer
{
    internal static Texture2D _defaultTexture;
    public static void CreateDefaultTexture(GraphicsDevice device)
    {
        _defaultTexture = TextureCreator.Create(device, 2, 2);
    }
    public static void CreateDefaultTexture(GraphicsDevice device, Color color)
    {
        _defaultTexture = TextureCreator.Create(device, 2, 2, color);
    }

    public Texture2D Texture { get; set; }

    public TextureRenderer() : base()
    {
        Texture = _defaultTexture;
    }

    public TextureRenderer(Texture2D texture) : base()
    {
        Texture = texture;
    }

    public TextureRenderer(Color color) : base(color)
    {
        Texture = _defaultTexture;
    }

    public TextureRenderer(Texture2D texture, Color color) : base(color)
    {
        Texture = texture;
    }

    public TextureRenderer(Color color, Texture2D texture) : this(texture, color) { }

    public void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c)
    {
        if (!hide)
            batch.Draw(Texture, rectangle, null, c, rotation, centerPoint, effect, _layer);
    }


    public void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        Draw(in batch, rectangle, centerPoint, rotation, Color);
    }

    public void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint)
    {
        Draw(in batch, rectangle, centerPoint, 0f);
    }
    public void Draw(in SpriteBatch batch, Rectangle rectangle, Color c)
    {
        Draw(in batch, rectangle, Vector2.Zero, 0f, c);
    }

    public void Draw(in SpriteBatch batch, Rectangle rectangle)
    {
        Draw(in batch, rectangle, Color);
    }
}

public class TextRenderer : BaseRenderer
{
    private static SpriteFont _font;
    private static bool hasAspect;
    private static byte fw, fh;
    public static void SetDefaultFont(SpriteFont value)
    {
        _font = value;
        hasAspect = false;
    }
    public static void SetDefaultFont(SpriteFont value, byte fontWidth, byte fontHeight)
    {
        _font = value;
        hasAspect = true;
        fw = fontWidth;
        fh = fontHeight;
    }

    public static Point Evaluate(string value)
    {
        if (hasAspect)
        {
            Point result = new Point(1, 1);
            ushort i = 0;
            int count = 1;
            while (i < value.Length)
            {
                if (value[i].Equals('\n'))
                {
                    count = 1;
                    result.Y++;
                }
                else
                {
                    count++;
                    if (count > result.X)
                        result.X = count;
                }
                i++;
            }
            return new Point(result.X * fw, result.Y * fh);
        }
        else
        {
            Vector2 scale = _font.MeasureString(value);
            return new Point((int)float.Round(scale.X), (int)float.Round(scale.Y));
        }
    }

    public string Text;
    private SpriteFont font;

    public TextRenderer() : base()
    {
        Text = string.Empty;
        font = _font;
    }

    public TextRenderer(string text) : this()
    {
        Text = text;
    }

    public TextRenderer(Color color) : base(color)
    {
        Text = string.Empty;
        font = _font;
    }

    public TextRenderer(string text, Color color) : this(color)
    {
        Text = text;
    }

    public TextRenderer(Color color, string text) : this(text, color) { }

    public TextRenderer(SpriteFont font) : base()
    {
        Text = string.Empty;
        this.font = font;
    }

    public TextRenderer(SpriteFont font, string text) : base()
    {
        Text = text;
        this.font = font;
    }

    public TextRenderer(SpriteFont font, Color color) : base(color)
    {
        Text = string.Empty;
        this.font = font;
    }

    public TextRenderer(SpriteFont font, string text, Color color) : base(color)
    {
        Text = text;
        this.font = font;
    }

    public void Draw(in SpriteBatch batch, Point pos, Vector2 centerPoint, float rotation, Color c)
    {
        if (!hide)
            batch.DrawString(this.font, Text, pos.ToVector2(), Color, rotation, centerPoint, 1f, effect, _layer);
    }

    public void Draw(in SpriteBatch batch, Point position, Vector2 centerPoint, float rotation)
    {
        Draw(in batch, position, centerPoint, rotation, Color);
    }

    public void Draw(in SpriteBatch batch, Point position, Vector2 centerPoint)
    {
        Draw(in batch, position, centerPoint, 0f);
    }
    public void Draw(in SpriteBatch batch, Point position, Color c)
    {
        Draw(in batch, position, Vector2.Zero, 0f, c);
    }

    public void Draw(in SpriteBatch batch, Point position)
    {
        Draw(in batch, position, Color);
    }
}