using FriteCollection2;
using FriteCollection2.Entity;
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
    private protected SpriteBatch SpriteBatch;

    public SpriteBatch Batch => SpriteBatch;
    public GraphicsDeviceManager Graphics => graphics;
    public GraphicsDevice Device => GraphicsDevice;

    internal List<FriteCollection2.UI.ButtonCore> _buttons = new List<FriteCollection2.UI.ButtonCore>();
    public virtual event ScreenUpdate OnScreenUpdate;
    protected bool changingScene = false;
    private protected readonly Type[] _childTypes;
    protected Settings S => GameManager.Settings;

    public delegate void ScreenUpdate(bool full);
    
    protected bool Loading { get; private set; }
    protected string LoadingText { get; private set; }

    public virtual FriteCollection2.Environment[] Environments
    {
        get;
    }

    protected internal virtual void TogglePause(bool value)
    {

    }

    public MonoGame(Type[] childTypes)
    {
        _childTypes = childTypes;
        IsMouseVisible = true;
        graphics = new GraphicsDeviceManager(this);
        Window.AllowUserResizing = S.AllowUserResizeing;
        GameManager.SetGameInstance(this);
    }

    protected List<Executable> _currentExecutables = new List<Executable>();
    internal List<Executable> CurrentExecutables => _currentExecutables;
    private GameTime lastGametime;

    protected override void Initialize()
    {
        Loading = true;
        LoadingText = "Loading game ...";
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Window.Title = GameManager.Settings.WindowName;

        graphics.PreferredBackBufferWidth = GameManager.Settings.WindowWidth;
        graphics.PreferredBackBufferHeight = GameManager.Settings.WindowHeight;
        FullScreen = GameManager.Settings.FullScreen;

        Color[] data = new Color[4]
        {
            new Color(0, 0, 0), new Color(255, 0, 255), new Color(0, 0, 0), new Color(255, 0, 255),
        };
        Renderer._notFoundTexture = new Texture2D(GraphicsDevice, 2, 2);
        Renderer._notFoundTexture.SetData<Color>(data);

        base.Initialize();

        GameManager.Fps = GameManager.Settings.FPS;

        UpdateEnvironments();
        lastGametime = new GameTime(new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0));
        UpdateScriptToScene();
        Loading = false;
    }

    public virtual void UpdateScriptToScene()
    {
        Loading = true;
        LoadingText = "Changing scene ...";
        GameManager.SetGameInstance(this);

        changingScene = true;
        FriteCollection2.Entity.Hitboxs.Hitbox.ClearAllLayers();
        MediaPlayer.Stop();
        _buttons.Clear();

        LoadingText = "Unloading ...";
        foreach (Executable exe in CurrentExecutables.ToArray())
        {
            if (exe is Clone)
                (exe as Clone).Destroy();
            else
                exe.Dispose();
        }

        CurrentExecutables.Clear();

        LoadingText = "Loading scripts ...";
        Camera.Position = Point.Zero;
        Screen.backGround = new Color(0.1f, 0.2f, 0.3f);

        foreach (Type type in _childTypes)
        {
            Script instance = (Script)Activator.CreateInstance(type);
            if (instance.AttributedScenes == GameManager.CurrentScene && instance.Active)
            {
                CurrentExecutables.Add(instance);
            }
            else instance = null;
        }

        Time.Reset(lastGametime);
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

        LoadingText = "Grabage collector ...";
        GC.Collect();

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
        lastGametime = gameTime;

        if (this.IsActive)
        {
            mstate = Mouse.GetState();
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                mcp = new Point(mstate.Position.X, mstate.Position.Y);
            }
        }

        Input.SetStates(Keyboard.GetState(), mstate);
        foreach (FriteCollection2.UI.ButtonCore but in _buttons)
        {
            but.Update();
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

public class MonoGameDefault : MonoGame
{
    public MonoGameDefault(Type[] types) : base(types)
    {

    }

    private static FriteCollection2.Environment game, ui;
    internal float aspectRatio;
    public override event ScreenUpdate OnScreenUpdate;

    public override FriteCollection2.Environment[] Environments => new FriteCollection2.Environment[2] { game, ui };

    public override void UpdateEnvironments()
    {
        Point user;
        if (this._fullScreen)
            user = new Point(
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        else
            user = new Point(S.WindowWidth, S.WindowHeight);

        float ratio = float.Min(user.X / (float)S.GameFixeWidth, user.Y / (float)S.GameFixeHeight);
        Vector2 scale = new Vector2(S.GameFixeWidth * ratio, S.GameFixeHeight * ratio);
        Rectangle rect = new Rectangle(
        (int)((user.X - scale.X) / 2f), (int)((user.Y - scale.Y) / 2f),
            (int)scale.X, (int)scale.Y);

        game = new FriteCollection2.Environment(rect,
            new RenderTarget2D(base.GraphicsDevice, S.GameFixeWidth, S.GameFixeHeight));
        ui = new FriteCollection2.Environment(rect,
            new RenderTarget2D(base.GraphicsDevice, rect.Width, rect.Height));

        if (OnScreenUpdate is not null)
            OnScreenUpdate(_fullScreen);
    }

    public override bool FullScreen
    {
        get => this._fullScreen;

        set
        {
            this._fullScreen = value;
            UpdateEnvironments();
            Space.SetDefaultEnvironment(in game);
            FriteCollection2.UI.UI.Rectangle.SetDefaultEnvironment(in ui);

            base.FullScreen = value;
        }
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (!changingScene)
        {
            foreach (Executable script in base._currentExecutables.ToArray())
            {
                if (script.Active)
                    script.BeforeUpdate();
            }
            foreach (Executable script in base._currentExecutables.ToArray())
            {
                if (script.Active)
                    script.Update();
            }
            foreach (Executable script in base._currentExecutables.ToArray())
            {
                if (script.Active)
                    script.AfterUpdate();
            }
        }
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.Target);
        GraphicsDevice.Clear(Screen.backGround);

        SpriteBatch.Begin(sortMode: SpriteSortMode.BackToFront,
            blendState: BlendState.AlphaBlend);

        foreach (Executable exe in _currentExecutables)
        {
            exe.BeforeDraw();
        }

        SpriteBatch.End();

        GraphicsDevice.SetRenderTarget(ui.Target);
        GraphicsDevice.Clear(Color.Transparent);

        SpriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);

        foreach (Executable exe in _currentExecutables)
        {
            exe.DrawUI();
        }

        SpriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        SpriteBatch.Begin();
        game.Draw();
        ui.Draw();
        SpriteBatch.End();
    }
}

public class MonoGameDefaultPixel : FriteModel.MonoGame
{
    public MonoGameDefaultPixel(System.Type[] types) : base(types)
    {

    }

    public override event ScreenUpdate OnScreenUpdate;

    private static FriteCollection2.Environment game;
    internal float aspectRatio;

    public override FriteCollection2.Environment[] Environments => new FriteCollection2.Environment[1] { game };

    public override void UpdateEnvironments()
    {
        Point user;
        if (this._fullScreen)
            user = new Point(
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        else
            user = new Point(S.WindowWidth, S.WindowHeight);

        Point target = new Point(S.GameFixeWidth, S.GameFixeHeight);
        int n = 2;
        while (target.X * n <= user.X && target.Y * n <= user.Y)
            n += 1;
        n += -1;
        Point scale = new Point(target.X * n, target.Y * n);
        game = new FriteCollection2.Environment(new Rectangle(
        (user.X - scale.X) / 2, (user.Y - scale.Y) / 2,
            scale.X, scale.Y),
            new RenderTarget2D(base.GraphicsDevice, S.GameFixeWidth, S.GameFixeHeight));
    }

    public override bool FullScreen
    {
        get => this._fullScreen;

        set
        {
            this._fullScreen = value;
            UpdateEnvironments();
            Space.SetDefaultEnvironment(in game);
            FriteCollection2.UI.UI.Rectangle.SetDefaultEnvironment(in game);

            base.FullScreen = value;
        }
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (!changingScene)
        {
            foreach (Executable script in base._currentExecutables.ToArray())
            {
                script.BeforeUpdate();
            }
            foreach (Executable script in base._currentExecutables.ToArray())
            {
                script.Update();
            }
            foreach (Executable script in base._currentExecutables.ToArray())
            {
                script.AfterUpdate();
            }
        }
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.Target);
        GraphicsDevice.Clear(Screen.backGround);

        SpriteBatch.Begin(sortMode: SpriteSortMode.BackToFront,
                        samplerState: SamplerState.PointClamp,
            blendState: BlendState.AlphaBlend);

        foreach (Executable exe in _currentExecutables)
        {
            exe.BeforeDraw();
        }

        SpriteBatch.End();

        SpriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);

        foreach (Executable exe in _currentExecutables)
        {
            exe.DrawUI();
        }

        SpriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        game.Draw();
        SpriteBatch.End();
    }
}