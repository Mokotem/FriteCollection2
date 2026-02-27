using FriteCollection2.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FriteCollection2.UI;

public class Image : UI
{
    public OutlineRenderer Renderer;

    public Image(UI parent, Texture2D texture, int width, int height) : base(parent, width, height)
    {
        this.Renderer = new OutlineRenderer(parent, texture);
        this.Renderer._layer = parent.Depth - 0.01f;
        this.Renderer.Color = Color.White;
    }

    public Image(UI parent, Texture2D texture) : this(parent, texture, texture.Width, texture.Height) { }
    public Image(Texture2D texture) : this(screen, texture, texture.Width, texture.Height) { }
    public Image() : this(screen, TextureRenderer.Default, 0, 0) { }
    public Image(UI parent) : this(parent, TextureRenderer.Default, 0, 0) { }
    public Image(Texture2D texture, int width, int height) : this(screen, texture, width, height) { }
    public Image(int width, int height) : this(screen, TextureRenderer.Default, width, height) { }

    public void ApplyOriginalSize(Rectangle parent)
    {
        int y = Renderer.Width * parent.Height; // 16
        int x = Renderer.Height * parent.Width; // 60

        bool fullOnY = x > y;

        if (fullOnY)
        {
            rect.Y = parent.Y;
            rect.Height = parent.Height;

            rect.Width = y / Renderer.Height;
            rect.X = parent.X + ((parent.Width - rect.Width) / 2);
        }
        else
        {
            rect.X = parent.X;
            rect.Width = parent.Width;

            rect.Height = x / Renderer.Width;
            rect.Y = parent.Y + ((parent.Height - rect.Height) / 2);
        }
    }

    public Texture2D Edit
    {
        get => Renderer.Texture;
        set => Renderer.Texture = value;
    }

    public override float Depth => Renderer._layer;

    public void ChangeTexture(Texture2D tex)
    {
        this.Renderer.Texture = tex;
    }

    public override void Draw(SpriteBatch batch)
    {
        if (Active)
        {
            Renderer.Draw(batch, rect);
            base.Draw(batch);
        }
    }
}
