using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.Animation;

public class State
{
    public static readonly State Empty = new State()
    {
        Update = (float dt) => { return null; }
    };

    public delegate State UpdateState(float timer);
    public System.Action Start { get; init; }
    public required UpdateState Update { get; init; }
    public IDraw.DrawFunction Draw { get; init; }
    public IDraw.DrawFunction DrawAdditive { get; init; }

    public State()
    {
        Start = () => { };
        Draw = (SpriteBatch batch) => { };
        DrawAdditive = (SpriteBatch batch) => { };
    }
}

public class StateMachine : IDraw
{
    private float timer, delta;
    private State current;
    public bool active;
    private readonly bool reset;
    private readonly State start;

    public StateMachine(State start, bool resetOnChange = true)
    {
        this.reset = resetOnChange;
        this.start = start;
        active = false;
        delta = 0f;
        timer = 0f;
    }

    public void Restart()
    {
        active = true;
        ForceState(start);
    }

    public void Restart(State state)
    {
        active = true;
        ForceState(state);
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

    public float UpdateDelta(float dt)
    {
        if (active)
        {
            timer += dt;
            State newState = current.Update(timer);
            if (newState is not null)
                ForceState(newState);
            return timer;
        }
        return -1f;
    }

    public void ResetTimer()
    {
        delta = 0f;
        timer = 0f;
    }

    public void ResetTimer(float tim)
    {
        delta = tim;
    }

    public void ForceState(State state)
    {
        if (reset)
        {
            ResetTimer();
        }
        current = state;
        state.Start();
    }

    public void Draw(SpriteBatch batch)
    {
        if (active)
        {
            current.Draw(batch);
        }
    }

    public void DrawAdditive(SpriteBatch batch)
    {
        if (active)
        {
            current.DrawAdditive(batch);
        }
    }
}