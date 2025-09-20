using FriteCollection2;
using FriteCollection2.Entity;
using FriteCollection2.Entity.Hitboxs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace FriteModel;

public abstract class MonoGame : Game, IHaveDrawingTools
{
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch SpriteBatch;

    protected object scene;
    public Color background;

    public SpriteBatch Batch => SpriteBatch;
    public GraphicsDeviceManager Graphics => graphics;
    public GraphicsDevice Device => GraphicsDevice;

    internal List<FriteCollection2.UI.ButtonCore> _buttons = new List<FriteCollection2.UI.ButtonCore>();
    protected bool changingScene = false;
    protected Type[] _childTypes;

    public delegate void ScreenUpdate(bool full);
    
    protected bool Loading { get; set; }
    protected string LoadingText { get; private set; }

    public virtual FriteCollection2.Environment[] Environments
    {
        get;
    }

    public MonoGame()
    {
        IsMouseVisible = true;
        graphics = new GraphicsDeviceManager(this);
        Window.AllowUserResizing = false;
    }

    protected List<Executable> _currentExecutables = new List<Executable>();
    internal List<Executable> CurrentExecutables => _currentExecutables;

    protected override void Initialize()
    {
        Loading = true;
        LoadingText = "Loading game ...";
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        base.Initialize();
    }

    protected void Apply(int rw, int rh)
    {
        if (scene is null)
            throw new System.NullReferenceException();

        graphics.PreferredBackBufferWidth = rw;
        graphics.PreferredBackBufferHeight = rh;

        Color[] data = new Color[4]
        {
            new Color(0, 0, 0), new Color(255, 0, 255), new Color(0, 0, 0), new Color(255, 0, 255),
        };
        Renderer._notFoundTexture = new Texture2D(GraphicsDevice, 2, 2);
        Renderer._notFoundTexture.SetData<Color>(data);

        Renderer.SetDefaultTexture(Renderer.CreateTexture(GraphicsDevice, 1, 1, Color.White));

        graphics.ApplyChanges();
    }

    public virtual void UpdateScriptToScene(Executable[] adds)
    {
        Loading = true;
        LoadingText = "Changing scene ...";

        changingScene = true;
        MediaPlayer.Stop();
        _buttons.Clear();
        Hitbox.ClearAllLayers();
        LoadingText = "Unloading ...";
        foreach (Script exe in CurrentExecutables.ToArray())
        {
            exe.Dispose();
        }

        CurrentExecutables.Clear();

        LoadingText = "Loading scripts ...";

        foreach (Type type in _childTypes)
        {
            Script instance = (Script)Activator.CreateInstance(type);
            if (instance.AttributedScenes.Equals(scene) && instance.Active)
            {
                CurrentExecutables.Add(instance);
            }
            else instance = null;
        }

        CurrentExecutables.AddRange(adds);

        Time.Reset();
        Time.SpaceTime = 1f;

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            LoadingText = "Loading " + script.GetType().Name + " ...";
            script.Load();
        }

        LoadingText = "Finishing ...";

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.Start();
        }
        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.AfterStart();
        }

        LoadingText = "Done!";
        Loading = false;
    }

    protected bool _fullScreen;
    protected DisplayMode Display => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

    public virtual bool FullScreen
    {
        get
        {
            return _fullScreen;
        }
        set
        {
            _fullScreen = value;

            graphics.HardwareModeSwitch = !value;
            graphics.IsFullScreen = value;
            graphics.ApplyChanges();
        }
    }

    public virtual void UpdateEnvironments()
    {

    }

    public static void SetUserCursor(Texture2D tex, int ox, int oy)
    {
        Mouse.SetCursor(MouseCursor.FromTexture2D(tex, ox, oy));
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {

    }

    protected MouseState mstate;
    private Point mcp;
    internal Point MouseClickedPosition => mcp;
    protected override void Update(GameTime gameTime)
    {
        Time.UpdateGameTime(in gameTime);

        if (_buttons.Count > 0)
        {
            if (this.IsActive)
            {
                mstate = Mouse.GetState();
                if (mstate.LeftButton == ButtonState.Pressed)
                {
                    mcp = new Point(mstate.Position.X, mstate.Position.Y);
                }
            }

            foreach (FriteCollection2.UI.ButtonCore but in _buttons)
            {
                but.Update(mstate.Position, mstate.LeftButton == ButtonState.Pressed);
            }
        }
        this.OnUpdate(gameTime);
        base.Update(gameTime);
    }

    protected virtual void OnDraw(GameTime gameTime)
    {

    }

    protected override void Draw(GameTime gameTime)
    {
        this.OnDraw(gameTime);
        changingScene = false;
        base.Draw(gameTime);
    }
}