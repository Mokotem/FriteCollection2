using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace FriteCollection2;

public abstract class Renderer
{
    internal static Color _defaultColor = Color.White;
    public static void SetDefaultColor(Color value)
    {
        _defaultColor = value;
    }

    private const short LayerFloor = -1024, LayerRoof = 1024;
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

    public float Depth => _layer;

    public bool hide = false;

    public short Layer
    {
        get => FromLayer(_layer);
        set => _layer = ToLayer(value);
    }

    public Renderer() : this(0, _defaultColor) { }
    public Renderer(short layer) : this(layer, _defaultColor) { }
    public Renderer(short layer, Color c)
    {
        Color = c;
        this._layer = ToLayer(layer);
    }

    public Renderer(UI.UI parent) : this(parent, _defaultColor) { }

    public Renderer(UI.UI parent, Color color)
    {
        this.Color = color;
        this._layer = parent.Depth - 0.02f;
    }
}

public class TextureRenderer : Renderer, IDisposable
{
    internal static Texture2D _defaultTexture;
    public static Texture2D Default => _defaultTexture;

    public static void CreateDefaultTexture(GraphicsDevice device)
    {
        _defaultTexture = TextureCreator.Create(device, 2, 2);
    }
    public static void CreateDefaultTexture(GraphicsDevice device, Color color)
    {
        _defaultTexture = TextureCreator.Create(device, 2, 2, color);
    }

    public static void Draw(SpriteBatch batch, Texture2D tex, Rectangle rect)
    {
        batch.Draw(tex, rect, _defaultColor);
    }

    public Rectangle offset = Rectangle.Empty;
    private Rectangle sub;
    protected Rectangle SubRect => sub;

    private protected Texture2D _texture;

    public void SetRenderTarget(RenderTarget2D target)
    {
        _texture = target;
        sub = new Rectangle(0, 0, target.Width, target.Height);
    }

    internal Texture2D Texture
    {
        get => _texture;
        set
        {
            _texture = value;
            sub = new Rectangle(0, 0, value.Width, value.Height);
        }
    }

    public int Width => _texture.Width;
    public int Height => _texture.Height;

    public TextureRenderer() : this(0, _defaultTexture, _defaultColor) { }
    public TextureRenderer(short layer) : this(layer, _defaultTexture, _defaultColor) { }
    public TextureRenderer(Texture2D texture) : this(0, texture, _defaultColor) { }
    public TextureRenderer(Color color) : this(0, _defaultTexture, color) { }
    public TextureRenderer(Texture2D texture, Color color) : base(0, color) { }
    public TextureRenderer(UI.UI parent) : this(parent, _defaultTexture, _defaultColor) { }
    public TextureRenderer(UI.UI parent, Texture2D texture) : this(parent, texture, _defaultColor) { }
    public TextureRenderer(UI.UI parent, Color color) : this(parent, _defaultTexture, color) { }

    public TextureRenderer(UI.UI parent, Texture2D texture, Color color) : base(parent, color)
    {
        this._texture = texture;
        sub = new Rectangle(0, 0, texture.Width, texture.Height);
    }
    public TextureRenderer(short layer, Texture2D texture, Color color) : base(layer, color)
    {
        this._texture = texture;
        sub = new Rectangle(0, 0, texture.Width, texture.Height);
    }

    public int SubWidth => sub.Width;
    public int SubHeight => sub.Height;

    public void SetSubSize(int size)
    {
        SetSubSize(size, size);
    }

    public void SetSubSize(int width, int height)
    {
        sub.Width = width;
        sub.Height = height;
    }

    public void SetSprite(int x, int y)
    {
        this.sub.X = x * sub.Width;
        this.sub.Y = y * sub.Height;
    }

    public void SetSprite(Point index)
    {
        this.SetSprite(index.X, index.Y);
    }

    public virtual void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c)
    {
        if (!hide)
        {
            batch.Draw(_texture, new Rectangle(
                rectangle.X + offset.X,
                rectangle.Y + offset.Y,
                rectangle.Width + offset.Width,
                rectangle.Height + offset.Height), sub, c, rotation, centerPoint, effect, _layer);
        }
    }

    public void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint)
    {
        Draw(batch, rectangle, centerPoint, 0f, Color);
    }

    public void Draw(SpriteBatch batch, Rectangle rectangle, Color c)
    {
        Draw(batch, rectangle, Vector2.Zero, 0f, c);
    }

    public void Draw(SpriteBatch batch, Rectangle rectangle)
    {
        Draw(batch, rectangle, Color);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}

public class StringRenderer : Renderer
{
    private static new Color _defaultColor = Color.White;
    public new static void SetDefaultColor(Color value)
    {
        _defaultColor = value;
    }

    private static SpriteFont _font;
    public static SpriteFont Font => _font;

    private static bool hasAspect;
    private static byte fw, fh;
    public static Point Aspect => new Point(fw, fh);

    private static float baseScale;

    public Color OutlineColor;
    public bool outline = true;

    public static void SetDefaultFont(SpriteFont value, byte scale)
    {
        _font = value;
        hasAspect = false;
        baseScale = (float)scale;
    }
    public static void SetDefaultFont(SpriteFont value, byte scale, byte fontWidth, byte fontHeight)
    {
        _font = value;
        hasAspect = true;
        fw = fontWidth;
        fh = fontHeight;
        baseScale = scale;
    }

    public static Point Evaluate(string value, char[] echaps, float _scale)
    {
        if (hasAspect)
        {
            if (value.Length < 2)
            {
                return new Point(Aspect.X - 1, Aspect.Y - 4);
            }

            Point result = new Point(1, 1);
            ushort i = 0;
            int count = 0;
            while (i < value.Length)
            {
                if (value[i].Equals('\n'))
                {
                    count = 1;
                    result.Y++;
                }
                else
                {
                    if (!Contains(echaps, value[i]))
                    {
                        count++;
                        if (count > result.X)
                            result.X = count;
                    }
                }
                i++;
            }
            return new Point(result.X * fw - 1, (result.Y * fh) - 4);
        }
        else
        {
            Vector2 scale = _font.MeasureString(value);
            return new Point((int)float.Round(scale.X * _scale), (int)float.Round(scale.Y * _scale));
        }
    }
    public static Point Evaluate(string value, float scale = 1f)
    {
        return Evaluate(value, Array.Empty<char>(), scale);
    }

    public static string Format(string value, bool sl, char[] echapements,
        Point box,
        ushort maxLine,
        out byte lineNumber,
        out ushort exedent,
        out int textWidth)
    {
        string result = "";
        string[] words = value.Split(' ');

        int line = 0;
        lineNumber = 1;
        textWidth = 0;

        if (hasAspect)
        {
            for (int i = 0; i < words.Length; i++)
            {
                int taille = LetterCount(words[i], echapements);
                line += taille * fw;
                if (line == box.X)
                {
                    result += words[i] + "\n";
                    line = 0;
                    lineNumber++;
                }
                else if (line > box.X)
                {
                    result += "\n" + words[i] + " ";
                    line = (taille + 1) * fw;
                    lineNumber++;
                }
                else
                {
                    result += words[i] + " ";
                    line += fw;
                }

                if (line > textWidth)
                {
                    textWidth = line;
                }

                if (lineNumber > maxLine)
                {
                    exedent = (ushort)taille;
                    return result;
                }
            }
        }
        else
        {
            for (int i = 0; i < words.Length; i++)
            {
                Vector2 asp = _font.MeasureString(words[i]);
                line += (int)float.Ceiling(asp.X);
                if (line == box.X)
                {
                    result += words[i] + "\n";
                    line = 0;
                    lineNumber++;
                }
                else if (line > box.X)
                {
                    result += "\n" + words[i];
                    line = (int)float.Ceiling(asp.X);
                    lineNumber++;
                }
                else
                {
                    result += words[i] + " ";
                }

                if (line > textWidth)
                {
                    textWidth = line;
                }

                if (lineNumber > maxLine)
                {
                    exedent = (ushort)float.Ceiling(asp.X);
                    return result;
                }
            }
        }

        exedent = 0;
        return result;
    }

    public static string Format(string value, Point box)
    {
        return Format(value, false, System.Array.Empty<char>(), box, ushort.MaxValue, out _, out _, out _);
    }

    private static int LetterCount(string word, char[] echaps)
    {
        int result = 0;
        foreach(char l in word)
        {
            if (!Contains(echaps, l))
                result++;
        }
        return result;
    }

    private static bool Contains(char[] tab, char c)
    {
        foreach(char e in tab)
        {
            if (e.Equals(c))
                return true;
        }
        return false;
    }

    public string Text;
    private SpriteFont font;
    private float _scalefactor = 1f;
    public float ScaleFactor => _scalefactor;

    public void SetSize(byte value)
    {
        _scalefactor = value / baseScale;
    }

    public StringRenderer(UI.UI parent) : base(parent, StringRenderer._defaultColor)
    {
        Text = string.Empty;
        font = _font;
        OutlineColor = OutlineRenderer._default;
    }
    public StringRenderer(string text) : base(0)
    {
        Text = text;
        font = _font;
        OutlineColor = OutlineRenderer._default;
        Color = _defaultColor;
    }

    public StringRenderer(UI.UI parent, string text) : this(parent)
    {
        Text = text;
    }

    public StringRenderer(UI.UI parent, Color color) : base(parent, color)
    {
        Text = string.Empty;
        font = _font;
        OutlineColor = OutlineRenderer._default;
    }

    public StringRenderer(UI.UI parent, string text, Color color) : this(parent, color)
    {
        Text = text;

    }

    public StringRenderer(UI.UI parent, Color color, string text) : this(parent, text, color) { }

    public StringRenderer(UI.UI parent, SpriteFont font) : base(parent)
    {
        Text = string.Empty;
        this.font = font;
    }

    public StringRenderer(UI.UI parent, SpriteFont font, string text) : base(parent)
    {
        Text = text;
        this.font = font;
        OutlineColor = OutlineRenderer._default;
    }

    public StringRenderer(UI.UI parent, SpriteFont font, Color color) : base(parent, color)
    {
        Text = string.Empty;
        this.font = font;
        OutlineColor = OutlineRenderer._default;
    }

    public StringRenderer(UI.UI parent, SpriteFont font, string text, Color color) : base(parent, color)
    {
        Text = text;
        this.font = font;
        OutlineColor = OutlineRenderer._default;
    }

    public void Draw(SpriteBatch batch, Point pos, Vector2 centerPoint, float rotation, Color c, float layer)
    {
        if (!hide)
        {
            if (outline)
            {
                foreach (Point r in OutlineRenderer.outLinePositions)
                {
                    batch.DrawString(this.font, Text, (pos + r).ToVector2(), OutlineColor,
                        rotation, centerPoint, _scalefactor, effect, layer + 0.001f);
                }
            }
            batch.DrawString(this.font, Text, pos.ToVector2(), c, rotation, centerPoint, _scalefactor, effect, layer);
        }
    }

    public void Draw(SpriteBatch batch, Point pos, Vector2 centerPoint, float rotation, Color c)
    {
        if (!hide)
            batch.DrawString(this.font, Text, pos.ToVector2(), c, rotation, centerPoint, _scalefactor, effect, _layer);
    }

    public void Draw(SpriteBatch batch, Point position, Vector2 centerPoint, float rotation)
    {
        Draw(batch, position, centerPoint, rotation, Color, this._layer);
    }

    public void Draw(SpriteBatch batch, Point position, Vector2 centerPoint)
    {
        Draw(batch, position, centerPoint, 0f);
    }

    public void Draw(SpriteBatch batch, Point position, Color c)
    {
        Draw(batch, position, Vector2.Zero, 0f, c, _layer);
    }

    public void Draw(SpriteBatch batch, Point position, Color c, float layer)
    {
        Draw(batch, position, Vector2.Zero, 0f, c, layer);
    }

    public void Draw(SpriteBatch batch, Point position)
    {
        Draw(batch, position, Color);
    }
}