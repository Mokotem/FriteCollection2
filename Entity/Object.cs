using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriteCollection2.Entity;

public class Object : Space, IDraw
{
    public TextureRenderer Renderer;

    public static float outlineLayer = 0.55f;
    public float outLayer = Object.outlineLayer;

    public Object() : base()
    {
        Renderer = new TextureRenderer();
    }

    public Object(Texture2D texture) : base(texture)
    {
        Renderer.Texture = texture;
    }

    public Object(int width, int height) : base(width, height)
    {
        Renderer = new TextureRenderer();
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