using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity;

public class Object : Space, IDraw
{
    public OutlineRenderer Renderer;

    public static float outlineLayer = 0.55f;
    public float outLayer = Object.outlineLayer;

    public Object() : base()
    {
        Renderer = new OutlineRenderer(0);
    }

    public Object(Texture2D texture) : base(texture)
    {
        Renderer.Texture = texture;
    }

    public Object(int width, int height) : base(width, height)
    {
        Renderer = new OutlineRenderer(0);
    }

    public void DrawBody(in SpriteBatch batch)
    {
        Renderer.DrawBody(in batch, this.ToScreen());
    }

    public void DrawOutline(in SpriteBatch batch)
    {
        Renderer.DrawOutline(in batch, this.ToScreen());
    }

    public virtual void Draw(in SpriteBatch batch)
    {
        Renderer.Draw(in batch, this.ToScreen());
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

    public override string ToString()
    {
        return "Object (" + base.ToString() + ", " + Renderer.ToString() + ")";
    }
}

public class RotatableObject : Object
{
    public float rotation;
    public Vector2 center;

    public void SetCenterPoint(Bounds bound)
    {
        center = BoundFunc.BoundToVector(bound, Renderer.Width, Renderer.Height);
    }

    public RotatableObject() : base() { }
    public RotatableObject(Texture2D texture) : base(texture) { }
    public RotatableObject(int width, int height) : base(width, height) { }

    public override void Draw(in SpriteBatch batch)
    {
        Renderer.Draw(in batch, ToScreen(), center, rotation);
    }
}