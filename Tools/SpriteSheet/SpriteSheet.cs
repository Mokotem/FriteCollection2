using System;
using FriteCollection2.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.SpriteSheet;

public class SpriteSheet : IDisposable
{
    private readonly Texture2D[,] textures;

    public readonly int Width;
    public readonly int Height;
    public int Count => Width * Height;

    public SpriteSheet(Texture2D texture, int width, int height)
    {
        this.Width = texture.Width / width;
        this.Height = texture.Height / height;

        textures = new Texture2D[this.Width, this.Height];

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                Texture2D tex = new Texture2D(GraphicDistributor.Device,
                    width, height);
                Color[] data = new Color[width * height];

                Rectangle rect = new Rectangle(x * width, y * height, width, height);
                texture.GetData(0, rect, data, 0, width * height);

                tex.SetData(data);
                textures[x, y] = tex;
            }
        }
    }

    public virtual Texture2D this[int x, int y] => textures[x, y];
    public virtual Texture2D this[int i] => textures[i % Width, i / Width];
    public virtual Texture2D this[short i] => textures[i % Width, i / Width];
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

    public NotFoundSpriteSheet(int width, int height)
        : base(Entity.Renderer.DefaultTexture, 2, 2)
    {
        _tex = Renderer.CreateNotFoundTexture(GraphicDistributor.Device, width, height);
    }

    public override Texture2D this[int x, int y] => _tex;

    public override Texture2D this[Point p] => _tex;

    public override void Dispose()
    {
        base.Dispose();
        _tex.Dispose();
    }
}