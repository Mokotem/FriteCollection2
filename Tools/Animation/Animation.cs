using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.Animation;

public abstract class AnimationBase
{

    private readonly float[] durations;
    protected float Delay => durations[currentKey % durations.Length];

    protected short currentKey;

    protected float start { get; private set; }

    public AnimationBase(float start, float delay)
    {
        this.start = start;
        currentKey = -1;
        this.durations = new float[1] { delay };
    }

    public AnimationBase(float start, params float[] durations)
    {
        this.start = start;
        currentKey = -1;
        this.durations = durations;
    }


    protected float a, b;

    public bool Loop = false;

    public virtual bool Done { get; }
    public int CurrentFrame { get; }

    public void Restart(float startTime = 0f)
    {
        this.start = startTime;
        b = 0f;
        a = 0f;
        currentKey = -1;
    }

    public virtual void Animate(float dt) { }
}


public class Animation : AnimationBase
{
    public delegate void KeyFrame(float dt);
    private readonly KeyFrame[] frames;

    public Animation(KeyFrame[] frames, float[] durations, float startTime = 0f) : base(startTime, durations)
    {
        if (frames.Length < 1 || durations.Length != frames.Length)
            throw new System.Exception("Frame count should be the same as durations");
        this.frames = frames;
        Restart(startTime);
        Active = true;
    }

    public Animation(KeyFrame[] frames, float delay, float startTime = 0f) : base(startTime, delay)
    {
        if (frames.Length < 1)
            throw new System.Exception("Frame count should be the same as durations");

        this.frames = frames;
        Restart(startTime);
        Active = true;
    }

    public override bool Done => Active && currentKey >= frames.Length;

    public bool Active;

    public override void Animate(float timer)
    {
        if (Active)
        {
            while (currentKey < frames.Length
                && timer >= start + b)
            {
                a = b;
                currentKey += 1;
                if (!Done)
                {
                    if (Delay <= 0)
                    {
                        frames[currentKey](0);
                    }
                    else
                    {
                        b += Delay;
                    }
                }
            }
            if (!Done && currentKey >= 0)
            {
                frames[currentKey]((timer - a - start) / Delay);
            }

            if (Loop && Done)
            {
                Restart(timer);
            }
        }
    }
}

public class AnimationSheet : AnimationBase
{
    public delegate void SetTexture(int index);
    private readonly int frameCount;
    protected SetTexture _OnTexture;

    public override bool Done => currentKey >= frameCount;

    public AnimationSheet(int frameCount, SetTexture OnTexture, float[] durations, float startTime = 0f)
        : base(startTime, durations)
    {
        this._OnTexture = OnTexture;
        this.frameCount = frameCount;
        Restart();
    }

    public AnimationSheet(int frameCount, SetTexture OnTexture, float delay, float startTime = 0f)
        : base(startTime, delay)
    {
        this._OnTexture = OnTexture;
        this.frameCount = frameCount;
        Restart();
    }

    public AnimationSheet(int frameCount, float[] durations, float startTime = 0f)
    : base(startTime, durations)
    {
        this.frameCount = frameCount;
        Restart();
    }

    public AnimationSheet(int frameCount, float delay, float startTime = 0f)
        : base(startTime, delay)
    {
        this.frameCount = frameCount;
        Restart();
    }

    public override void Animate(float timer)
    {
        while (currentKey < frameCount
            && timer >= start + b)
        {
            a = b;
            currentKey += 1;
            if (!Done)
            {
                _OnTexture(currentKey);
                b += Delay;
            }
        }

        if (Loop && Done)
        {
            Restart(timer);
        }
    }
}