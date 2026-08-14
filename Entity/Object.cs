using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity;

public class Object : Space, IDraw
{
    public readonly OutlineRenderer Renderer;
    private Rectangle sub;

    public Object() : base()
    {
        Renderer = new OutlineRenderer(0);
    }

    public Object(Texture2D texture) : base(texture)
    {
        Renderer = new OutlineRenderer(texture);
    }

    public Object(int width, int height) : base(width, height)
    {
        Renderer = new OutlineRenderer();
        Renderer.sub.Width = width;
        Renderer.sub.Height = height;
    }

    public Object(Texture2D renderer, int width, int height) : base(width, height)
    {
        Renderer = new OutlineRenderer(renderer);
        Renderer.sub.Width = width;
        Renderer.sub.Height = height;
    }

    public virtual void DrawBody(SpriteBatch batch)
    {
        Renderer.DrawBody(batch, this.ToScreen());
    }

    public void DrawOutline(SpriteBatch batch)
    {
        Renderer.DrawOutline(batch, this.ToScreen());
    }

    public virtual void Draw(SpriteBatch batch)
    {
        Renderer.Draw(batch, this.ToScreen());
    }

    public override bool Equals(object obj)
    {
        if (obj is Object)
        {
            return base.Equals((Object)obj)
                && Renderer.Equals(((Object)obj).Renderer);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }

    public override string ToString()
    {
        return "Object (" + base.ToString() + ", " + Renderer.ToString() + ")";
    }
}

public class RotatableObject : Object
{
    public float rotation;
    public Vector2 center;
    private readonly Bounds centerBound;

    public required Bounds RotationCenterPoint
    {
        init
        {
            this.centerBound = value;
            this.center = BoundFunc.BoundToVector(value, Renderer.Width, Renderer.Height);
        }
    }

    public RotatableObject() : base() { }
    public RotatableObject(Texture2D texture) : base(texture) { }
    public RotatableObject(int width, int height) : base(width, height) { }

    public void UpdateCenterPoint()
    {
        this.center = BoundFunc.BoundToVector(centerBound, Renderer.Width, Renderer.Height);
    }

    public override void Draw(SpriteBatch batch)
    {
        Renderer.Draw(batch, ToScreen(), center, rotation);
    }

    public override void SetPosition(Vector2 pos, Bounds centerPoint)
    {
        base.SetPosition(pos, centerPoint);
    }
}