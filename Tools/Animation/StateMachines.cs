namespace FriteCollection2.Tools.Animation;

public class State
{
    public delegate State UpdateState(float timer);
    public System.Action Start { get; init; }
    public UpdateState Update { get; init; }
    public System.Action Draw { get; init; }
    public System.Action DrawAdditive { get; init; }

    public State()
    {
        Start = () => { };
        Draw = () => { };
        DrawAdditive = () => { };
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

    public void Draw()
    {
        if (active)
        {
            current.Draw();
        }
    }

    public void DrawAdditive()
    {
        if (active)
        {
            current.DrawAdditive();
        }
    }
}