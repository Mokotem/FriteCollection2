using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Entity;

public class ObjectOutline : Object
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

    public static Color outlineColor = Color.Black;
    private static float outlineLayer = 0f;
    public static short OutlineLayer
    {
        get => TextureRenderer.FromLayer(outlineLayer);
        set => outlineLayer = TextureRenderer.ToLayer(value);
    }

    public ObjectOutline() : base() { }

    public ObjectOutline(Texture2D texture) : base(texture) { }

    public ObjectOutline(int width, int height) : base(width, height) { }

    public override void Draw(in SpriteBatch batch)
    {
        Rectangle rect = ToScreen();
        foreach (Point p in outLinePositions)
        {
            batch.Draw(Renderer.Texture, rect, null, outlineColor, 0f, Vector2.Zero, Renderer.effect, outlineLayer);
        }
        Renderer.Draw(in batch, rect);
    }
}
