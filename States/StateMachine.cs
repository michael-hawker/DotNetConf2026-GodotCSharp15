namespace GodotNet11.States;

public class StateMachine<T>
{
    public T CurrentState { get; private set; }
    
    public StateMachine(T initialState)
    {
        CurrentState = initialState;
    }

    /// <summary>
    /// Go to the next state, return what the state was.
    /// </summary>
    /// <param name="nextState"></param>
    /// <returns></returns>
    public T GoToState(T nextState)
    {
        T current = CurrentState;
        CurrentState = nextState;
        return current;
    }
}