using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FriteCollection2.Entity.Hitboxs;

internal class HitboxLayer : List<Hitbox>, IDraw
{
    public readonly Color debugColor;

    internal HitboxLayer(Color debugColor)
    {
        this.debugColor = debugColor;
    }

    public void Draw(SpriteBatch batch)
    {
        foreach (Hitbox hit in this)
        {
            hit.Draw(batch);
        }
    }
}
