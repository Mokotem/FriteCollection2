using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.Animation;

public abstract class AnimationBase
{
    protected delegate float DurationFunc();
    protected DurationFunc delay;
    protected short currentKey;

    protected float start { get; private set; }

    public AnimationBase(float start)
    {
        this.start = start;
        currentKey = -1;
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

    public Animation(KeyFrame[] frames, float[] durations, float startTime = 0f) : base(startTime)
    {
        if (frames.Length < 1 || durations.Length != frames.Length)
            throw new System.Exception("Frame count should be the same as durations");

        this.frames = frames;
        Restart();
        delay = () => durations[currentKey];
    }

    public Animation(KeyFrame[] frames, float delay, float startTime = 0f) : base(startTime)
    {
        if (frames.Length < 1)
            throw new System.Exception("Frame count should be the same as durations");

        this.frames = frames;
        Restart();
        this.delay = () => delay;
    }

    public override bool Done => currentKey >= frames.Length;

    public override void Animate(float timer)
    {
        while (currentKey < frames.Length
            && timer >= start + b)
        {
            a = b;
            currentKey += 1;
            if (!Done)
            {
                if (delay() <= 0)
                {
                    frames[currentKey](0);
                }
                else
                {
                    b += delay();
                }
            }
        }
        if (!Done && currentKey >= 0)
        {
            frames[currentKey]((timer - a - start) / delay());
        }

        if (Loop && Done)
        {
            Restart(timer);
        }
    }
}

public class AnimationSheet : AnimationBase
{
    public delegate void SetTexture(int index);
    private readonly int frameCount;
    private readonly SetTexture OnTexture;

    public override bool Done => currentKey >= frameCount;

    public AnimationSheet(int frameCount, SetTexture OnTexture, float[] durations, float startTime = 0f) : base(startTime)
    {
        this.OnTexture = OnTexture;
        this.frameCount = frameCount;
        Restart();
        delay = () => durations[currentKey];
    }

    public AnimationSheet(int frameCount, SetTexture OnTexture, float delay, float startTime = 0f) : base(startTime)
    {
        this.OnTexture = OnTexture;
        this.frameCount = frameCount;
        Restart();
        this.delay = () => delay;
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
                OnTexture(currentKey);
                b += delay();
            }
        }

        if (Loop && Done)
        {
            Restart(timer);
        }
    }
}