using Autofac.Core;
using FriteCollection2.Entity;
using FriteCollection2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

/// <summary>
/// Un module 
/// </summary>
namespace FriteCollection2;

public interface IDraw
{
    public delegate void DrawFunction(in SpriteBatch batch);
    public void Draw(in SpriteBatch batch);
}

public interface IDrawUI
{
    public void Draw(in SpriteBatch batch, int width, int height) { }
}

public enum Bounds
{
    TopLeft, Top, TopRight,
    Left, Center, Right,
    BottomLeft, Bottom, BottomRight,
}

public enum Align
{
    Left, Center, Right
}

/// <summary>
/// Représente un endroit pour dessiner.
/// </summary>
public class Environment : IDraw, IHaveRectangle
{
    public Rectangle Rect { get; set; }
    public RenderTarget2D Target { get; private set; }
    public Vector2[] Bounds { get; private set; }

    public Rectangle TargetRect => new Rectangle(0, 0, Target.Width, Target.Height);
    public Rectangle mRect => new Rectangle(0, 0, Rect.Width, Rect.Height);
    public float Depth => 0.5f;

    public Environment(Rectangle t, RenderTarget2D r)
    {
        Rect = t;
        Target = r;
        Bounds = BoundFunc.CreateBounds(r.Width, r.Height);
    }

    public void Edit(Rectangle t, RenderTarget2D r)
    {
        Rect = t;
        Target = r;
        Bounds = BoundFunc.CreateBounds(r.Width, r.Height);
    }

    public void Draw(in SpriteBatch batch)
    {
        batch.Draw(Target, Rect, Color.White);
    }

    public void Draw(in SpriteBatch batch, float depth)
    {
        batch.Draw(Target, Rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, depth);
    }

    public void Draw(in SpriteBatch batch, int amount)
    {
        batch.Draw(Target,
            new Rectangle(Rect.X, Rect.Y, Rect.Width, amount),
            new Rectangle(0, 0, Target.Width, amount),
            Color.White);
    }

    public void Draw(in SpriteBatch batch, int amount, float depth)
    {
        batch.Draw(Target,
             new Rectangle(Rect.X, Rect.Y, Rect.Width, amount),
             new Rectangle(0, 0, Target.Width, amount), Color.White, 0, Vector2.Zero, SpriteEffects.None,
             depth);
    }
}

public interface IExecutable : IDraw
{
    public void Load(SpriteBatch batch, GraphicsDevice device) { }
    public void Start();
    public void Update(float dt);
}

/// <summary>
/// Représente un objet qui sera appelé dans la boucle principale.
/// </summary>
public abstract class AdvancedExecutable : IExecutable, IDrawUI, IDisposable
{
    private static ushort currentId = 0;
    internal readonly ushort id;

    protected AdvancedExecutable()
    {
        this.id = currentId;
        currentId++;
    }

    protected AdvancedExecutable(ushort id)
    {
        this.id = id;
    }

    public override int GetHashCode()
    {
        return id;
    }

    public override bool Equals(object obj)
    {
        if (obj is AdvancedExecutable)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        return false;
    }

    public virtual void Start() { }

    public virtual void Load(in SpriteBatch batch, GraphicsDevice gd) { }
    public virtual void AfterStart() { }

    public virtual void BeforeUpdate(float dt) { }
    public virtual void AfterUpdate(float dt) { }
    public virtual void WhenPaused(float dt) { }


    public virtual void DrawBackground(in SpriteBatch batch) { }
    public virtual void DrawShader(in SpriteBatch batch, GraphicsDevice device) { }
    public virtual void AfterDraw(in SpriteBatch batch) { }
    public virtual void DrawUI(in SpriteBatch batch, int width, int height) { }
    public virtual void DrawMain(in SpriteBatch batch) { }


    public virtual void Update(float dt) { }

    public virtual void Draw(in SpriteBatch batch) { }

    public virtual void Dispose() { }
}

public class Scene : AdvancedExecutable
{
    private protected readonly List<AdvancedExecutable> exes;
    public AdvancedExecutable[] Scripts => exes.ToArray();

    public int Count => exes.Count;


    public Scene(params AdvancedExecutable[] exes)
    {
        this.exes = new List<AdvancedExecutable>(exes);
    }

    public override void Load(in SpriteBatch batch, GraphicsDevice gd)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].Load(batch, gd);
    }

    public void Add(AdvancedExecutable script)
    {
        exes.Add(script);
    }

    public void Clear()
    {
        exes.Clear();
    }

    public override void Start()
    {
        byte i;
        for (i = 0; i < exes.Count; i++)
            exes[i].Start();
        for (i = 0; i < exes.Count; i++)
            exes[i].AfterStart();
    }

    public override void Update(float dt)
    {
        byte i;
        for (i = 0; i < exes.Count; i++)
            exes[i].BeforeUpdate(dt);
        for (i = 0; i < exes.Count; i++)
            exes[i].Update(dt);
        for (i = 0; i < exes.Count; i++)
            exes[i].AfterUpdate(dt);
    }
    public override void WhenPaused(float dt)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].WhenPaused(dt);
    }

    public override void Draw(in SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].Draw(in batch);
    }

    public override void DrawBackground(in SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawBackground(in batch);
    }

    public override void DrawShader(in SpriteBatch batch, GraphicsDevice device)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawShader(in batch, device);
    }

    public override void AfterDraw(in SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].AfterDraw(in batch);
    }

    public override void DrawUI(in SpriteBatch batch, int width, int height)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawUI(in batch, width, height);
    }

    public override void DrawMain(in SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawMain(in batch);
    }

    public override void Dispose()
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].Dispose();
    }
}

public class Clone : AdvancedExecutable
{
    public bool IsDestroyed { get; protected set; }
}


public class CloneContainer : AdvancedExecutable
{
    private readonly List<Clone> clones;

    public CloneContainer(params Clone[] clones)
    {
        this.clones = new List<Clone>(clones);
    }

    public void Add(Clone script)
    {
        clones.Add(script);
    }

    public void Clear()
    {
        clones.Clear();
    }


    public override void Load(in SpriteBatch batch, GraphicsDevice device)
    {
        foreach (Clone c in clones)
        {
            c.Load(in batch, device);
        }
    }

    public override void Start()
    {
        foreach(Clone c in clones)
        {
            c.Start();
        }
    }

    public override void AfterStart()
    {
        foreach (Clone c in clones)
        {
            c.AfterStart();
        }
    }

    public override void BeforeUpdate(float dt)
    {
        for (int i = 0; i < clones.Count; i++)
        {
            if (clones[i].IsDestroyed)
                clones.RemoveAt(i);
            else
                clones[i].BeforeUpdate(dt);
        }
    }

    public override void Update(float dt)
    {
        for (int i = 0; i < clones.Count; i++)
        {
            if (clones[i].IsDestroyed)
                clones.RemoveAt(i);
            else
                clones[i].Update(dt);
        }
    }

    public override void AfterUpdate(float dt)
    {
        for (int i = 0; i < clones.Count; i++)
        {
            if (clones[i].IsDestroyed)
                clones.RemoveAt(i);
            else
                clones[i].AfterUpdate(dt);
        }
    }

    public override void Draw(in SpriteBatch batch)
    {
        foreach (Clone c in clones)
        {
            c.Draw(in batch);
        }
    }
}