using FriteCollection2.Tools.TileMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FriteCollection2.UI;

public abstract class ButtonCore : Panel
{
    public static void SetDefaultColor(Color value)
    {
        defaultColor = value;
    }

    public Microsoft.Xna.Framework.Rectangle Environment { get; private set; }

    private static Color defaultColor = new Color(0.8f, 0.8f, 0.8f);
    protected Text titleText;
    private bool enabled = true;
    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
        }
    }

    private static List<ButtonCore> _list = new List<ButtonCore>();
    public static ButtonCore[] ToArray() => _list.ToArray();

    private bool IsInRange(Point pos) =>
        pos.X >= this.mRect.X && pos.X < this.mRect.X + this.mRect.Width
     && pos.Y >= this.mRect.Y && pos.Y < this.mRect.Y + this.mRect.Height;

    private protected bool selected = false;
    private bool previousClic = false;

    private protected Procedure _fonction;

    public override void Dispose()
    {
        _list.Remove(this);
        _fonction = null;
        if (titleText is not null)
        {
            titleText.Dispose();
        }
    }

    public Color RestColor;

    private void SetGreyColor()
    {
        this.Color = this.RestColor * 0.5f;
        if (titleText is not null)
            titleText.Color = new Color(0.7f, 0.7f, 0.7f);
    }

    public static Point GetPointPosition(Microsoft.Xna.Framework.Rectangle envi, int factor, Point mouse)
    {
        Point offset = new Point(-envi.X, -envi.Y);
        offset += mouse;
        return new Point(offset.X / factor,
            offset.Y / factor);
    }

    public virtual void Update(Point mousePos, bool mousePressed)
    {
        if (_active)
        {
            if (enabled)
            {
                selected = IsInRange(GetPointPosition(Environment, 1, mousePos));

                if (mousePressed && selected)
                {
                    SetGreyColor();
                }
                else
                {
                    this.Color = RestColor;
                    if (titleText is not null)
                        titleText.Color = Color.White;

                    if (previousClic && _fonction is not null && selected)
                    {
                        _fonction();
                        if (!enabled) SetGreyColor();
                        selected = false;
                    }
                }
            }
            else SetGreyColor();
        }

        previousClic = mousePressed;
    }

    public string EditText
    {
        set
        {
            if (titleText is null)
            {
                this.childs = new List<UI> { new Text(value, new Rectangle(Bounds.Center, Extend.Full), this) };
            }
            else
                titleText.Edit = value;
        }
    }

    private static readonly Point offs = new(14, 2);

    private void Init()
    {
        this.Environment = Default;
        _list.Add(this);
        RestColor = defaultColor;
    }

    private void InitText(string value)
    {
        titleText = new Text(value, new Rectangle(Bounds.TopLeft, Extend.Full, Point.Zero, offs), this);
        titleText.Outline = true;
        this.Add(titleText);
    }

    private void InitEnvironment(Microsoft.Xna.Framework.Rectangle envi)
    {
        this.Environment = envi;
    }


    public ButtonCore(Texture2D image, Rectangle space)
        : base(image, space)
    { Init(); }

    public ButtonCore(TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(tileSet, in batch, device, space)
    { Init(); }
    public ButtonCore(TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(tileSet, in batch, device, space, parent)
    { Init(); }

    public ButtonCore(Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(image, space, parent)
    { Init(); }

    public ButtonCore(string title, Texture2D image, Rectangle space)
        : this(image, space)
    { InitText(title); }

    public ButtonCore(string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : this(tileSet, in batch, device, space, parent)
    { InitText(title); }

    public ButtonCore(string title, Texture2D image, Rectangle space, IHaveRectangle parent)
        : this(image, space, parent)
    { InitText(title); }

    public ButtonCore(string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : this(tileSet, in batch, device, space)
    { InitText(title); }

    ////

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, Texture2D image, Rectangle space)
        : this(image, space)
    { InitEnvironment(envi); }

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : this(tileSet, in batch, device, space)
    { InitEnvironment(envi); }
    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : this(tileSet, in batch, device, space, parent)
    { InitEnvironment(envi); }

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, Texture2D image, Rectangle space, IHaveRectangle parent)
        : this(image, space, parent)
    { InitEnvironment(envi); }

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, string title, Texture2D image, Rectangle space)
        : this(title, image, space)
    { InitEnvironment(envi); }

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : this(title, tileSet, in batch, device, space, parent)
    { InitEnvironment(envi); }

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, string title, Texture2D image, Rectangle space, IHaveRectangle parent)
        : this(title, image, space, parent)
    { InitEnvironment(envi); }

    public ButtonCore(Microsoft.Xna.Framework.Rectangle envi, string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : this(title, tileSet, in batch, device, space)
    { InitEnvironment(envi); }

    public override void Draw(in SpriteBatch batch)
    {
        if (selected && enabled)
        {
            batch.Draw(Entity.Renderer.DefaultTexture,
                new Microsoft.Xna.Framework.Rectangle(
                    this.mRect.X - 1,
                    this.mRect.Y - 1,
                    this.mRect.Width + 2,
                    this.mRect.Height + 2
                    ),
                null,
                Color.Yellow,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                this.depth + 0.001f);
        }
        base.Draw(in batch);
    }
}

public class Toggle : ButtonCore
{
    public Procedure OnActivate;
    public Procedure OnDeactivate;

    protected bool _on = false;

    public void Set(bool value)
    {
        _on = value;
    }

    public bool On => _on;

    public Toggle[] voisins = new Toggle[0];
    public void Deactivate()
    {
        _on = false;
        OnDeactivate();
    }

    public Color OnColor { get; init; }
    public Color OffColor { get; init; }

    public Toggle(TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(tileSet, in batch, device, space, parent) { _fonction = OnClic; }
    public Toggle(Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(image, space, parent) { _fonction = OnClic; }
    public Toggle(string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(title, tileSet, in batch, device, space, parent) { _fonction = OnClic; }
    public Toggle(string title, Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(title, image, space, parent) { _fonction = OnClic; }
    public Toggle(TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(tileSet, in batch, device, space) { _fonction = OnClic; }
    public Toggle(Texture2D image, Rectangle space)
        : base(image, space) { _fonction = OnClic; }
    public Toggle(string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(title, tileSet, in batch, device, space) { _fonction = OnClic; }
    public Toggle(string title, Texture2D image, Rectangle space)
        : base(title, image, space) { _fonction = OnClic; }
    public Toggle(string title, Rectangle space)
        : base(title, Entity.Renderer._defaultTexture, space) { _fonction = OnClic; }


    public Toggle(Microsoft.Xna.Framework.Rectangle envi, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(envi, tileSet, in batch, device, space, parent) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(envi, image, space, parent) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(envi, title, tileSet, in batch, device, space, parent) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, string title, Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(envi, title, image, space, parent) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(envi, tileSet, in batch, device, space) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, Texture2D image, Rectangle space)
        : base(envi, image, space) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(envi, title, tileSet, in batch, device, space) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, string title, Texture2D image, Rectangle space)
        : base(envi, title, image, space) { _fonction = OnClic; }
    public Toggle(Microsoft.Xna.Framework.Rectangle envi, string title, Rectangle space)
        : base(envi, title, Entity.Renderer._defaultTexture, space) { _fonction = OnClic; }


    private void OnClic()
    {
        foreach (Toggle tog in voisins)
        {
            if (tog != this)
                tog.Deactivate();
        }
        _on = !_on;
        if (_on)
            OnActivate();
        else
        {
            OnDeactivate();
            this.Color = OffColor;
        }
    }

    public override void Update(Point mousePos, bool mousepressed)
    {
        base.Update(mousePos, mousepressed);
        if (!mousepressed)
        {
            this.Color = _on ? OnColor : OffColor;
        }
    }
}

public class Button : ButtonCore
{
    public Button(TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(tileSet, in batch, device, space, parent) { }
    public Button(Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(image, space, parent) { }
    public Button(TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(tileSet, in batch, device, space) { }
    public Button(Texture2D image, Rectangle space)
        : base(image, space) { }
    public Button(string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(title, tileSet, in batch, device, space, parent) { }
    public Button(string title, Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(title, image, space, parent) { }
    public Button(string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(title, tileSet, in batch, device, space) { }
    public Button(string title, Texture2D image, Rectangle space)
        : base(title, image, space) { }


    public Button(Microsoft.Xna.Framework.Rectangle envi, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(envi, tileSet, in batch, device, space, parent) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(envi, image, space, parent) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(envi, tileSet, in batch, device, space) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, Texture2D image, Rectangle space)
        : base(envi, image, space) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space, IHaveRectangle parent)
        : base(envi, title, tileSet, in batch, device, space, parent) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, string title, Texture2D image, Rectangle space, IHaveRectangle parent)
        : base(envi, title, image, space, parent) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, string title, TileSet tileSet, in SpriteBatch batch, GraphicsDevice device, Rectangle space)
        : base(envi, title, tileSet, in batch, device, space) { }
    public Button(Microsoft.Xna.Framework.Rectangle envi, string title, Texture2D image, Rectangle space)
        : base(envi, title, image, space) { }


    public Procedure Fonction
    {
        set
        {
            base._fonction = value;
        }
    }
}