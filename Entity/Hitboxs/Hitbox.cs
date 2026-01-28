

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox : IDraw, IDisposable
{
    public delegate bool ConditionToCheckCollision(Hitbox hit);

    private static HitboxLayer[] layers;

    public static void CreateLayers(params Color[] debugColors)
    {

#if DEBUG
        if (debugColors.Length > 255)
            throw new Exception("le nombre de couche ne doit pas dépasser 255.");
#endif

        layers = new HitboxLayer[debugColors.Length];
        for (int i = 0; i < debugColors.Length; i++)
        {
            layers[i] = new HitboxLayer(debugColors[i]);
        }
    }

    public static void ClearLayer(byte i)
    {
        layers[i].Clear();
    }

    public static void ClearAllLayer()
    {
        for (byte i = 0; i < layers.Length; i++)
        {
            ClearLayer(i);
        }
    }

    private readonly byte layer;
    private readonly string[] tags;
    protected Space parent;
    public bool active;

    protected Hitbox(in Space parent, byte layer, params string[] tags)
    {
#if DEBUG
        if (layers is null)
            throw new Exception("'Hitbox.CreateLayers(params Color[])' doit etre appelé avant de créer des hiboxs.");
        if (layer >= layers.Length)
            throw new Exception("la couche '" + layer + "' n'existe pas.");
#endif
        this.layer = layer;
        layers[layer].Add(this);
        this.tags = tags;
        this.parent = parent;
        active = true;
    }

    protected Hitbox() : this(Space.Zero, 0) { }
    protected Hitbox(byte layer, params string[] tags) : this(Space.Zero, layer, tags) { }
    protected Hitbox(in Space parent) : this(in parent, 0) { }
    protected Hitbox(in Space parent, params string[] tags) : this(in parent, 0, tags) { }

    public bool IsTag(string tag)
    {
        foreach (string t in this.tags)
        {
            if (tag.Equals(t))
                return true;
        }
        return false;
    }

    protected abstract void UpdatePosition();

    public abstract bool Check(byte layer, ConditionToCheckCollision condition);

    public bool Check()
    {
        return Check(this.layer, (Hitbox hit) => true);
    }

    public bool Check(string tagToCheck)
    {
        return Check(this.layer, (Hitbox hit) => hit.IsTag(tagToCheck));
    }

    public abstract void Draw(in SpriteBatch batch);

    public virtual void Dispose()
    {
        layers[layer].Remove(this);
    }
}
