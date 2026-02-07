

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using System;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox : IDraw, IDisposable
{
    public delegate bool ConditionToCheckCollision(Hitbox hit);

    protected static bool SelectAllHitboxs(Hitbox hit) => true;


    protected static ConditionToCheckCollision SelectTag(string tag) => (Hitbox hit) => hit.IsTag(tag);


    private static HitboxLayer[] layers;

    public struct CollisionData<T> where T : Hitbox
    {
        public readonly T collider;
        public readonly Sides side;

        public CollisionData(in T col, Sides side)
        {
            this.collider = col;
            this.side = side;
        }
    }

    public delegate bool HitboxMessage(short value);

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

    public readonly byte layer;
    private readonly string[] tags;
    protected readonly Space _parent;

    public Space Parent => _parent;

    public HitboxMessage SendMessage { get; init; }

    public bool active;
    public bool isStatic;

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
        this._parent = parent;
        active = true;
        isStatic = false;
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

    public bool IsTag(char value, int index)
    {
        foreach (string t in this.tags)
        {
            if (t[0].Equals(value))
                return true;
        }
        return false;
    }

    public bool HasTagsOf(Hitbox other)
    {
        if (tags.Length > other.tags.Length)
            return false;

        foreach (string tag in tags)
        {
            if (!other.IsTag(tag))
            {
                return false;
            }
        }
        return true;
    }

    public bool HasExactSameTagsAs(Hitbox other)
    {
        if (this.tags.Length < other.tags.Length)
            return false;

        return HasTagsOf(other);
    }

    public abstract void UpdatePosition(float x, float y);

    public void UpdatePosition()
    {
        UpdatePosition(_parent.X, _parent.Y);
    }

    public abstract bool Check(byte layer, ConditionToCheckCollision condition);

    public bool Check()
    {
        return Check(this.layer, (Hitbox hit) => true);
    }

    public bool Check(string tagToCheck)
    {
        return Check(this.layer, SelectTag(tagToCheck));
    }

    public bool Check(ConditionToCheckCollision condition)
    {
        return Check(this.layer, condition);
    }

    public bool Check(byte layer)
    {
        return Check(layer, SelectAllHitboxs);
    }

    public bool Check(byte layer, string tagToCheck)
    {
        return Check(layer, SelectTag(tagToCheck));
    }

    public abstract void Draw(in SpriteBatch batch);

    public static void Debug(in SpriteBatch batch)
    {
        foreach(HitboxLayer l in layers)
        {
            l.Draw(in batch);
        }
    }

    public virtual void Dispose()
    {
        layers[layer].Remove(this);
    }

    public void Destroy()
    {
        Dispose();
    }

    public void Reactivate()
    {
        layers[layer].Add(this);
    }
}
