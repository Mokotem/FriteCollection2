using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace FriteCollection2.UI;

public class Panel : UI
{
    private readonly RenderTarget2D target, scroolTarget;
    private Rectangle targetRect;
    private readonly Rectangle rectForChilds;
    private readonly Color color;
    private int scrollValue;

    public Panel(GraphicsDevice device, UI parent, Bounds pos, Extend ext, int height,
        int addWidth = 0, int addHeight = 0, int addx = 0, int addy = 0, float addLayer = 0)
        : base(parent)
    {
        this.layer += addLayer;
        base.ApplyScale(ext, addWidth, addHeight);
        if (height < 2)
        {
            height = rect.Height - padding2;
        }
        base.ApplyPosition(pos, addx, addy);

        target = new RenderTarget2D(device, rect.Width - padding2, rect.Height - padding2);
        scroolTarget = new RenderTarget2D(device, rect.Width - padding2, height);

        targetRect = new Rectangle(rect.X + padding, rect.Y + padding, target.Width, target.Height);
        rectForChilds = new Rectangle(0, 0, target.Width, target.Height);

        float c = 1f - (layer * 8);
        color = new Color(c, c, c);
    }

    public Panel(GraphicsDevice device, UI parent, Bounds pos, Extend ext,
    int addWidth = 0, int addHeight = 0, int addx = 0, int addy = 0, float addLayer = 0)
    : this(device, parent, pos, ext, 0, addWidth, addHeight, addx, addy, addLayer) { }

    public Panel(GraphicsDevice device, Bounds pos, Extend ext,
            int addWidth = 0, int addHeight = 0, int addx = 0, int addy = 0, float addLayer = 0)
            : this(device, screen, pos, ext, addWidth, addHeight, addx, addy, addLayer) { }

    public Panel(GraphicsDevice device, Bounds pos, Extend ext, int height,
          int addWidth = 0, int addHeight = 0, int addx = 0, int addy = 0, float addLayer = 0)
          : this(device, screen, pos, ext, height, addWidth, addHeight, addx, addy, addLayer) { }


    internal override Rectangle ParentRect => rectForChilds;

    internal override Point GetMousePos()
    {
        Point result = parent.GetMousePos();
        result.X -= targetRect.X;
        result.Y -= targetRect.Y - scrollValue;
        return result;
    }

    public void Update(int value)
    {
        scrollValue += value;
        if (scrollValue > scroolTarget.Height - target.Height)
        {
            scrollValue = scroolTarget.Height - target.Height;
        }
        if (scrollValue < 0)
        {
            scrollValue = 0;
        }
    }

    protected override void OnPositionChanged()
    {
        base.OnPositionChanged();
        targetRect.X = rect.X + padding;
        targetRect.Y = rect.Y + padding;
    }

    public void DrawTarget(GraphicsDevice device, in SpriteBatch batch)
    {
        if (active)
        {
            device.SetRenderTarget(scroolTarget);
            device.Clear(Color.Transparent);
            batch.Begin();
            base.DrawChilds(in batch);
            batch.End();

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);
            batch.Begin();
            batch.Draw(scroolTarget, new Rectangle(0, -scrollValue, scroolTarget.Width, scroolTarget.Height), Color.White);
            batch.End();
        }
    }

    public override void Draw(in SpriteBatch batch)
    {
        if (active)
        {
            batch.Draw(Image.defaultTex, rect, color);
            batch.Draw(target, targetRect, Color.White);
        }
    }
}
