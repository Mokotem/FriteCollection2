using FriteCollection2.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection2;

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


public interface IHaveDrawingTools
{
    public SpriteBatch Batch { get; }
    public GraphicsDeviceManager Graphics { get; }
    public GraphicsDevice Device { get; }
    public void Exit();
}

public class Settings
{
    public int WindowWidth = 800;
    public int WindowHeight = 600;
    public int GameFixeWidth = 800;
    public int GameFixeHeight = 600;
    public int ReferenceWidth = 800;
    public int ReferenceHeight = 600;
    public bool FullScreen = false;
    public bool PixelArtDrawing = false;
    public string WindowName = "";
    public byte UICoef = 1;
    public byte StartScene = 0;
    public byte FPS = 60;
    public bool AllowUserResizeing = false;
}

public static class GameManager
{
    public delegate bool Discriminent<T>(T truc);

    public static void DisposeInstance()
    {
        _mg = null;
        if (_nstnc is not null)
        {
            _nstnc.Dispose();
            _nstnc = null;
        }
    }

    private static FriteModel.MonoGameDefault _nstnc;
    private static IHaveDrawingTools _mg;

    internal static FriteModel.MonoGameDefault Instance => _nstnc;

    public static void TogglePause(bool value)
    {
        _nstnc.TogglePause(value);
    }

    /// <summary>
    /// Donner la référence de l'instance MonoGame.
    /// </summary>
    /// <param name="_instance"></param>
    public static void SetGameInstance(in FriteModel.MonoGameDefault _instance)
    {
        _nstnc = _instance;
        _mg = _nstnc;
    }

    public static void SetGameInstance(in IHaveDrawingTools _instance)
    {
        _mg = _instance;
    }

    public static void DebugScriptOrder()
    {
        string result = "order: ";
        for (ushort i = 0; i < _nstnc.CurrentExecutables.Count; ++i)
        {
            result += _nstnc.CurrentExecutables[i].GetType().Name + ", ";
        }
        result.Remove(result.Length - 3);
        System.Diagnostics.Debug.WriteLine(result);
    }

    /// <summary>
    /// Ferme le jeu.
    /// </summary>
    public static void Quit()
    {
        _mg.Exit();
    }

    /// <summary>
    /// Plein écran.
    /// </summary>
    public static bool FullScreen
    {
        get => _nstnc.FullScreen;
        set => _nstnc.FullScreen = value;
    }

    public static Texture2D CreateTexture(int w, int h, Color color)
    {
        Texture2D texture = new Texture2D(_mg.Device, w, h);

        Color[] data = new Color[w * h];
        for (int pixel = 0; pixel < w * h; pixel++)
        {
            data[pixel] = color;
        }

        texture.SetData(data);

        return texture;
    }

    /// <summary>
    /// Environements de dessins principales.
    /// </summary>
    /// <example>généralement, [0] jeu principale, [1] interface.</example>
    public static Environment[] Environments => _nstnc.Environments;

    public static Texture2D CreateNotFoundTexture(int w, int h)
    {
        Texture2D texture = new Texture2D(_mg.Device, w, h);

        float ws2 = w / 2 - 0.5f;
        float hs2 = h / 2 - 0.5f;

        Color[] data = new Color[w * h];
        for (int pixel = 0; pixel < w * h; pixel++)
        {
            int x = pixel % w;
            int y = pixel / w;
            if ((x - ws2) * (y - hs2) < 0)
            {
                data[pixel] = new Color(255, 0, 255);
            }
            else
            {
                data[pixel] = new Color(0, 0, 0);
            }
        }

        texture.SetData(data);

        return texture;
    }

    /// <summary>
    /// Instance MonoGame.
    /// </summary>
    public static IHaveDrawingTools Draw => _mg;

    /// <summary>
    /// Mets à jour les environements, lors de changement de la taille d'écran.
    /// </summary>
    public static void UpdateEnvironments()
    {
        _nstnc.UpdateEnvironments();
    }
    
    private static Settings _settings;
    
    private static ushort _fps;
    public static ushort Fps
    {
        get => _fps;
        set
        {
            _fps = value;
            _nstnc.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / value);
        }
    }

    /// <summary>
    /// Parramètres du projet.
    /// </summary>
    public static Settings Settings => _settings;

    private static SpriteFont _font;

    /// <summary>
    /// Police principale du projet.
    /// </summary>
    public static SpriteFont Font => _font;

    /// <summary>
    /// Mettre la police principale du projet.
    /// </summary>
    /// <param name="font"></param>
    public static void SetFont(SpriteFont font)
    {
        _font = font;
    }

    /// <summary>
    /// Donner les paramètres du projet.
    /// ne sert à rien après avoir appelé 'game.Run()'.
    /// </summary>
    /// <param name="settings"></param>
    public static void SetSettings(Settings settings)
    {
        _settings = settings;
        _currentScene = settings.StartScene;
        Time._frameTime = 1f / settings.FPS;
    }

    /// <summary>
    /// Afficher dans la fenêtre 'sortie' de visual studio.
    /// </summary>
    /// <param name="listText"></param>
    public static void Print(params object[] listText)
    {
        string finalTxt = "";
        foreach (object s in listText) { finalTxt += s.ToString() + "  "; }
        System.Diagnostics.Debug.WriteLine(finalTxt);
    }

    private static byte _currentScene;
    /// <summary>
    /// Scène en cour d'execution.
    /// </summary>
    public static byte CurrentScene
    {
        get
        {
            return _currentScene;
        }

        set
        {
            _currentScene = value;
            _nstnc.UpdateScriptToScene();
        }
    }

    public delegate Texture2D TextureCreator(GraphicsDevice graphic, SpriteBatch batch);
    public delegate void TextureModifier(GraphicsDevice graphic, SpriteBatch batch, Texture2D texture);
    public delegate void TextureMaker(GraphicsDevice graphic, SpriteBatch batch);

    /// <summary>
    /// Appelle une fonction qui créé une Texture2D.
    /// </summary>
    /// <param name="method">void Texture2D Exemple(GraphicsDevice graphic, in SpriteBatch batch)</param>
    /// <returns></returns>
    public static Texture2D MakeTextureCreator(TextureCreator method)
    {
        return method(Draw.Device, Draw.Batch);
    }

    /// <summary>
    /// Appelle une fonction qui dessine des trucs.
    /// </summary>
    /// <param name="method">void Exemple(GraphicsDevice graphic, in SpriteBatch batch, Texture2D texture)</param>
    public static void MakeTexture(TextureModifier method, Texture2D texture)
    {
        method(Draw.Device, Draw.Batch, texture);
    }

    /// <summary>
    /// Appelle une fonction qui dessine des trucs.
    /// </summary>
    /// <param name="method">void Exemple(GraphicsDevice graphic, in SpriteBatch batch)</param>
    public static void MakeTexture(TextureMaker method)
    {
        method(Draw.Device, Draw.Batch);
    }
}

/// <summary>
/// Caméra, affecte le déssin finale.
/// </summary>
public static class Camera
{
    public static Point Position;
    public static Point RoundPosition;
}

/// <summary>
/// Données sur la fenêtre du projet.
/// </summary>
public static class Screen
{
    /// <summary>
    /// Couleur d'arrière plan.
    /// </summary>
    public static Color backGround = new(0.1f, 0.2f, 0.3f);

    internal static int rww, rwh;
    public static int WindowWidth => rww;
    public static int WindowHeight => rwh;

    /// <summary>
    /// Résolution du jeu.
    /// </summary>
    public static readonly int widht = GameManager.Settings.GameFixeWidth, height = GameManager.Settings.GameFixeHeight;
}
