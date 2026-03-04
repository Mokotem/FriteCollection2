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

public abstract class UI : IDraw
{
    protected const int defaultWidth = 32, defaultHeight = defaultWidth;

    protected static Screen screen;

    public static void UpdateMousePos(MouseState state)
    {
        screen.UpdateMouse(state);
    }

    public static void SetScreenResolution(int width, int height)
    {
        screen = new Screen(width, height);
    }

    private bool _active = true;
    public virtual bool Active
    {
        get => _active;
        set
        {
            if (!_active && value)
            {
                ApplyPosition(_lastPos, _lastx, _lasty);
                this.OnIShouldUpdatePositionsOfMyChilds();
            }

            _active = value;
        }
    }

    protected readonly UI parent;
    protected Rectangle rect;

    private Bounds _lastPos, _lastCenter;
    private int _lastx, _lasty;

    public int Width => rect.Width;
    public int Height => rect.Height;

    public Point Size => rect.Size;

    protected internal virtual Rectangle ParentRect => rect;
    public Rectangle Rectangle => rect;

    public abstract float Depth { get; }

    internal virtual bool IsMouseOn => parent.IsMouseOn;

    private byte _parentCount;
    public byte ParentCount => _parentCount;

    public virtual Point GetMousePos()
    {
        return parent.GetMousePos();
    }

    protected UI(UI parent, int width, int height)
    {
        if (parent is null)
        {
            _parentCount = 0;
            if (width < 90 || width < 90)
            {
                throw new System.Exception("parent cannot be null");
            }
            else
            {
                childs = new List<UI>();
                return;
            }
        }
        
        this.parent = parent;

        this._parentCount = parent._parentCount;
        this._parentCount++;

        rect = new Rectangle(
            parent.ParentRect.X,
            parent.ParentRect.Y,
            width,
            height);

        childs = new List<UI>();
        _active = true;
        parent.childs.Add(this);

        _lastPos = Bounds.TopLeft;
        _lastCenter = Bounds.TopLeft;
    }

    protected UI(UI parent, Extend extend, int width = 0, int height = 0)
        : this(parent, width, height)
    {
        _SetScale(extend);
    }

    protected UI(int width, int height) : this(screen, width, height) { }
    protected UI(Extend extend, int width = 0, int height = 0) : this(screen, extend, width, height) { }

    private List<UI> childs;

    public void ClearChilds()
    {
        childs.Clear();
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

#if DEBUG
    private void _MakeSureIHaveNoChild()
    {
        foreach(UI c in childs)
        {
            if (c.Active)
                throw new System.Exception("tu tprends pour qui, can't change size if it has childs");
        }
    }
#endif


    public void AddScale(int dx, int dy)
    {
#if DEBUG
        _MakeSureIHaveNoChild();
#endif
        rect.Width += dx;
        rect.Height += dy;
        this.OnMyScaleChange();
    }

    public void ScaleY(int height)
    {
        if (height != rect.Height)
        {
#if DEBUG
            _MakeSureIHaveNoChild();
#endif
            rect.Height = height;
        }
    }

    public void ScaleX(int width)
    {
        if (width != rect.Width)
        {
#if DEBUG
            _MakeSureIHaveNoChild();
#endif
            rect.Width = width;
        }
    }

    public void Scale(Extend ext, int width = 0, int height = 0)
    {
        ScaleX(width);
        ScaleY(height);
        _SetScale(ext);
        this.OnMyScaleChange();
    }

    public void Scale(int width, int height)
    {
        ScaleX(width);
        ScaleY(height);
        this.OnMyScaleChange();
    }

    public void Scale(int size)
    {
        ScaleX(size);
        ScaleY(size);
        this.OnMyScaleChange();
    }

    protected static Point MakePosition(Rectangle parent, Point scale, Bounds pos, Bounds center)
    {
        return parent.Location + BoundFunc.BoundToPoint(pos, parent.Width, parent.Height) - BoundFunc.BoundToPoint(center, scale.X, scale.Y);
    }

    protected static Point MakePosition(Rectangle parent, Point scale, Bounds pos)
    {
        return MakePosition(parent, scale, pos, pos);
    }

    public virtual int Bottom => rect.Bottom;
    public virtual int Left => rect.Left;
    public virtual int Right => rect.Right;
    public virtual int Top => rect.Top;

    /*
     * Note pour les positions:
     * les méthodes type 'Apply' sont relatives au parent
     * les méthodes type 'Set' agissent directement sur le rectangle
    */

    public void ApplyPosition(Bounds pos, Bounds center, int dx = 0, int dy = 0)
    {
        this._lastPos = pos;
        this._lastCenter = center;
        _lastx = dx;
        _lasty = dy;

        rect.Location = MakePosition(parent.ParentRect, rect.Size, pos, center);
        rect.X += dx;
        rect.Y += dy;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public void ApplyPositionX(int dx = 0)
    {
        _lastx = dx;

        rect.X = parent.ParentRect.X + BoundFunc.AlignToInt(BoundFunc.BoundsToAlignX(_lastPos), parent.ParentRect.Width)
            - BoundFunc.AlignToInt(BoundFunc.BoundsToAlignX(_lastCenter), rect.Width);
        rect.X += dx;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public void ApplyPositionY(int dy = 0)
    {
        _lasty = dy;

        rect.Y = parent.ParentRect.Y + BoundFunc.AlignToInt(BoundFunc.BoundsToAlignY(_lastPos), parent.ParentRect.Height)
            - BoundFunc.AlignToInt(BoundFunc.BoundsToAlignY(_lastCenter), rect.Height);
        rect.Y += dy;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public void ApplyPosition(Bounds pos, int dx = 0, int dy = 0)
    {
        this.ApplyPosition(pos, pos, dx, dy);
    }

    public void ApplyPosition(int dx = 0, int dy = 0)
    {
        this.ApplyPosition(Bounds.TopLeft, Bounds.TopLeft, dx, dy);
    }

    public void SetPosition(Bounds center, int x = 0, int y = 0)
    {
        Point s = BoundFunc.BoundToPoint(center, rect.Width, rect.Height);
        rect.X = x - s.X;
        rect.Y = y - s.Y;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public void SetPosition(int x, int y)
    {
        rect.X = x;
        rect.Y = y;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public int PositionX
    {
        get => rect.Y;
        set => SetPositionX(value);
    }

    public void SetPositionX(int x)
    {
        this.rect.X = x;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public void SetPositionX(int x, Align align)
    {
        this.SetPositionX(parent.ParentRect.X + x - BoundFunc.AlignToInt(align, rect.Width));
    }

    public int PositionY
    {
        get => rect.Y;
        set => SetPositionY(value);
    }

    public void SetPositionY(int y)
    {
        this.rect.Y = y;
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    public void SetPositionY(int y, Align align)
    {
        this.SetPositionY(parent.ParentRect.Y + y - BoundFunc.AlignToInt(align, rect.Height));
    }

    public void ApplyPosition(Point pos)
    {
        ApplyPosition(pos.X, pos.Y);
    }

    protected virtual void OnIShouldUpdatePositionsOfMyChilds()
    {
        foreach (UI c in childs)
        {
            c.OnParentPositionChanged();
        }
    }

    protected virtual void OnParentPositionChanged()
    {
        ApplyPosition(_lastPos, _lastCenter, _lastx, _lasty);
        this.OnIShouldUpdatePositionsOfMyChilds();
    }

    protected virtual void OnMyScaleChange()
    {

    }

    protected bool AmIVisible()
    {
        UI courrant = this;
        while (courrant.parent is not null)
        {
            if (!courrant.parent.Active)
            {
                return false;
            }
            courrant = courrant.parent;
        }
        return true;
    }

    public void DrawChilds(SpriteBatch batch)
    {
        foreach (UI c in childs)
        {
            c.Draw(batch);
        }
    }

    public void AddBackMyChild(UI value)
    {
        if (value.parent == this)
            this.childs.Add(value);
    }

    public virtual void Draw(SpriteBatch batch)
    {
        DrawChilds(batch);
    }

    public void FlexChildsHorizontal(int margin = 2)
    {
        childs[0].SetPositionX(this.ParentRect.X, Align.Left);
        for(int i = 1; i < childs.Count; i++)
        {
            childs[i].SetPositionX(childs[i - 1].Right + margin);
        }
    }

    public void FlexChildsVertical(int margin = 2)
    {
        if (childs.Count > 0)
        {
            childs[0].SetPositionY(this.ParentRect.Y, Align.Left);
            for (int i = 1; i < childs.Count; i++)
            {
                childs[i].SetPositionY(childs[i - 1].Bottom + margin);
            }
        }
    }

    /// <summary>
    /// barbar
    /// </summary>
    public static void DrawRoot(SpriteBatch batch)
    {
        screen.Draw(batch);
    }
}

public class Screen : UI
{
    internal Screen(int width, int height) : base(width, height)
    {
        rect = new Rectangle(0, 0, width, height);
        this.Active = true;
    }

    private Point mousePos;

    public override float Depth => 1f;

    internal void UpdateMouse(MouseState mouse)
    {
        this.mousePos = mouse.Position;
    }

    public override Point GetMousePos()
    {
        return mousePos;
    }

    protected override void OnParentPositionChanged()
    {
        throw new System.Exception("I have no parent, i am the beginning of the universe");
    }
}
   