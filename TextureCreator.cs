using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection2;

public static class TextureCreator
{
    public static Texture2D Create(GraphicsDevice device, int w, int h, Color color)
    {
        Texture2D texture = new Texture2D(device, w, h);

        Color[] data = new Color[w * h];
        for (int pixel = 0; pixel < w * h; pixel++)
        {
            data[pixel] = color;
        }

        texture.SetData(data);

        return texture;
    }

    public static Texture2D Create(GraphicsDevice device, int w, int h)
    {
        return Create(device, w, h, Color.White);
    }

    public static Texture2D CreateNotFound(GraphicsDevice device, int w, int h)
    {
        Texture2D texture = new Texture2D(device, w, h);

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

    public static Texture2D CreateCircle(GraphicsDevice device, int width, Color color)
    {
        Texture2D tex = new Texture2D(device, width, width);
        Color[] data = new Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            for (int j = 0; j < width; j += 1)
            {
                if (float.Sqrt(float.Pow(i - (width / 2), 2) + float.Pow(j - (width / 2), 2)) <= width / 2)
                {
                    data[i + (j * width)] = color;
                }
                else
                {
                    data[i + (j * width)] = Color.Transparent;
                }
            }
        }
        tex.SetData<Color>(data);
        return tex;
    }

    public static Texture2D CreateCircle(GraphicsDevice device, int width)
    {
        return CreateCircle(device, width, Color.White);
    }

    public static Texture2D CreateCircle(GraphicsDevice device, int width, int holeSize, Color color)
    {
        Texture2D tex = new Texture2D(device, width, width);
        Color[] data = new Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            float a = float.Pow(i - (width / 2), 2);
            for (int j = 0; j < width; j += 1)
            {
                float d = float.Sqrt(a + float.Pow(j - (width / 2), 2));
                if (d <= width / 2 && d >= holeSize / 2)
                {
                    data[i + (j * width)] = color;
                }
                else
                {
                    data[i + (j * width)] = Color.Transparent;
                }
            }
        }
        tex.SetData<Color>(data);
        return tex;
    }

    public static Texture2D CreateCircle(GraphicsDevice device, int width, int holeSize)
    {
        return CreateCircle(device, width, holeSize, Color.White);
    }

    public static Texture2D CreateFrame(GraphicsDevice device, int width, int height, ushort borderSize, Color color)
    {
        Texture2D tex = new Texture2D(device, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < width; i += 1)
        {
            for (int j = 0; j < height; j += 1)
            {
                if (i < borderSize || j < borderSize || width - i < borderSize + 1 || height - j < borderSize + 1)
                {
                    data[i + (j * width)] = color;
                }
                else
                {
                    data[i + (j * width)] = Color.Transparent;
                }
            }
        }
        tex.SetData<Color>(data);
        return tex;
    }

    public static Texture2D CreateFrame(GraphicsDevice device, int width, int height, ushort borderSize)
    {
        return CreateFrame(device, width, height, borderSize, Color.White);
    }
}
