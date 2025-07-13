using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FriteCollection2.Tools.TileMap;
using System.Collections.Generic;
using System;

namespace FriteCollection2.UI;

public enum Extend
{
    Horizontal, Vertical, Full, None
}

public interface IEdit<T>
{
    public T Edit
    {
        get;
        set;
    }
}

public abstract class UI : IDisposable
{
    internal float depth = 0.5f;
    public class Rectangle
    {
        public Extend Extend = Extend.None;
        public Point Position = Point.Zero;
        public Point Scale = Point.Zero;
        public Bounds Origin;
        public static void SetDefaultEnvironment(in Environment env)
        {
            _defaultEnvironment = env;
            _default = new Microsoft.Xna.Framework.Rectangle(0, 0, env.Target.Width, env.Target.Height);
        }

        private static Microsoft.Xna.Framework.Rectangle _default;
        public static Microsoft.Xna.Framework.Rectangle Default => _default;

        private static Environment _defaultEnvironment;
        public static Environment DefaultEnvironment => _defaultEnvironment;

        public readonly Environment environment;
        public Microsoft.Xna.Framework.Rectangle EnviRect => this.environment.Target.Bounds;

        public Rectangle(Bounds origin, Extend extend)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.environment = _defaultEnvironment;
        }

        public Rectangle(Bounds origin, Extend extend, Point scale)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.Scale = scale;
            this.environment = _defaultEnvironment;
        }

        public Rectangle(Bounds origin, Extend extend, Point scale, Point position)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.Scale = scale;
            this.Position = position;
            this.environment = _defaultEnvironment;
        }

        public Rectangle(byte env, Bounds origin, Extend extend)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.environment = GameManager.Environments[env];
        }

        public Rectangle(byte env, Bounds origin, Extend extend, Point scale)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.Scale = scale;
            this.environment = GameManager.Environments[env];
        }

        public Rectangle(byte env, Bounds origin, Extend extend, Point scale, Point position)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.Scale = scale;
            this.Position = position;
            this.environment = GameManager.Environments[env];
        }

        public Rectangle(in Environment env, Bounds origin, Extend extend)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.environment = env;
        }

        public Rectangle(in Environment env, Bounds origin, Extend extend, Point scale)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.Scale = scale;
            this.environment = env;
        }

        public Rectangle(in Environment env, Bounds origin, Extend extend, Point scale, Point position)
        {
            this.Origin = origin;
            this.Extend = extend;
            this.Scale = scale;
            this.Position = position;
            this.environment = env;
        }
    }

    public virtual int PositionY
    {
        get => papa.Position.Y;
        set
        {
            space.Position.Y = value;
            ApplyPosition(papa is null ? space.EnviRect : papa.mRect);
        }
    }

    public virtual Point Position
    {
        get => space.Position;
        set
        {
            space.Position = value;
            ApplyPosition(papa is null ? space.EnviRect : papa.mRect);
        }
    }

    public virtual void Dispose()
    {
        if (papa is not null)
            papa.Dispose();
    }

    private protected bool _active = true;
    public delegate void Procedure();
    private protected UI papa;

    public UI()
    {
        GameManager.Instance.OnScreenUpdate += this.OnScreenUpdate;
    }

    private void OnScreenUpdate(bool full)
    {
        if (papa is null)
        {
            this.ApplySpace();
        }
    }

    public Point Scale
    {
        get => space.Scale;
        set
        {
            space.Scale = value;
            ApplySpace(papa is null ? space.EnviRect : papa.mRect);
        }
    }

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            foreach(UI c in childs)
            {
                c.Active = value;
            }
        }
    }

    private protected Microsoft.Xna.Framework.Rectangle rect;

    private protected List<UI> childs = new List<UI>();
    public UI[] Childs => childs.ToArray();

    public void Add(UI element)
    {
        element.depth = this.depth - 0.05f;
        childs.Add(element);
    }
    public void FlexChilds(Point spacing, Point offset)
    {
        Point cursor = spacing + offset;
        int maxHeight = -1;
        foreach(UI u in childs)
        {
            if (u.Active)
            {
                if (cursor.X + u.rect.Width + spacing.X > this.rect.Width)
                {
                    cursor.X = spacing.X;
                    cursor.Y += maxHeight + spacing.Y;
                    maxHeight = u.rect.Height;
                }
                if (u.rect.Height > maxHeight)
                    maxHeight = u.rect.Height;
                u.Position = cursor;
                cursor.X += u.rect.Width + spacing.X;
            }
        }
    }

    public Microsoft.Xna.Framework.Rectangle mRect => rect;

    public Color Color = Color.White;

    private protected Rectangle space;
    internal Rectangle Space => space;

    private protected void ApplyScale(Microsoft.Xna.Framework.Rectangle parent)
    {
        switch (space.Extend)
        {
            case Extend.None:
                rect.Width = 0;
                rect.Height = 0;
                break;

            case Extend.Full:
                rect.Width = parent.Width;
                rect.Height = parent.Height;
                break;

            case Extend.Horizontal:
                rect.Width = parent.Width;
                rect.Height = 0;
                break;

            case Extend.Vertical:
                rect.Width = 0;
                rect.Height = parent.Height;
                break;
        }

        rect.Width += space.Scale.X;
        rect.Height += space.Scale.Y;

        foreach (UI e in childs)
        {
            e.ApplyScale(this.rect);
        }
    }

    internal virtual void ApplyPosition(Microsoft.Xna.Framework.Rectangle parent)
    {
        switch ((int)space.Origin % 3)
        {
            default:
                rect.X = parent.X;
                break;

            case 1:
                rect.X = parent.X + (parent.Width / 2) - (rect.Width / 2);
                break;

            case 2:
                rect.X = parent.X + parent.Width - rect.Width;
                break;
        }

        switch ((int)space.Origin / 3)
        {
            default:
                rect.Y = parent.Y;
                break;

            case 1:
                rect.Y = parent.Y + (parent.Height / 2) - (rect.Height / 2);
                break;

            case 2:
                rect.Y = parent.Y + parent.Height - rect.Height;
                break;
        }

        rect.X += space.Position.X;
        rect.Y += space.Position.Y;

        foreach(UI e in childs)
        {
            e.ApplyPosition(this.rect);
        }
    }

    private protected virtual void ApplySpace(Microsoft.Xna.Framework.Rectangle parent)
    {
        ApplyScale(parent);
        ApplyPosition(parent);
    }

    public void ApplySpace()
    {
        ApplyScale(papa is null ? space.EnviRect : papa.mRect);
        ApplyPosition(papa is null ? space.EnviRect : papa.mRect);
    }

    public virtual void Draw() { }
}

public class Image : UI, IEdit<Texture2D>
{
    private Texture2D image;

    public Texture2D Edit
    {
        get => image;
        set => image = value;
    }

    public Image(Texture2D image, Rectangle space) : base()
    {
        this.image = image;
        this.space = space;
        base.ApplyScale(space.EnviRect);
        base.ApplyPosition(space.EnviRect);
    }

    public Image(Texture2D image, Rectangle space, UI parent) : base()
    {
        this.image = image;
        this.space = space;
        this.papa = parent;
        base.ApplyScale(parent.mRect);
        base.ApplyPosition(parent.mRect);
        this.depth = parent.depth - 0.05f;
    }

    public override void Draw()
    {
        if (_active)
        {
            GameManager.Instance.SpriteBatch.Draw(
                image,
                rect,
                null,
                this.Color,
                0, Vector2.Zero, SpriteEffects.None,
                this.depth);
            foreach (UI element in childs)
                element.Draw();
        }
    }

    public override void Dispose()
    {
        if (image is not null)
        {
            image.Dispose();
            image = null;
        }
    }
}

public class Text : UI, IEdit<string>
{
    private Microsoft.Xna.Framework.Rectangle par;
    private string text;
    public bool Outline;

    public static Color OutlineColor = Color.Black;

    private static Point fontaspect;
    public static void SetFontAspect(Point p)
    {
        fontaspect = p;
        HasFontAspect = true;
    }

    private static bool HasFontAspect = false;

    public float Size { get; set; }

    public string Edit
    {
        get => text;
        set
        {
            if (value != text)
            {
                this.text = value;
            }
        }
    }

    public void SetPar(Microsoft.Xna.Framework.Rectangle rect1)
    {
        par = rect1;
    }

    public override int PositionY
    {
        get => papa.Position.Y;
        set
        {
            space.Position.Y = value;
            ApplyPosition(par);
        }
    }

    private protected override void ApplySpace(Microsoft.Xna.Framework.Rectangle parent)
    {
        ApplyScale(parent);
        ApplyText(this.text);
        ApplyPosition(parent);
    }

    private void ApplyText(string input)
    {
        text = "";
        string[] txt = input.Split(" ");
        int i = 0;
        Point s = Point.Zero;
        int ln = 1;
        if (txt.Length < 2 || par.Width < 2)
        {
            text = input;
        }
        else
        {
            if (HasFontAspect)
            {
                while (i < txt.Length)
                {
                    string line = "";
                    while (i < txt.Length && line.Length * fontaspect.X < par.Width)
                    {
                        s.X += fontaspect.X;
                        line += txt[i] + " ";
                        i += 1;
                    }
                    text += line + "\n";
                    ln += 1;
                    s.Y += fontaspect.Y;
                }
            }
            else
            {
                while (i < txt.Length)
                {
                    string line = "";
                    while (i < txt.Length && GameManager.Font.MeasureString(line).X < par.Width)
                    {
                        line += txt[i] + " ";
                        i += 1;
                    }
                    text += line + "\n";
                    ln += 1;
                    s.Y += fontaspect.Y;
                }
            }
            ln += -1;
            text = text.Remove(text.Length - 2);
        }

        if (HasFontAspect)
        {
            rect.Width = ((text.Length - 2) * fontaspect.X);
            rect.Height = fontaspect.Y * ln - 2;
        }
        else
        {
            Vector2 fa = GameManager.Font.MeasureString(text);
            rect.Width = (int)fa.X;
            rect.Height = (int)fa.Y;
        }
    }

    public Text(string txt, Rectangle space) : base()
    {
        this.Size = 1f;
        this.space = space;
        base.ApplyScale(space.EnviRect);
        ApplyText(txt);
        base.ApplyPosition(space.EnviRect);
        par = space.EnviRect;
        Outline = true;
    }

    public Text(string txt, Rectangle space, UI parent) : base()
    {
        this.papa = parent;
        this.Size = 1f;
        this.space = space;
        base.ApplyScale(parent.mRect);
        ApplyText(txt);
        base.ApplyPosition(parent.mRect);
        par = parent.mRect;
        Outline = true;
        this.depth = parent.depth - 0.05f;
    }

    public override void Draw()
    {
        if (_active)
        {
            if (Outline)
            {
                foreach (Vector2 r in new Vector2[8]
                {
                new(-1, 1),
                new(0, 1),
                new(1, 1),

                new(-1, 0),
                new(1, 0),

                new(-1, -1),
                new(0, -1),
                new(1, -1)
                })
                {
                    GameManager.Instance.SpriteBatch.DrawString
                    (GameManager.Font, text, new Vector2(rect.X + r.X, rect.Y + r.Y),
                    OutlineColor, 0, Vector2.Zero, Size,
                    SpriteEffects.None, this.depth + 0.0001f);
                }
            }
        }
        GameManager.Instance.SpriteBatch.DrawString
                    (GameManager.Font, text, new Vector2(rect.X, rect.Y),
                    this.Color, 0, Vector2.Zero, Size,
                    SpriteEffects.None, this.depth);
    }
}


public class Panel : UI, IDisposable, IEdit<Texture2D>
{
    private Texture2D texture;
    private RenderTarget2D rt;

    public Texture2D Edit
    {
        get => texture;
        set => this.texture = value;
    }

    public static Texture2D CreatePanelTexture(
        TileSet set,
        Point size)
    {
        int sx = set.TileSize.X;
        int sy = set.TileSize.Y;
        if (size.X< sx * 2)
        {
            sx = size.X/ 2;
        }
        if (size.Y < sy * 2)
        {
            sy = size.Y / 2;
        }

        GraphicsDevice gd = GameManager.Instance.GraphicsDevice;
        SpriteBatch sb = GameManager.Instance.SpriteBatch;
        RenderTarget2D rt = new RenderTarget2D(gd, size.X, size.Y);

        gd.SetRenderTarget(rt);
        gd.Clear(Color.Transparent);
        sb.Begin(samplerState: SamplerState.PointClamp);

        for (int x = 0; x < 3; x++)
        {
            int width;
            if (x == 0 || x == 2)
                width = sx;
            else
                width = size.X - (sx * GameManager.Settings.UICoef);

            int posX;
            if (x == 0)
                posX = 0;
            else if (x == 1)
                posX = sx;
            else
                posX = size.X - sx;


            for (int y = 0; y < 3; y++)
            {
                int height;
                if (y == 0 || y == 2)
                    height = sy;
                else
                    height = size.Y - (sy * GameManager.Settings.UICoef);

                int posY;
                if (y == 0)
                    posY = 0;
                else if (y == 1)
                    posY = sy;
                else
                    posY = size.Y - sy;

                sb.Draw(set.Texture,
                    new Microsoft.Xna.Framework.Rectangle(posX, posY, width, height),
                    set.GetRectangle(x + (y * 3)),
                    Microsoft.Xna.Framework.Color.White);
            }
        }

        sb.End();
        return rt;
    }

    public void Clear()
    {
        foreach(UI c in childs)
        {
            c.Active = false;
        }
        this.childs.Clear();
    }

    public Panel(Rectangle space) : base()
    {
        this.space = space;
        ApplySpace(space.EnviRect);
    }

    public Panel(Rectangle space, UI parent) : base()
    {
        this.papa = parent;
        this.space = space;
        ApplySpace(parent.mRect);
        this.depth = parent.depth - 0.05f;
    }

    public Panel(TileSet tileSet, Rectangle space) : base()
    {
        this.space = space;
        ApplySpace(space.EnviRect);
        this.texture = CreatePanelTexture(tileSet, new Point(rect.Width, rect.Height));
    }

    public Panel(TileSet tileSet, Rectangle space, UI parent) : base()
    {
        this.papa = parent;
        this.space = space;
        ApplySpace(parent.mRect);
        this.texture = CreatePanelTexture(tileSet, new Point(rect.Width, rect.Height));
        this.depth = parent.depth - 0.05f;
    }

    public Panel(Texture2D image, Rectangle space) : base()
    {
        this.space = space;
        ApplySpace(space.EnviRect);
        this.texture = image;
    }

    public Panel(Texture2D image, Rectangle space, UI parent) : base()
    {
        this.papa = parent;
        this.space = space;
        ApplySpace(parent.mRect);
        this.texture = image;
        this.depth = parent.depth - 0.05f;
    }

    public override void Draw()
    {
        if (_active)
        {
            if (texture != null)
                GameManager.Instance.SpriteBatch.Draw
                 (texture, rect, null, Color,
                 0, Vector2.Zero, SpriteEffects.None, this.depth);
            foreach (UI element in childs)
                element.Draw();
        }
    }

    public override void Dispose()
    {
        if (texture is not null)
        texture.Dispose();
        if (rt is not null)
            rt.Dispose();
        rt = null;
        texture = null;
        foreach (UI ui in childs)
        {
            ui.Dispose();
        }
    }
}
