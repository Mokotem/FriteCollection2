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

    private static FriteModel.MonoGame I;

    public static void SetInstance(in FriteModel.MonoGame instance)
    {
        I = instance;
    }

    private bool IsInRange(Point pos) =>
        pos.X >= this.mRect.X && pos.X < this.mRect.X + this.mRect.Width
     && pos.Y >= this.mRect.Y && pos.Y < this.mRect.Y + this.mRect.Height;

    protected bool b;

    private protected bool selected = false;
    private bool previousClic = false;

    private protected Procedure _fonction;

    public override void Dispose()
    {
        I._buttons.Remove(this);
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

    public static Point GetPointPosition(in Environment envi, Point mouse)
    {
        Point offset = new Point(-envi.Rect.X, -envi.Rect.Y);
        offset += mouse;
        return new Point(offset.X / (envi.Rect.Width / envi.Target.Width),
            offset.Y / (envi.Rect.Height / envi.Target.Height));
    }

    internal virtual void Update(Point mousePos, bool mousePressed)
    {
        if (_active)
        {
            if (enabled)
            {
                selected = IsInRange(GetPointPosition(in Space.environment, mousePos));

                if (b)
                {
                    SetGreyColor();
                }
                else
                {
                    this.Color = RestColor;
                    if (titleText is not null)
                        titleText.Color = Color.White;

                    if (previousClic == true && _fonction is not null && selected)
                    {
                        _fonction();
                        previousClic = false;
                        if (!enabled) SetGreyColor();
                        selected = false;
                    }
                }
            }
            else SetGreyColor();
        }

        previousClic = b;
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

    public ButtonCore(TileSet tileset, Rectangle space, UI parent) : base(tileset, space, parent)
    {
        I._buttons.Add(this);
        RestColor = defaultColor;
    }


    public ButtonCore(TileSet tileset, Rectangle space) : base(tileset, space)
    {
        I._buttons.Add(this);
        RestColor = defaultColor;
    }

    public ButtonCore(Texture2D image, Rectangle space, UI parent) : base(image, space, parent)
    {
        I._buttons.Add(this);
        RestColor = defaultColor;
    }


    public ButtonCore(Texture2D image, Rectangle space) : base(image, space)
    {
        I._buttons.Add(this);
        RestColor = defaultColor;
    }

    private static readonly Point offs = new(16, 3);

    public ButtonCore(string title, TileSet tileset, Rectangle space, UI parent) : base(tileset, space, parent)
    {
        titleText = new Text(title, new Rectangle(in space.environment, Bounds.TopLeft, Extend.Full, Point.Zero, offs), this);
        titleText.Outline = true;
        this.Add(titleText);
        I._buttons.Add(this);
        RestColor = defaultColor;
    }

    public ButtonCore(string title, TileSet tileset, Rectangle space) : base(tileset, space)
    {
        titleText = new Text(title, new Rectangle(in space.environment, Bounds.TopLeft, Extend.Full, Point.Zero, offs), this);
        titleText.Outline = true;
        this.Add(titleText);
        I._buttons.Add(this);
        RestColor = defaultColor;
    }

    public ButtonCore(string title, Texture2D image, Rectangle space, UI parent) : base(image, space, parent)
    {
        titleText = new Text(title, new Rectangle(in space.environment, Bounds.TopLeft, Extend.Full, Point.Zero, offs), this);
        titleText.Outline = true;
        this.Add(titleText);
        I._buttons.Add(this);
        RestColor = defaultColor;
    }

    public ButtonCore(string title, Texture2D image, Rectangle space) : base(image, space)
    {
        titleText = new Text(title, new Rectangle(in space.environment, Bounds.TopLeft, Extend.Full, Point.Zero, offs), this);
        titleText.Outline = true;
        this.Add(titleText);
        I._buttons.Add(this);
        RestColor = defaultColor;
    }

    public override void Draw()
    {
        if (selected && enabled)
        {
            I.Batch.Draw(Entity.Renderer.DefaultTexture,
                new Microsoft.Xna.Framework.Rectangle(
                    this.rect.X - 1,
                    this.rect.Y - 1,
                    this.rect.Width + 2,
                    this.rect.Height + 2
                    ),
                null,
                Color.Yellow,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                this.depth + 0.001f);
        }
        base.Draw();
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

    public Toggle(TileSet tileset, Rectangle space, UI parent) : base(tileset, space, parent) { _fonction = OnClic; }
    public Toggle(Texture2D image, Rectangle space, UI parent) : base(image, space, parent) { _fonction = OnClic; }
    public Toggle(string title, TileSet tileset, Rectangle space, UI parent) : base(title, tileset, space, parent) { _fonction = OnClic; }
    public Toggle(string title, Texture2D image, Rectangle space, UI parent) : base(title, image, space, parent) { _fonction = OnClic; }
    public Toggle(TileSet tileset, Rectangle space) : base(tileset, space) { _fonction = OnClic; }
    public Toggle(Texture2D image, Rectangle space) : base(image, space) { _fonction = OnClic; }
    public Toggle(string title, TileSet tileset, Rectangle space) : base(title, tileset, space) { _fonction = OnClic; }
    public Toggle(string title, Texture2D image, Rectangle space) : base(title, image, space) { _fonction = OnClic; }


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

    internal override void Update(Point mousePos, bool mousepressed)
    {
        base.Update(mousePos, mousepressed);
        if (!b)
        {
            this.Color = _on ? OnColor : OffColor;
        }
    }
}

public class Button : ButtonCore
{
    public Button(TileSet tileset, Rectangle space, UI parent) : base(tileset, space, parent) { }
    public Button(Texture2D image, Rectangle space, UI parent) : base(image, space, parent) { }
    public Button(TileSet tileset, Rectangle space) : base(tileset, space) { }
    public Button(Texture2D image, Rectangle space) : base(image, space) { }
    public Button(string title, TileSet tileset, Rectangle space, UI parent) : base(title, tileset, space, parent) { }
    public Button(string title, Texture2D image, Rectangle space, UI parent) : base(title, image, space, parent) { }
    public Button(string title, TileSet tileset, Rectangle space) : base(title, tileset, space) { }
    public Button(string title, Texture2D image, Rectangle space) : base(title, image, space) { }

    public Procedure Fonction
    {
        set
        {
            base._fonction = value;
        }
    }
}