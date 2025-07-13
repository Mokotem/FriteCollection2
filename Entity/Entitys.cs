using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity;

public abstract class Entity
{
    public Space Space = new Space();

    public Renderer Renderer = new Renderer();

    public virtual void Draw() { }
}

/// <summary>
/// Object.
/// </summary>
public partial class Object : Entity, ICopy<Object>, IDraw
{
    public Object Copy()
    {
        return new()
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy()
        };
    }

    public override void Draw()
    {
        if (!Renderer.hide)
        {
            Point pos = new Point((int)(float.Round(Space.Position.X) - Camera.Position.X),
                    (int)(float.Round(Space.Position.Y) - Camera.Position.Y));
            Point scale = new Point((int)float.Round(Space.Scale.X),
                    (int)float.Round(Space.Scale.Y));

            if (Renderer.outline)
            {
                foreach (Point r in new Point[8]
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
                    GameManager.Instance.SpriteBatch.Draw
            (
                Renderer.Texture,
                new Rectangle(pos + r, scale),
                null,
                Renderer.outlineColor,
                0,
                Vector2.Zero,
                Renderer.effect,
                Renderer.outlineLayer
            );
                }
            }
            GameManager.Instance.SpriteBatch.Draw
            (
                Renderer.Texture,
                new Rectangle(pos, scale),
                null,
                Renderer.Color,
                0,
                Vector2.Zero,
                Renderer.effect,
                Renderer.GetLayer()
            );
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Object)
        {
            return Space.Equals((obj as Object).Space)
                && Renderer.Equals((obj as Object).Renderer);
        }
        return false;
    }

    public override string ToString()
    {
        return "Object (" + Space.ToString() + ", " + Renderer.ToString() + ")";
    }
}