using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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
    public event EventHandler<ExitingEventArgs> Exiting;
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
    public bool FullScreen = false;
    public bool PixelArtDrawing = false;
    public byte FPS = 60;
}

public static class GraphicDistributor
{
    private static SpriteBatch _batch;
    private static GraphicsDevice _device;
    private static List<Executable> _executables; 

    public static SpriteBatch Batch => _batch;
    public static GraphicsDevice Device => _device;
    public static List<Executable> Executables => _executables;

    private static int _width, _height;
    public static int Width => _width;
    public static int Height => _height;

    private static Settings _sets;

    public static byte FPS => _sets.FPS;

    /// <summary>
    /// Donner la référence de l'instance MonoGame.
    /// </summary>
    /// <param name="_instance"></param>
    public static void SetInstance(in GraphicsDevice grapic, in SpriteBatch batch, in List<Executable> exes, in Settings sets)
    {
        _device = grapic;
        _batch = batch;
        _executables = exes;
        _width = sets.GameFixeWidth;
        _height = sets.GameFixeHeight;
        _sets = sets;
    }

    public static void SetInstance(in GraphicsDevice grapic, in SpriteBatch batch)
    {
        _device = grapic;
        _batch = batch;
    }

    public static void DisposeInstance()
    {
        _device = null;
        _batch = null;
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
}

/// <summary>
/// Caméra, affecte le déssin finale.
/// </summary>
public static class Camera
{
    public static Point Position;
    public static Point RoundPosition;
}
