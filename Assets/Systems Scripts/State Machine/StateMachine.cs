using System;

/// <summary>
/// A generic, game-independent state machine. Holds the current state,
/// handles switching (Exit old, Enter new), and ticks the active state.
/// </summary>
public class StateMachine<T>
{
    private readonly T context;

    public State<T> Current { get; private set; }

    /// <summary>Raised after every switch: (previous state, new state).</summary>
    public event Action<State<T>, State<T>> StateChanged;

    public StateMachine(T context)
    {
        this.context = context;
    }

    public void ChangeState(State<T> next)
    {
        if (next == null || next == Current) return;

        State<T> previous = Current;
        Current?.Exit(context);
        Current = next;
        Current.Enter(context);

        StateChanged?.Invoke(previous, next);
    }

    public void Tick()
    {
        Current?.Tick(context);
    }
}