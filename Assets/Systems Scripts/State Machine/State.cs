/// <summary>
/// Base class for a state. T is the "context": the object the state controls
/// (a player, an enemy, a game manager...). The state machine never needs to know what T is.
/// </summary>
public abstract class State<T>
{
    public virtual void Enter(T context) { }
    public virtual void Tick(T context) { }
    public virtual void Exit(T context) { }
}