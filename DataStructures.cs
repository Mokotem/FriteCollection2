using FriteCollection2.Entity;
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
        GameManager.Draw.Batch.Draw(Target, Rect, Color.White);
    }

    public void Draw(float depth)
    {
        GameManager.Draw.Batch.Draw(Target, Rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, depth);
    }
}

public static class Input
{
    private static KeyboardState _kbstate, _prekbstate;
    private static MouseState _mouseState, _pMouseState;

    public static KeyboardState KB => _kbstate;
    public static KeyboardState KBP => _prekbstate;

    public static class Mouse
    {
        public static MouseState State => _mouseState;
        public static MouseState StateP => _pMouseState;

        private static Bounds _origin = Bounds.TopLeft;
        public static Bounds GridOrigin
        {
            get => _origin;
            set
            {
                _origin = value;
            }
        }

        public static Vector2 GetVectorPosition(in Environment envi)
        {
            Vector2 offset = new Vector2(-envi.Rect.X, -envi.Rect.Y);
            offset += new Vector2(State.Position.X, State.Position.Y);
            return new Vector2(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height)) + envi.Bounds[(int)_origin];
        }
        public static Vector2 GetVectorPosition(in Environment envi, Vector2 mouse)
        {
            Vector2 offset = new Vector2(-envi.Rect.X, -envi.Rect.Y);
            offset += mouse;
            return new Vector2(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height));
        }

        public static Point GetPointPosition(in Environment envi)
        {
            Point offset = new Point(-envi.Rect.X, -envi.Rect.Y);
            offset += new Point(State.Position.X, State.Position.Y);
            return new Point(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height));
        }


        public static Point GetPointPosition(in Environment envi, Point mouse)
        {
            Point offset = new Point(-envi.Rect.X, -envi.Rect.Y);
            offset += mouse;
            return new Point(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height));
        }
    }

    public static void SetStates(KeyboardState kbs, MouseState mss)
    {
        _prekbstate = _kbstate;
        _pMouseState = _mouseState;
        _kbstate = kbs;
        _mouseState = mss;
    }
}

/// <summary>
/// Données temporels.
/// </summary>
public static class Time
{
    private static float _sp = 1f;
    internal static float _frameTime = 1f / GameManager.Fps;

    /// <summary>
    /// 'vitesse' du temps. 0f arrêt, 1f normal, 2f rapide
    /// </summary>
    public static float SpaceTime
    {
        get => _sp;
        set
        {
            _sp = value;
        }
    }

    /// <summary>
    /// Temps théorique d'une frame. Est toujours constant, et est dépendant de 'SpaceTime'.
    /// </summary>
    /// <example>~ 0.01f</example>
    public static float FrameTime
    {
        get
        {
            return _frameTime * _sp;
        }
    }

    /// <summary>
    /// Temps théorique d'une frame. Est toujours constant, et indépendant 'SpaceTime'.
    /// </summary>
    /// <example>~ 0.01f</example>
    public static float FixedFrameTime
    {
        get
        {
            return _frameTime;
        }
    }

    public static void UpdateGameTime(in GameTime gt)
    {
        timer += _frameTime * _sp;
        dtf = (float)gt.ElapsedGameTime.TotalMilliseconds / 1000f;
    }

    public static void Reset()
    {
        timer = 0f;
    }

    private static float timer;
    private static float dtf;

    /// <summary>
    /// Temps écoulé depuis le début de la scène.
    /// </summary>
    public static float Timer => timer;

    /// <summary>
    /// Temps écoulé entre les deux dernières frames.
    /// </summary>
    /// <example>~ 0.01f</example>
    public static float Delta => dtf * _sp;
}

/// <summary>
/// Représente un objet qui sera appelé dans la boucle principale.
/// </summary>
public abstract class Executable : IDisposable
{
    public virtual bool Active { get; }
    /// <summary>
    /// Appelé au début de la scène
    /// </summary>
    public virtual void Start() { }
    public virtual void AfterStart() { }

    /// <summary>
    /// s'execute à chaque frame. Avant tout 'Update'.
    /// </summary>
    public virtual void BeforeUpdate() { }
    /// <summary>
    /// s'execute à chaque frame.
    /// </summary>
    public virtual void Update() { }
    /// <summary>
    /// s'execute à chaque frame. Après tout 'Update'.
    /// </summary>
    public virtual void AfterUpdate() { }


    public virtual void DrawBackground() { }
    public virtual void BeforeDraw() { }
    public virtual void DrawShader(SpriteBatch batch) { }
    public virtual void AfterDraw() { }

    public virtual void WhenPaused() { }

    /// <summary>
    /// Méthode supplémentaire pour dessiner, utilisé de base pour l'interface.
    /// </summary>
    public virtual void DrawUI() { }
    public virtual void DrawMain() { }

    /// <summary>
    /// Ici, disposer toutes les ressources.
    /// </summary>
    public virtual void Dispose() { }
    /// <summary>
    /// Ici, charger toutes les ressources. (est appelé avant 'Start')
    /// </summary>
    public virtual void Load() { }


    private short layer = 0;

    /// <summary>
    /// Les executables sont appelés dans l'ordre croissant.
    /// </summary>
    public short Layer
    {
        set
        {
            if (GameManager.Instance.CurrentExecutables.Contains(this) == true)
            {
                List<Executable> _currentScripts = GameManager.Instance.CurrentExecutables;
                _currentScripts.Remove(this);

                layer = value;
                if (_currentScripts.Count == 0)
                    _currentScripts = new List<Executable>() { this };
                else
                {
                    int i = 0;
                    while (i < _currentScripts.Count && _currentScripts[i].layer < this.layer)
                    {
                        i++;
                    }

                    _currentScripts.Insert(i, this);
                }
            }
        }

        get
        {
            return layer;
        }
    }
}



public abstract class Script : Executable
{
    /// <summary>
    /// Est appelé quand la taille de la fenêtre change.
    /// </summary>
    public virtual void OnWindowResize() { }

    /// <param name="scene">Scène à laquelle appartient le script. Doit être convertible en 'int'</param>
    /// <param name="active">si 'false' le script ne sera pas appelé.</param>
    public Script(object scene, bool active = true)
    {
        _attributedScenes = (int)scene;
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
        foreach (Executable s in GameManager.Instance.CurrentExecutables)
        {
            if (s.GetType().Name == typeof(T).Name)
                return s as T;
        }
        throw new Exception("'" + typeof(T).Name + "' scripte n'existe pas dans cette scène.");
    }

    private bool _active;
    public override bool Active => _active;
    private int _attributedScenes;

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
        GameManager.Instance.CurrentExecutables.Remove(this);
    }

    /// <summary>
    /// La scène à laquelle appartient le script.
    /// </summary>
    internal int AttributedScenes
    {
        get
        {
            return _attributedScenes;
        }
    }
}

/// <summary>
/// Un script qui peut être créé plusieurs fois dans la scène.
/// </summary>
public abstract class Clone : Executable
{
    private bool isdestroyed = false;

    /// <summary>
    /// Détruit tout les Clones dans la scène.
    /// </summary>
    /// <param name="exepts">sauf les clones de ce type.</param>
    public static void DestroyAll(params Type[] exepts)
    {
        foreach (Executable exe in GameManager.Instance.CurrentExecutables.ToArray())
        {
            if (exe is Clone && !(exepts.Contains(exe.GetType().BaseType) || exepts.Contains(exe.GetType())))
            {
                (exe as Clone).Destroy();
            }
        }
    }

    public static void DestroyAll(GameManager.Discriminent<Clone> discr)
    {
        foreach (Executable exe in GameManager.Instance.CurrentExecutables.ToArray())
        {
            if (exe is Clone && discr(exe as Clone))
            {
                (exe as Clone).Destroy();
            }
        }
    }

    /// <summary>
    /// Est appelé quand le clone est retiré de la scène.
    /// Ne pas disposer les ressources ici mais dans 'Dispose'.
    /// </summary>
    protected virtual void OnDestroy()
    {

    }

    private bool _active;
    public override bool Active => _active;

    /// <summary>
    /// Vérifie si le Clone a été retiré de la scène.
    /// </summary>
    public bool IsDestroyed => isdestroyed;


    public Clone()
    {
        _active = true;
        GameManager.Instance.CurrentExecutables.Add(this);
    }

    /// <summary>
    /// Retire le Clone de la scène.
    /// </summary>
    public void Destroy()
    {
        this._active = false;
        this.OnDestroy();
        this.Dispose();
        GameManager.Instance.CurrentExecutables.Remove(this);
        isdestroyed = true;
    }
}

