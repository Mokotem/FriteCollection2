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
    private static FriteModel.MonoGame _nstnc;

    /// <summary>
    /// Donner la référence de l'instance MonoGame.
    /// </summary>
    /// <param name="_instance"></param>
    public static void SetGameInstance(in FriteModel.MonoGame _instance)
    {
        _nstnc = _instance;
    }

    /// <summary>
    /// Ferme le jeu.
    /// </summary>
    public static void Quit()
    {
        _nstnc.Exit();
    }

    /// <summary>
    /// Plein écran.
    /// </summary>
    public static bool FullScreen
    {
        get => _nstnc.FullScreen;
        set => _nstnc.FullScreen = value;
    }

    /// <summary>
    /// Environements de dessins principales.
    /// </summary>
    /// <example>généralement, [0] jeu principale, [1] interface.</example>
    public static Environment[] Environments => _nstnc.Environments;

    /// <summary>
    /// Instance MonoGame.
    /// </summary>
    public static FriteModel.MonoGame Instance => _nstnc;

    /// <summary>
    /// Mets à jour les environements, lors de changement de la taille d'écran.
    /// </summary>
    public static void UpdateEnvironments()
    {
        _nstnc.UpdateEnvironments();
    }
    
    private static Settings _settings;
    
    private static ushort _fps;
    internal static ushort Fps
    {
        get => _fps;
        set
        {
            _fps = value;
            Instance.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / value);
        }
    }

    public static GraphicsDevice GraphicsDevice => _nstnc.GraphicsDevice;

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
            Instance.UpdateScriptToScene();
        }
    }

    public delegate Texture2D TextureCreator(GraphicsDevice graphic, in SpriteBatch batch);
    public delegate void TextureModifier(GraphicsDevice graphic, in SpriteBatch batch, Texture2D texture);
    public delegate void TextureMaker(GraphicsDevice graphic, in SpriteBatch batch);

    /// <summary>
    /// Appelle une fonction qui créé une Texture2D.
    /// </summary>
    /// <param name="method">void Texture2D Exemple(GraphicsDevice graphic, in SpriteBatch batch)</param>
    /// <returns></returns>
    public static Texture2D MakeTextureCreator(TextureCreator method)
    {
        return method(Instance.GraphicsDevice, in Instance.SpriteBatch);
    }

    /// <summary>
    /// Appelle une fonction qui dessine des trucs.
    /// </summary>
    /// <param name="method">void Exemple(GraphicsDevice graphic, in SpriteBatch batch, Texture2D texture)</param>
    public static void MakeTexture(TextureModifier method, Texture2D texture)
    {
        method(Instance.GraphicsDevice, in Instance.SpriteBatch, texture);
    }

    /// <summary>
    /// Appelle une fonction qui dessine des trucs.
    /// </summary>
    /// <param name="method">void Exemple(GraphicsDevice graphic, in SpriteBatch batch)</param>
    public static void MakeTexture(TextureMaker method)
    {
        method(Instance.GraphicsDevice, in Instance.SpriteBatch);
    }
}

/// <summary>
/// Caméra, affecte le déssin finale.
/// </summary>
public static class Camera
{
    public static Vector2 Position;
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
