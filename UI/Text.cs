using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace FriteCollection2.UI;

public class Text : UI
{
    private static SpriteFont font;
    public static SpriteFont Font => font;

    private static byte baseScale;
    public static void SetFont(SpriteFont font, byte baseScale)
    {
        Text.font = font;
        Text.baseScale = baseScale;
    }

    private Rectangle textRect;
    private string value;
    private Bounds textAlign;
    public Color color;
    private readonly float scale;

    public Text(UI parent, string value, Bounds textAlign, byte taille = 16) : base(parent)
    {
        textRect = new Rectangle(0, 0, 0, 0);
        this.textAlign = textAlign;
        color = Color.Black;
        this.value = value;
        this.scale = taille / (float)baseScale;
        ApplyScale(Extend.Horizontal);
    }

    public Text(string value, Bounds textAlign, byte taille = 16) : this(screen, value, textAlign, taille) { }

    public Text(UI parent, string value, byte taille = 16) : this(parent, value, Bounds.Top, taille) { }

    public Text(string value, byte taille = 16) : this(screen, value, taille) { }

    public void ChangeText(string value)
    {
        this.value = value;
        Vector2 taille = font.MeasureString(value);
        textRect.Width = (int)float.Round(taille.X * scale);
        textRect.Height = (int)float.Round(taille.Y * scale);
        textRect.Location = MakePosition(rect, textRect.Size, textAlign);
        textRect.Y -= 2;
    }

    public override int Bottom => textRect.Bottom;

    protected override void OnSizeChanged()
    {
        ChangeText(value);
        base.OnSizeChanged();
    }

    protected override void OnPositionChanged()
    {
        ChangeText(value);
        base.OnPositionChanged();
    }

    public override void Draw(in SpriteBatch batch)
    {
        if (active)
        {
            batch.DrawString(font, value, textRect.Location.ToVector2(), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layer);
        }
    }
}
