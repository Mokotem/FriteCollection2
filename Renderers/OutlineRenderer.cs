using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2;

public class OutlineRenderer : TextureRenderer
{
    private static Color _default;

    public static void SetDefault(Color value)
    {
        OutlineRenderer._default = value;
    }

    public OutlineRenderer() : base() { }
    public OutlineRenderer(Texture2D texture, Color outline) : base()
    {
        this.Outline = outline;
    }
    public OutlineRenderer(Color color) : base(color) { }

    public Color Outline;

    public override void Draw(in SpriteBatch batch, Rectangle rectangle, Vector2 centerPoint, float rotation)
    {
        batch.Draw(_defaultTexture,
            new Rectangle(rectangle.X - 1, rectangle.Y - 1, rectangle.Width + 2, rectangle.Height + 2), null,
            Outline, rotation, centerPoint, SpriteEffects.None, _layer + 0.0001f);
        base.Draw(batch, rectangle, centerPoint, rotation);
    }
}