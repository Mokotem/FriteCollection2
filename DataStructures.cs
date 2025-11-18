using FriteCollection2.Entity;
using FriteCollection2.Entity.Hitboxs;
using FriteCollection2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Un module 
/// </summary>
namespace FriteCollection2;

/// <summary>
/// Jeu peux dessiner
/// </summary>
public interface IDraw
{
    /// <summary>
    /// Dessine l'objet.
    /// </summary>
    public void Draw();
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

    public void Draw()
    {
        GraphicDistributor.Batch.Draw(Target, Rect, Color.White);
    }

    public void Draw(float depth)
    {
        GraphicDistributor.Batch.Draw(Target, Rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, depth);
    }

    public void Draw(int amount)
    {
        GraphicDistributor.Batch.Draw(Target,
            new Rectangle(Rect.X, Rect.Y, Rect.Width, amount),
            new Rectangle(0, 0, Target.Width, amount),
            Color.White);
    }

    public void Draw(int amount, float depth)
    {
        GraphicDistributor.Batch.Draw(Target,
             new Rectangle(Rect.X, Rect.Y, Rect.Width, amount),
             new Rectangle(0, 0, Target.Width, amount), Color.White, 0, Vector2.Zero, SpriteEffects.None,
             depth);
    }
}

/// <summary>
/// Représente un objet qui sera appelé dans la boucle principale.
/// </summary>
public interface IExecutable : IDisposable
{
    public bool Active { get; }
    /// <summary>
    /// Appelé au début de la scène
    /// </summary>
    public void Start() { }
    public void AfterStart() { }

    /// <summary>
    /// s'execute à chaque frame. Avant tout 'Update'.
    /// </summary>
    public void BeforeUpdate(float dt) { }
    /// <summary>
    /// s'execute à chaque frame.
    /// </summary>
    public void Update(float dt) { }
    /// <summary>
    /// s'execute à chaque frame. Après tout 'Update'.
    /// </summary>
    public void AfterUpdate(float dt) { }


    public void DrawBackground() { }
    public void BeforeDraw() { }
    public void DrawShader(SpriteBatch batch) { }
    public void AfterDraw() { }

    public void WhenPaused() { }

    /// <summary>
    /// Méthode supplémentaire pour dessiner, utilisé de base pour l'interface.
    /// </summary>
    public void DrawUI() { }
    public void DrawMain() { }

    /// <summary>
    /// Ici, charger toutes les ressources. (est appelé avant 'Start')
    /// </summary>
    public void Load() { }
}



public abstract class Script : IExecutable
{
    /// <param name="scene">Scène à laquelle appartient le script. Doit être convertible en 'int'</param>
    /// <param name="active">si 'false' le script ne sera pas appelé.</param>
    public Script(object scene, bool active = true)
    {
        _attributedScenes = scene;
        _active = active;
    }

    /// <summary>
    /// Donne la référence d'un script dans la scène.
    /// </summary>
    /// <typeparam name="T">Le script</typeparam>
    /// <returns></returns>
    /// <exception cref="Exception">le scripte n'existe pas dans la scène.</exception>
    public static T GetScript<T>() where T : Script
    {
        foreach (IExecutable s in GraphicDistributor.Executables)
        {
            if (s.GetType().Name.Equals(typeof(T).Name))
                return (T)s;
        }
        throw new Exception("'" + typeof(T).Name + "' scripte n'existe pas dans cette scène.");
    }

    private bool _active;
    public bool Active => _active;
    private object _attributedScenes;

    /// <summary>
    /// Active ou désactive le script.
    /// </summary>
    public void SetActive(bool value)
    {
        this._active = value;
    }

    /// <summary>
    /// Détruit le script pour toujours dans la scène.
    /// </summary>
    public void Destroy()
    {
        this.Dispose();
        GraphicDistributor.Executables.Remove(this);
    }

    public virtual void Dispose()
    {

    }

    /// <summary>
    /// La scène à laquelle appartient le script.
    /// </summary>
    internal object AttributedScenes
    {
        get
        {
            return _attributedScenes;
        }
    }
}