
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
    public delegate void DrawFunction(SpriteBatch batch);
    public void Draw(SpriteBatch batch);
}

public interface IDrawUI
{
    public void Draw(SpriteBatch batch, int width, int height) { }
}

public enum Align
{
    Left = -1, Center = 0, Right = 1
}

public enum Sides
{
    Up, Down, Left, Right, Center
}

public enum Bounds
{
    TopLeft, Top, TopRight,
    Left, Center, Right,
    BottomLeft, Bottom, BottomRight
}

public interface IHaveRectangle
{
    public Rectangle mRect { get; }
    public float Depth { get; }

}

/// <summary>
/// Représente un endroit pour dessiner.
/// </summary>
public class Environment : IDraw, IHaveRectangle
{
    public Rectangle Rect { get; set; }
    public RenderTarget2D Target { get; private set; }

    public Rectangle TargetRect => new Rectangle(0, 0, Target.Width, Target.Height);
    public Rectangle mRect => new Rectangle(0, 0, Rect.Width, Rect.Height);
    public float Depth => 0.5f;

    public Environment(Rectangle rect, RenderTarget2D target)
    {
        this.Rect = rect;
        this.Target = target;
    }

    public void Draw(SpriteBatch batch)
    {
        batch.Draw(Target, Rect, Color.White);
    }

    public void Draw(SpriteBatch batch, float depth)
    {
        batch.Draw(Target, Rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, depth);
    }

    public void Draw(SpriteBatch batch, int amount)
    {
        batch.Draw(Target,
            new Rectangle(Rect.X, Rect.Y, Rect.Width, amount),
            new Rectangle(0, 0, Target.Width, amount),
            Color.White);
    }

    public void Draw(SpriteBatch batch, int amount, float depth)
    {
        batch.Draw(Target,
             new Rectangle(Rect.X, Rect.Y, Rect.Width, amount),
             new Rectangle(0, 0, Target.Width, amount), Color.White, 0, Vector2.Zero, SpriteEffects.None,
             depth);
    }
}

public interface IExecutable : IDraw
{
    public void Load(SpriteBatch batch, GraphicsDevice device);
    public void Start();
    public void Update(float dt);
}

/// <summary>
/// Représente un objet qui sera appelé dans la boucle principale.
/// </summary>
public abstract class AdvancedExecutable : IExecutable, IDrawUI, IDisposable
{
    private static ushort currentId = 0;
    public readonly ushort exe_id;

    protected AdvancedExecutable()
    {
        this.exe_id = currentId;
        currentId++;
    }

    protected AdvancedExecutable(ushort id)
    {
        this.exe_id = id;
    }

    public override int GetHashCode()
    {
        return exe_id;
    }

    public override bool Equals(object obj)
    {
        if (obj is AdvancedExecutable)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        return false;
    }

    public abstract void Start();

    public abstract void Load(SpriteBatch batch, GraphicsDevice gd);
    public virtual void AfterStart() { }

    public virtual void BeforeUpdate(float dt) { }
    public virtual void AfterUpdate(float dt) { }
    public virtual void WhenPaused(float dt) { }


    public virtual void DrawBackground(SpriteBatch batch) { }
    public virtual void DrawShaderBefore(SpriteBatch batch, GraphicsDevice device) { }
    public virtual void DrawShader(SpriteBatch batch, GraphicsDevice device) { }
    public virtual void AfterDraw(SpriteBatch batch) { }
    public virtual void DrawUI(SpriteBatch batch, int width, int height) { }
    public virtual void DrawMain(SpriteBatch batch) { }


    public abstract void Update(float dt);

    public abstract void Draw(SpriteBatch batch);

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

    public override void Load(SpriteBatch batch, GraphicsDevice gd)
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

    public override void Draw(SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].Draw(batch);
    }

    public override void DrawBackground(SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawBackground(batch);
    }

    public override void DrawShader(SpriteBatch batch, GraphicsDevice device)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawShader(batch, device);
    }

    public override void AfterDraw(SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].AfterDraw(batch);
    }

    public override void DrawUI(SpriteBatch batch, int width, int height)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawUI(batch, width, height);
    }

    public override void DrawMain(SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Count; i++)
            exes[i].DrawMain(batch);
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

    public override void Draw(SpriteBatch batch)
    {

    }

    public override void Load(SpriteBatch batch, GraphicsDevice gd)
    {

    }

    public override void Start()
    {

    }

    public override void Update(float dt)
    {
    }
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


    public override void Load(SpriteBatch batch, GraphicsDevice device)
    {
        foreach (Clone c in clones)
        {
            c.Load(batch, device);
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

    public override void Draw(SpriteBatch batch)
    {
        foreach (Clone c in clones)
        {
            c.Draw(batch);
        }
    }

    public override void AfterDraw(SpriteBatch batch)
    {
        foreach (Clone c in clones)
        {
            c.AfterDraw(batch);
        }
    }

    public override void DrawUI(SpriteBatch batch, int w, int h)
    {
        foreach (Clone c in clones)
        {
            c.DrawUI(batch, w, h);
        }
    }

    public override void DrawBackground(SpriteBatch batch)
    {
        foreach (Clone c in clones)
        {
            c.DrawBackground(batch);
        }
    }

    public override void DrawMain(SpriteBatch batch)
    {
        foreach (Clone c in clones)
        {
            c.DrawMain(batch);
        }
    }

    public override void DrawShader(SpriteBatch batch, GraphicsDevice device)
    {
        foreach (Clone c in clones)
        {
            c.DrawShader(batch, device);
        }
    }

    public override void DrawShaderBefore(SpriteBatch batch, GraphicsDevice device)
    {
        foreach (Clone c in clones)
        {
            c.DrawShaderBefore(batch, device);
        }
    }

    public override void WhenPaused(float dt)
    {
        foreach (Clone c in clones)
        {
            c.WhenPaused(dt);
        }
    }
}