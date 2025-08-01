using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.SpriteSheet;

public class SpriteSheet : IDisposable
{
    private readonly Texture2D[,] textures;

    public SpriteSheet(Texture2D texture, int width, int height)
    {
        int wCount = texture.Width / width;
        int hCount = texture.Height / height;

        textures = new Texture2D[wCount, hCount];

        for (int x = 0; x < wCount; x++)
        {
            for (int y = 0; y < hCount; y++)
            {
                Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice,
                    width, height);
                Color[] data = new Color[width * height];

                Rectangle rect = new Rectangle(x * width, y * height, width, height);
                texture.GetData(0, rect, data, 0, width * height);

                tex.SetData(data);
                textures[x, y] = tex;
            }
        }
    }

    public virtual Texture2D this[int x, int y]
    {
        get
        {
            return textures[x, y];
        }
    }

    public virtual Texture2D this[Point p]
    {
        get
        {
            return textures[p.X, p.Y];
        }
    }

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
        _tex = GameManager.Instance.CreateNotFoundTexture(width, height);
    }

    public override Texture2D this[int x, int y] => _tex;

    public override Texture2D this[Point p] => _tex;

    public override void Dispose()
    {
        base.Dispose();
        _tex.Dispose();
    }
}