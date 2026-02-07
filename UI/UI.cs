using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FriteCollection2.UI;

public class Extend
{
    public static readonly Extend
        None = new Extend(false, false),
        Full = new Extend(true, true),
        Horizontal = new Extend(true, false),
        Vertical = new Extend(false, true);

    public readonly bool x, y;
    private Extend(bool onx, bool ony)
    {
        this.x = onx;
        this.y = ony;
    }
}

public abstract class UI : IDraw, IHaveRectangle
{
    protected const int padding = 8, padding2 = padding * 2;
    protected const int defaultWidth = 32, defaultHeight = defaultWidth;

    private protected static Screen screen;

    public static void UpdateMousePos(MouseState state)
    {
        screen.UpdateMouse(state);
    }

    public static void SetScreenResolution(ushort width, ushort height)
    {
        screen = new Screen(width, height);
    }

    public bool Active;
    protected readonly UI parent;
    protected Rectangle rect;

    protected internal virtual Rectangle ParentRect => rect;
    public Rectangle mRect => ParentRect;
    public abstract float Depth { get; }

    internal virtual bool IsMouseOn => parent.IsMouseOn;

    internal virtual Point GetMousePos()
    {
        return parent.GetMousePos();
    }

    protected UI(UI parent, int width, int height)
    {
        rect = new Rectangle(0, 0, width, height);
        this.parent = parent;
        childs = new List<UI>();
        Active = true;
        parent.childs.Add(this);
    }

    public void FlexChildsVertical()
    {
        for (byte i = 1; i < childs.Count; i++)
        {
            childs[i].rect.Y = childs[i - 1].Bottom + padding;
            childs[i].OnPositionChanged();
        }
    }

    protected UI(int width, int height) : this(screen, width, height) { }

    protected UI(Vector2 scale) : this(
        (int)float.Round(scale.X),
        (int)float.Round(scale.Y)
        ) { }

    protected UI(UI parent) : this(parent, defaultWidth, defaultHeight) { }
    protected UI()
    {
        childs = new List<UI>();
    }

    private List<UI> childs;


    public void ApplyScale(int width = 0, int height = 0)
    {
        rect.Width = width;
        rect.Height = height;
        OnSizeChanged();
    }

    private void _SetScale(Extend ext)
    {
        if (ext.x)
        {
            rect.Width = parent.ParentRect.Width;
        }

        if (ext.y)
        {
            rect.Height = parent.ParentRect.Height;
        }
    }

    public void ApplyScale(Extend ext)
    {
        _SetScale(ext);
        OnSizeChanged();
    }

    public void ApplyScale(Extend ext, int addWidth = 0, int addHeight = 0)
    {
        _SetScale(ext);
        rect.Width += addWidth;
        rect.Height += addHeight;
        OnSizeChanged();
    }

    protected virtual void OnSizeChanged()
    {

    }

    protected static Point MakePosition(Rectangle parent, Point scale, Bounds pos)
    {
        Point result = new Point(0, 0);
        if ((int)pos % 3 == 1)
        {
            result.X = parent.X + (parent.Width - scale.X) / 2;
        }
        else if ((int)pos % 3 == 2)
        {
            result.X = parent.Right - scale.X;
        }
        else
        {
            result.X = parent.X;
        }

        if ((int)pos / 3 == 1)
        {
            result.Y = parent.Y + (parent.Height - scale.Y) / 2;
        }
        else if ((int)pos / 3 == 2)
        {
            result.Y = parent.Bottom - scale.Y;
        }
        else
        {
            result.Y = parent.Y;
        }

        return result;
    }

    public virtual int Bottom => rect.Bottom;


    public void ApplyPosition(Bounds pos, int x = 0, int y = 0)
    {
        rect.Location = MakePosition(parent.ParentRect, rect.Size, pos);
        rect.X += x;
        rect.Y += y;
        OnPositionChanged();
    }

    public void ApplyPosition(int x = 0, int y = 0)
    {
        ApplyPosition(Bounds.TopLeft, x, y);
    }

    protected virtual void OnPositionChanged()
    {

    }

    protected void DrawChilds(in SpriteBatch batch)
    {
        if (Active)
        {
            foreach (UI c in childs)
            {
                c.Draw(in batch);
            }
        }
    }

    public abstract void Draw(in SpriteBatch batch);

    public static void DrawRoot(in SpriteBatch batch)
    {
        screen.Draw(in batch);
    }
}

internal class Screen : UI
{
    internal Screen(int width, int height)
    {
        rect = new Rectangle(0, 0, width, height);
    }

    private Point mousePos;

    public override float Depth => 0f;

    internal void UpdateMouse(MouseState mouse)
    {
        this.mousePos = mouse.Position;
    }


    internal override Point GetMousePos()
    {
        return mousePos;
    }

    public override void Draw(in SpriteBatch batch)
    {
        DrawChilds(in batch);
    }
}
   