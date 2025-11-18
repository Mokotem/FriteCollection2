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
    public void Draw(in SpriteBatch batch, int width, int height);
}

public interface ICreateTextures
{
    public void CreateTexture(in SpriteBatch batch, GraphicsDevice graphics) { }
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

    public Rectangle mRect => new Rectangle(0, 0, Target.Width, Target.Height);
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
    public void Load() { }
    public void Start();
    public void Update(float dt);
}

/// <summary>
/// Représente un objet qui sera appelé dans la boucle principale.
/// </summary>
public interface IAdvancedExecutable : IExecutable, IDrawUI, ICreateTextures, IDisposable
{
    public void BeforeStart() { }
    public void AfterStart() { }

    public void BeforeUpdate(float dt) { }
    public void AfterUpdate(float dt) { }
    public void WhenPaused() { }


    public void DrawBackground(in SpriteBatch batch) { }
    public void BeforeDraw(in SpriteBatch batch) { }
    public void DrawShader(in SpriteBatch batch) { }
    public void AfterDraw(in SpriteBatch batch) { }
    public void DrawUI(in SpriteBatch batch, int width, int height) { }
    public void DrawMain(in SpriteBatch batch) { }
}

public class Scene : IExecutable
{
    private readonly IAdvancedExecutable[] exes;
    public IAdvancedExecutable[] Scripts => exes;

    public Scene(params IAdvancedExecutable[] exes)
    {
        this.exes = exes;
    }

    public void Load()
    {
        for (byte i = 0; i < exes.Length; i++)
            exes[i].Load();
    }

    public void Start()
    {
        byte i;
        for (i = 0; i < exes.Length; i++)
            exes[i].BeforeStart();
        for (i = 0; i < exes.Length; i++)
            exes[i].Start();
        for (i = 0; i < exes.Length; i++)
            exes[i].AfterStart();
    }

    public void Update(float dt)
    {
        byte i;
        for (i = 0; i < exes.Length; i++)
            exes[i].BeforeUpdate(dt);
        for (i = 0; i < exes.Length; i++)
            exes[i].Update(dt);
        for (i = 0; i < exes.Length; i++)
            exes[i].AfterUpdate(dt);
    }

    public void Draw(in SpriteBatch batch)
    {
        for (byte i = 0; i < exes.Length; i++)
            exes[i].Draw(in batch);
    }
}