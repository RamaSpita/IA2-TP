using System;

public class Transition<T>
{
    public event Action<T> OnTransition = delegate { };
    public T Input { get { return input; } }
    public MyState<T> TargetState { get { return targetState; } }

    T input;
    MyState<T> targetState;

    public void OnTransitionExecute(T input)
    {
        OnTransition(input);
    }

    public Transition(T input, MyState<T> targetState)
    {
        this.input = input;
        this.targetState = targetState;
    }
}