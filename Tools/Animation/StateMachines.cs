using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.Animation;

public class State
{
    public delegate State UpdateState(float timer);
    public System.Action Start { get; init; }
    public required UpdateState Update { get; init; }
    public IDraw.DrawFunction Draw { get; init; }
    public IDraw.DrawFunction DrawAdditive { get; init; }

    public State()
    {
        Start = () => { };
        Draw = (in SpriteBatch batch) => { };
        DrawAdditive = (in SpriteBatch batch) => { };
    }
}

public class StateMachine : IDraw
{
    private float timer, delta;
    private State current;
    public bool active;
    private readonly bool deltaMode;
    private readonly State start;

    public StateMachine(in State start)
    {
        this.start = start;
        active = false;
        deltaMode = false;
        delta = 0f;
        timer = 0f;
    }

    public StateMachine(in State start, float delta)
    {
        this.start = start;
        active = false;
        this.delta = delta;
        deltaMode = true;
    }

    public void Restart()
    {
        active = true;
        ForceState(start);
    }

    public void Update(float t)
    {
        if (active)
        {
            timer = t;
            State newState = current.Update(t - delta);
            if (newState is not null)
                ForceState(newState);
        }
    }

    public void UpdateRaw(float t)
    {
        if (active)
        {
            State newState = current.Update(t);
            if (newState is not null)
                ForceState(newState);
        }
    }

    public void Update()
    {
        if (active)
        {
            timer += delta;
            State newState = current.Update(timer);
            if (newState is not null)
                ForceState(newState);
        }
    }

    public void ResetTimer()
    {
        if (deltaMode)
            timer = 0f;
        else
        {
            delta = timer;
        }
    }

    public void ForceState(in State state)
    {
        if (deltaMode)
            timer = 0f;
        current = state;
        state.Start();
    }

    public void Draw(in SpriteBatch batch)
    {
        if (active)
        {
            current.Draw(in batch);
        }
    }

    public void DrawAdditive(in SpriteBatch batch)
    {
        if (active)
        {
            current.DrawAdditive(in batch);
        }
    }
}