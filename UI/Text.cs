using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace FriteCollection2.UI;

public class Text : UI
{
    private Rectangle textRect;
    public StringRenderer Renderer;

    public short Layer
    {
        get => Renderer.Layer;
        set => Renderer.Layer = value;
    }

    private Bounds textAlign;

    public Text(UI parent, string value, Bounds textAlign, byte taille = 8) : base(parent)
    {
        textRect = new Rectangle(0, 0, 0, 0);
        this.textAlign = textAlign;
        this.Renderer = new StringRenderer(value);
        this.Renderer.SetSize(taille);
        this.Renderer._layer = parent.Depth - 0.01f;
        ApplyScale(Extend.Full);
    }

    public Text(string value, Bounds textAlign, byte taille = 8) : this(screen, value, textAlign, taille) { }

    public Text(UI parent, string value, byte taille = 8) : this(parent, value, Bounds.TopLeft, taille) { }

    public Text(string value, byte taille = 8) : this(screen, value, taille) { }

    public override float Depth => Renderer._layer;

    public void ChangeText(string value)
    {
        this.Renderer.Text = value;
        Vector2 taille = StringRenderer.Evaluate(value).ToVector2();
        textRect.Width = (int)float.Round(taille.X * Renderer.ScaleFactor);
        textRect.Height = (int)float.Round(taille.Y * Renderer.ScaleFactor);
        textRect.Location = MakePosition(rect, textRect.Size, textAlign);
        textRect.Y -= 2;
    }

    public string Edit
    {
        get => Renderer.Text;
        set => ChangeText(value);
    }

    public int TextHeight => 0;
    public int TextWidth => 30;

    public override int Left => textRect.Left;
    public override int Right => textRect.Right;
    public override int Top => textRect.Top;
    public override int Bottom => textRect.Bottom;

    protected override void OnSizeChanged()
    {
        ChangeText(Renderer.Text);
        base.OnSizeChanged();
    }

    protected override void OnPositionChanged()
    {
        ChangeText(Renderer.Text);
        base.OnPositionChanged();
    }

    public override void Draw(in SpriteBatch batch)
    {
        if (Active)
        {
            Renderer.Draw(in batch, textRect.Location);
        }
    }
}
