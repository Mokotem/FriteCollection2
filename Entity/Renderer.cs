using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity;

public class Renderer
{
    private static Texture2D _defaultTexture;
    private static Color _defaultColor = Color.White;
    public static void CreateDefaultTexture(GraphicsDevice device)
    {
        _defaultTexture = TextureCreator.Create(device, 2, 2);
    }
    public static void CreateDefaultTexture(GraphicsDevice device, Color color)
    {
        _defaultTexture = TextureCreator.Create(device, 2, 2, color);
    }
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


    public Texture2D Texture { get; set; }
    public Color color;
    public SpriteEffects effect = SpriteEffects.None;
    internal float _layer = 0.5f;

    public bool hide = false;

    public short Layer
    {
        get => FromLayer(_layer);
        set => _layer = ToLayer(value);
    }

    public Renderer()
    {
        Texture = _defaultTexture;
        color = _defaultColor;
    }

    public Renderer(Texture2D texture)
    {
        Texture = texture;
        color = _defaultColor;
    }

    public Renderer(Color color)
    {
        Texture = _defaultTexture;
        this.color = color;
    }

    public Renderer(Texture2D texture, Color color)
    {
        Texture = texture;
        this.color = color;
    }

    public Renderer(Color color, Texture2D texture) : this(texture, color) { }

    public void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation, Color c)
    {
        if (!hide)
            batch.Draw(Texture, rectangle, null, c, rotation, centerPoint, effect, _layer);
    }


    public void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        Draw(in batch, rectangle, centerPoint, rotation, color);
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
        Draw(in batch, rectangle, color);
    }
}
