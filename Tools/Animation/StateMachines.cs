using FriteCollection2;

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
    private float dt;
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

    public void Update()
    {
        if (active)
        {
            State newState = current.Update(dt);
            dt += Time.FrameTime;
            if (newState is not null)
                ForceState(newState);
        }
    }

    public void ResetTimer()
    {
        dt = 0f;
    }

    public void ForceState(in State state)
    {
        dt = 0f;
        state.Start();
        State newState = state.Update(0);
        if (newState is not null)
        {
            ForceState(newState);
            current = newState;
        }
        else
            current = state;
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
            current.Draw();
        }
    }
}