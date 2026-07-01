
/*
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace FriteCollection2.UI;

public class Panel : UI
{
    private const int padding = 2, padding2 = padding * 2;
    protected readonly RenderTarget2D target, scroolTarget;

    public static Color ClearColor = Color.Transparent;
    public static Color Color = Color.White;

    public RenderTarget2D Target => scroolTarget;

    public int WindowHeight => target.Height;

    protected Rectangle targetRect;
    private readonly Rectangle rectForChilds;
    public readonly OutlineRenderer Renderer;
    protected int scrollValue;

    public SamplerState sampler;

    private int maxScrool;
    public void SetMaxHeight(int value)
    {
        this.maxScrool = value - target.Height;
    }

    public Panel(GraphicsDevice device, UI parent, Bounds pos, Extend ext, int height,
        int addWidth = 0, int addHeight = 0, int addx = 0, int addy = 0, float addLayer = 0)
        : base(parent, 0, 0)
    {
        this.Renderer = new OutlineRenderer(parent);
        this.Renderer.StickOutline();
        base.Scale(ext);
        AddScale(addWidth, addHeight);
        if (height < 2)
        {
            height = rect.Height - padding2;
        }
        base.ApplyPosition(pos, addx, addy);

        target = new RenderTarget2D(device, rect.Width - padding2, rect.Height - padding2);
        scroolTarget = new RenderTarget2D(device, rect.Width - padding2, height);

        targetRect = new Rectangle(rect.X + padding, rect.Y + padding, target.Width, target.Height);
        rectForChilds = new Rectangle(0, 0, target.Width, target.Height);

        int c = 255 - (ParentCount * 24);
        this.Renderer.Color = new Color(c, c, c, 255);
        sampler = SamplerState.LinearWrap;
        this.SetMaxHeight(height);
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


    protected internal override Rectangle ParentRect => rectForChilds;
    public override float Depth => Renderer._layer;

    public override Point GetMousePos()
    {
        Point result = parent.GetMousePos();
        result.X -= targetRect.X;
        result.Y -= targetRect.Y - scrollValue;
        return result;
    }

    public void Update(int value)
    {
        scrollValue -= value;
        if (scrollValue > maxScrool)
        {
            scrollValue = maxScrool;
        }
        if (scrollValue < 0)
        {
            scrollValue = 0;
        }
    }

    public int Scroll
    {
        get => scrollValue;
        set => scrollValue = value;
    }

    protected override void OnIShouldUpdatePositionsOfMyChilds()
    {
        base.OnIShouldUpdatePositionsOfMyChilds();
        targetRect.X = rect.X + padding;
        targetRect.Y = rect.Y + padding;
    }

    protected override void OnParentPositionChanged()
    {
        base.OnParentPositionChanged();
        targetRect.X = rect.X + padding;
        targetRect.Y = rect.Y + padding;
    }

    protected virtual void DrawChildsOnTarget(GraphicsDevice device, SpriteBatch batch)
    {
        device.SetRenderTarget(scroolTarget);
        device.Clear(ClearColor);
        batch.Begin(samplerState: sampler);
        base.Draw(batch);
        batch.End();
    }

    protected virtual void DrawScroll(GraphicsDevice device, SpriteBatch batch)
    {
        device.SetRenderTarget(target);
        device.Clear(ClearColor);
        batch.Begin(samplerState: sampler);
        batch.Draw(scroolTarget, new Rectangle(0, -scrollValue, scroolTarget.Width, scroolTarget.Height), Color);
        batch.End();
    }

    public virtual void DrawTarget(GraphicsDevice device, SpriteBatch batch)
    {
        if (Active)
        {
            DrawChildsOnTarget(device, batch);
            DrawScroll(device, batch);
        }
    }

    public override void Draw(SpriteBatch batch)
    {
        if (Active)
        {
            Renderer.Draw(batch, rect);
            batch.Draw(target, targetRect, null, Color, 0f, Vector2.Zero, SpriteEffects.None, Renderer._layer - 0.001f);
        }
    }
}
*/
