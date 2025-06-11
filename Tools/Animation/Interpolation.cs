using Microsoft.Xna.Framework;

namespace FriteCollection2.Tools.Animation;

public static class Interpolation
{
    public delegate float MoveFloat(float a, float b, float t);
    public delegate Color MoveColor(Color a, Color b, float t);
    public delegate Vector2 MoveVector(Vector2 a, Vector2 b, float t);

    public static float Linear(float a, float b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        return (a * (1 - t)) + (b * t);
    }
    public static float EaseIn(float a, float b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float k = t * t;
        return (a * (1 - k)) + (b * k);
    }

    public static float EaseOut(float a, float b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float k = t * (2 - t);
        return (a * (1 - k)) + (b * k);
    }

    public static float EaseInOut(float a, float b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float s = float.Sin((float.Pi * t) / 2f);
        float k = s * s;
        return (a * (1 - k)) + (b * k);
    }

    public static float BackIn(float a, float b, float t, float k = 4)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float t2 = t * t;
        float t3 = t2 * t;
        float q = -t3 + (2 * t2) + (k * (t3 - t2));
        return (a * (1 - q)) + (b * q);
    }

    public static float BackOut(float a, float b, float t, float k = 4)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float t2 = t * t;
        float t3 = t2 * t;
        float q = t2 - t3 + t + k * (t3 - (2 * t2) + t);
        return (a * (1 - q)) + (b * q);
    }

    public static float BackInOut(float a, float b, float t, float k = 1)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float teta = t * float.Pi;
        float q = float.Pow(float.Sin(teta / 2f), 2) - (k * float.Sin(teta) * float.Sin(2 * teta));
        return (a * (1 - q)) + (b * q);
    }

    public static float Triangle(float a, float b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return a;
        float q = 1 - (2 * float.Abs(t - 0.5f));
        return a * (1 - q) + (b * q);
    }

    private static Color Add(Color c1, Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    public static Color Linear(Color a, Color b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        return Add(a * (1 - t), b * t);
    }
    public static Color EaseIn(Color a, Color b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float k = t * t;
        return Add(a * (1 - k), b * k);
    }

    public static Color EaseOut(Color a, Color b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float k = t * (2 - t);
        return Add(a * (1 - k), b * k);
    }

    public static Color EaseInOut(Color a, Color b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float s = float.Sin((float.Pi * t) / 2f);
        float k = s * s;
        return Add(a * (1 - k), b * k);
    }

    public static Color BackIn(Color a, Color b, float t, float k = 4)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float t2 = t * t;
        float t3 = t2 * t;
        float q = -t3 + (2 * t2) + (k * (t3 - t2));
        return Add(a * (1 - q), b * q);
    }

    public static Color BackOut(Color a, Color b, float t, float k = 4)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float t2 = t * t;
        float t3 = t2 * t;
        float q = t2 - t3 + t + k * (t3 - (2 * t2) + t);
        return Add(a * (1 - q), b * q);
    }

    public static Color BackInOut(Color a, Color b, float t, float k = 1)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float teta = t * float.Pi;
        float q = float.Pow(float.Sin(teta / 2f), 2) - (k * float.Sin(teta) * float.Sin(2 * teta));
        return Add(a * (1 - q), b * q);
    }

    public static Color Triangle(Color a, Color b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return a;
        float q = 1 - (2 * float.Abs(t - 0.5f));
        return Add(a * (1 - q), b * q);
    }

    public static Vector2 Linear(Vector2 a, Vector2 b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        return (a * (1 - t)) + (b * t);
    }
    public static Vector2 EaseIn(Vector2 a, Vector2 b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float k = t * t;
        return (a * (1 - k)) + (b * k);
    }

    public static Vector2 EaseOut(Vector2 a, Vector2 b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float k = t * (2 - t);
        return (a * (1 - k)) + (b * k);
    }

    public static Vector2 EaseInOut(Vector2 a, Vector2 b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float s = float.Sin((float.Pi * t) / 2f);
        float k = s * s;
        return (a * (1 - k)) + (b * k);
    }

    public static Vector2 BackIn(Vector2 a, Vector2 b, float t, float k = 4)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float t2 = t * t;
        float t3 = t2 * t;
        float q = -t3 + (2 * t2) + (k * (t3 - t2));
        return (a * (1 - q)) + (b * q);
    }

    public static Vector2 BackOut(Vector2 a, Vector2 b, float t, float k = 4)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float t2 = t * t;
        float t3 = t2 * t;
        float q = t2 - t3 + t + k * (t3 - (2 * t2) + t);
        return (a * (1 - q)) + (b * q);
    }

    public static Vector2 BackInOut(Vector2 a, Vector2 b, float t, float k = 1)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        float teta = t * float.Pi;
        float q = float.Pow(float.Sin(teta / 2f), 2) - (k * float.Sin(teta) * float.Sin(2 * teta));
        return (a * (1 - q)) + (b * q);
    }

    public static Vector2 Triangle(Vector2 a, Vector2 b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return a;
        float q = 1 - (2 * float.Abs(t - 0.5f));
        return a * (1 - q) + (b * q);
    }
}