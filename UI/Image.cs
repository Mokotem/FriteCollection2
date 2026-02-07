using FriteCollection2.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FriteCollection2.UI;

public class Image : UI
{
    public TextureRenderer Renderer;
    private Rectangle imgRect;

    public Image(UI parent, Texture2D texture) : base(parent, texture.Width, texture.Height)
    {
        this.Renderer = new TextureRenderer(texture, Color.White);
        imgRect = base.rect;
        this.Renderer._layer = parent.Depth + 0.01f;
    }

    public Image(UI parent) : base(parent)
    {
        this.Renderer = new TextureRenderer(Color.White);
        imgRect = base.rect;
        this.Renderer._layer = parent.Depth + 0.01f;
    }

    public Image() : this(screen)
    {

    }

    public Image(Texture2D texture) : this(screen, texture) { }

    public Texture2D Edit
    {
        get => Renderer.Texture;
        set => Renderer.Texture = value;
    }

    protected internal override Rectangle ParentRect => imgRect;

    public override float Depth => Renderer._layer;

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
        this.Renderer.Texture = tex;
    }

    public void ApplyOriginalSize()
    {
        int y = Renderer.Width * rect.Height; // 16
        int x = Renderer.Height * rect.Width; // 60

        bool fullOnY = x > y;

        if (fullOnY)
        {
            imgRect.Y = rect.Y;
            imgRect.Height = rect.Height;

            imgRect.Width = y / Renderer.Height;
            imgRect.X = rect.X + ((rect.Width - imgRect.Width) / 2);
        }
        else
        {
            imgRect.X = rect.X;
            imgRect.Width = rect.Width;

            imgRect.Height = x / Renderer.Width;
            imgRect.Y = rect.Y + ((rect.Height - imgRect.Height) / 2);
        }
    }

    public override void Draw(in SpriteBatch batch)
    {
        if (Active)
        {
            Renderer.Draw(in batch, imgRect);
            DrawChilds(in batch);
        }
    }
}
