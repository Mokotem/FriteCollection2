using System;
using FriteCollection2.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.SpriteSheet;

public class SpriteSheet : IDisposable
{
    private readonly Texture2D[,] textures;

    public readonly int CountX;
    public readonly int CountY;
    public readonly int Count;

    public readonly int Width;
    public readonly int Height;

    public SpriteSheet(Texture2D texture, int width, int height, GraphicsDevice device)
    {
        this.Width = width;
        this.Height = height;
        this.CountX = texture.Width / width;
        this.CountY = texture.Height / height;
        this.Count = CountX * CountY;

        textures = new Texture2D[this.CountX, this.CountY];

        for (int x = 0; x < this.CountX; x++)
        {
            for (int y = 0; y < this.CountY; y++)
            {
                Texture2D tex = new Texture2D(device, width, height);
                Color[] data = new Color[width * height];

                Rectangle rect = new Rectangle(x * width, y * height, width, height);
                texture.GetData(0, rect, data, 0, width * height);

                tex.SetData(data);
                textures[x, y] = tex;
            }
        }
    }

    public virtual Texture2D this[int x, int y] => textures[x, y];
    public virtual Texture2D this[int i] => textures[i % CountX, i / CountX];
    public virtual Texture2D this[short i] => textures[i % CountX, i / CountX];
    public virtual Texture2D this[Point p] => textures[p.X, p.Y];

    public virtual void Dispose()
    {
        foreach (Texture2D tex in textures)
            tex.Dispose();
    }
}

public class NotFoundSpriteSheet : SpriteSheet
{
    private readonly Texture2D _tex;

    public NotFoundSpriteSheet(int width, int height, GraphicsDevice device)
        : base(Entity.Renderer.DefaultTexture, 2, 2, device)
    {
        _tex = Renderer.CreateNotFoundTexture(device, width, height);
    }

    public override Texture2D this[int x, int y] => _tex;

    public override Texture2D this[Point p] => _tex;

    public override void Dispose()
    {
        base.Dispose();
        _tex.Dispose();
    }
}