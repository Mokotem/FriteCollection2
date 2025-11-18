using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection2.Tools.Animation;

public class State
{
    public delegate State UpdateState(float timer);
    public System.Action Start { get; init; }
    public UpdateState Update { get; init; }
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
    private float timer;
    private State current;
    public bool active;
    private readonly State start;

    public StateMachine(in State start)
    {
        this.start = start;
        active = false;
    }

    public void Restart()
    {
        active = true;
        ForceState(start);
    }

    public void Update(float dt)
    {
        if (active)
        {
            State newState = current.Update(this.timer);
            timer += dt;
            if (newState is not null)
                ForceState(newState);
        }
    }

    public void ResetTimer()
    {
        timer = 0f;
    }

    public void ForceState(in State state)
    {
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