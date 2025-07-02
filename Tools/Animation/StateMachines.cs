using FriteCollection2;

public class State
{
    public delegate State UpdateState(float timer);
    public System.Action Start { get; init; }
    public UpdateState Update { get; init; }
    public System.Action Draw { get; init; }

    public State()
    {
        Start = () => { };
        Draw = () => { };
    }
}

public class StateMachine : IDraw
{
    private float dt;
    private State current;

    public StateMachine(State start)
    {
        dt = 0f;
        current = start;
        start.Start();
        start.Update(0);
    }

    public void Update()
    {
        State newState = current.Update(dt);
        dt += Time.FrameTime;
        if (newState is not null)
            ForceState(newState);
    }

    public void ForceState(State state)
    {
        dt = 0f;
        state.Start();
        current = state;
    }

    public void Draw()
    {
        current.Draw();
    }
}