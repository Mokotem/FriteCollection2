
using static FriteCollection2.Tools.Animation.Animation;

namespace FriteCollection2.Tools.Animation;

public abstract class AnimationBase
{

    protected readonly float[] durations;
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

    public void ChangeDuration(int id, float value)
    {
        this.durations[id] = value;
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

    public abstract void Animate(float dt);
}


public class Animation : AnimationBase
{
    public delegate void KeyFrame(float dt);
    private readonly KeyFrame[] frames;
    public bool IsReversed { get; private set; }

    public Animation(KeyFrame[] frames, float[] durations, float startTime = 0f) : base(startTime, durations)
    {
        if (frames.Length < 1 || durations.Length != frames.Length)
            throw new System.Exception("Frame count should be the same as durations");
        this.frames = frames;
        Restart(startTime);
        Active = true;
        IsReversed = false;
    }

    public Animation(KeyFrame[] frames, float delay, float startTime = 0f) : base(startTime, delay)
    {
        if (frames.Length < 1)
            throw new System.Exception("Frame count should be the same as durations");

        this.frames = frames;
        Restart(startTime);
        Active = true;
        IsReversed = false;
    }

    public override bool Done => Active && currentKey >= frames.Length;

    public bool Active;

    private float cropDelay;
    public void CantAnimateDuring(float timer, float dt)
    {
        currentKey = 0;
        this.cropDelay = timer + dt;
    }

    public void CantAnimateDuring(float timer)
    {
        currentKey = 0;
        CantAnimateDuring(timer, Delay);
    }

    public void ChangeFrame(int id, KeyFrame value)
    {
        this.frames[id] = value;
    }

    public override void Animate(float timer)
    {
        if (Active)
        {
            if (timer < cropDelay)
            {
                frames[0](0);
                return;
            }

            while (currentKey < frames.Length
                && timer >= start + b)
            {
                a = b;
                currentKey += 1;
                if (!Done)
                {
                    if (Delay < 0)
                    {
                        frames[currentKey](0);
                    }
                    else if (Delay == 0)
                    {
                        frames[currentKey](0);
                        return;
                    }
                    else
                    {
                        b += Delay;
                    }
                }
            }

            if (!Done && currentKey >= 0)
            {
                if (IsReversed)
                {
                    frames[currentKey]((Delay - timer + a + start) / Delay);
                }
                else
                {
                    frames[currentKey]((timer - a - start) / Delay);
                }
            }

            if (Loop && Done)
            {
                Restart(timer);
            }
        }
    }

    public void Reverse()
    {
        KeyFrame temp1;
        float temp2;
        for (int i = 0; i < frames.Length / 2; i++)
        {
            temp1 = frames[i];
            temp2 = durations[i];
            frames[i] = frames[frames.Length - i - 1];
            durations[i] = durations[frames.Length - i - 1];
            frames[frames.Length - i - 1] = temp1;
            durations[frames.Length - i - 1] = temp2;
        }
        IsReversed = !IsReversed;
    }
}

public class AnimationSheet : AnimationBase
{
    public delegate void SetTexture(int index);
    private readonly int frameCount;
    protected SetTexture _OnTexture;
    private bool reverse;

    public override bool Done => currentKey >= frameCount;

    public AnimationSheet(int frameCount, SetTexture OnTexture, float[] durations, float startTime = 0f)
        : base(startTime, durations)
    {
        this._OnTexture = OnTexture;
        this.frameCount = frameCount;
        reverse = false;
        Restart();
    }

    public AnimationSheet(int frameCount, SetTexture OnTexture, float delay, float startTime = 0f)
        : base(startTime, delay)
    {
        this._OnTexture = OnTexture;
        this.frameCount = frameCount;
        reverse = false;
        Restart();
    }

    public void Reverse()
    {
        reverse = true;
    }

    public void UnReverse()
    {
        reverse = false;
    }

    public override void Animate(float timer)
    {
        while (currentKey < frameCount
            && timer >= start + b)
        {
            a = b;
            currentKey++;
            if (!Done)
            {
                _OnTexture(reverse ? frameCount - currentKey - 1 : currentKey);
                b += Delay;
            }
        }

        if (Loop && Done)
        {
            Restart(timer);
        }
    }
}