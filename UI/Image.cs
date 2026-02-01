using FriteCollection2.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FriteCollection2.UI;

public class Image : UI
{
    internal static Texture2D defaultTex;
    public static Texture2D CreateDefaultTexture(GraphicsDevice device)
    {
        defaultTex = Renderer.CreateTexture(device, 2, 2, Color.White);
        return defaultTex;
    }

    protected Texture2D tex;
    private Rectangle imgRect;
    public Color color;

    public Image(UI parent, Texture2D texture) : base(parent, texture.Width, texture.Height)
    {
        this.tex = texture;
        imgRect = base.rect;
        color = Color.White;
    }

    public Image(Texture2D texture) : this(screen, texture) { }
    public Image(UI parent) : this(parent, defaultTex) { }
    public Image() : this(screen, defaultTex) { }

    internal override Rectangle ParentRect => imgRect;

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        imgRect.Width = rect.Width;
        imgRect.Height = rect.Height;
    }

    protected override void OnPositionChanged()
    {
        base.OnPositionChanged();
        imgRect.X = rect.X;
        imgRect.Y = rect.Y;
    }

    public void ChangeTexture(Texture2D tex)
    {
        this.tex = tex;
    }

    public void ApplyOriginalSize()
    {
        int y = tex.Width * rect.Height; // 16
        int x = tex.Height * rect.Width; // 60

        bool fullOnY = x > y;

        if (fullOnY)
        {
            imgRect.Y = rect.Y;
            imgRect.Height = rect.Height;

            imgRect.Width = y / tex.Height;
            imgRect.X = rect.X + ((rect.Width - imgRect.Width) / 2);
        }
        else
        {
            imgRect.X = rect.X;
            imgRect.Width = rect.Width;

            imgRect.Height = x / tex.Width;
            imgRect.Y = rect.Y + ((rect.Height - imgRect.Height) / 2);
        }
    }

    public override void Draw(in SpriteBatch batch)
    {
        if (active)
        {
            batch.Draw(tex, imgRect, color);
            DrawChilds(in batch);
        }
    }
}
