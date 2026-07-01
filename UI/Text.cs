using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2.UI;

public class Text : UI
{
    private static byte defaultSize = 8;

    public static void SetDefaultSize(byte value)
    {
        Text.defaultSize = value;
    }

    private Rectangle textRect;
    public StringRenderer Renderer;

    public short Layer
    {
        get => Renderer.Layer;
        set => Renderer.Layer = value;
    }

    private Bounds textAlign;

    public Text(UI parent, string value, Bounds textAlign, byte taille = byte.MaxValue) : base(parent, 0, 0)
    {
        textRect = new Rectangle(0, 0, 0, 0);
        this.textAlign = textAlign;
        this.Renderer = new StringRenderer(parent, value);

        if (taille > 254)
        {
            this.Renderer.SetSize(defaultSize);
        }
        else
            this.Renderer.SetSize(taille);

        this.ChangeText(value);
        this.Renderer._layer = parent.Depth - 0.01f;
        Scale(Extend.Full);
    }

    public Text(string value, Bounds textAlign, byte taille = byte.MaxValue) : this(screen, value, textAlign, taille) { }

    public Text(UI parent, string value, byte taille = byte.MaxValue) : this(parent, value, Bounds.TopLeft, taille) { }

    public Text(string value, byte taille = byte.MaxValue) : this(screen, value, taille) { }

    public override float Depth => Renderer._layer;

    public int TextWidth => textRect.Width;
    public int TextHeight => textRect.Height;

    public void ChangeText(string value)
    {
        this.Renderer.Text = value;
        textRect.Size = StringRenderer.Evaluate(value, Renderer.ScaleFactor);
        this.OnMyScaleChange();
    }

    public string Edit
    {
        get => Renderer.Text;
        set => ChangeText(value);
    }

    public void Format()
    {
        if (textRect.Width > rect.Width)
        {
            ChangeText(StringRenderer.Format(Renderer.Text, rect.Size));
        }
    }

    public override int Left => textRect.Left;
    public override int Right => textRect.Right;
    public override int Top => textRect.Top;
    public override int Bottom => textRect.Bottom;


    protected override void OnIShouldUpdatePositionsOfMyChilds()
    {
        base.OnIShouldUpdatePositionsOfMyChilds();
        this.OnMyScaleChange();
    }

    protected override void OnParentPositionChanged()
    {
        base.OnParentPositionChanged();
        this.OnMyScaleChange();
    }

    protected sealed override void OnMyScaleChange()
    {
        textRect.Location = MakePosition(rect, textRect.Size, textAlign);
        textRect.Y -= 2;
    }

    public override void Draw(SpriteBatch batch)
    {
        if (Active)
        {
            Renderer.Draw(batch, textRect.Location);
        }
    }
}
