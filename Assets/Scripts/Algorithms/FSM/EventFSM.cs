using System;


public class EventFSM<T>
{
    private MyState<T> current;

    public EventFSM(MyState<T> initial)
    {
        current = initial;
        current.Enter(default(T));
    }

    public void SendInput(T input)
    {
        MyState<T> newState;

        if (current.CheckInput(input, out newState))
        {
            current.Exit(input);
            current = newState;
            current.Enter(input);
        }
    }
    public void SendState(MyState<T> newState,T input)
    {
        current.Exit(input);
        current = newState;
        current.Enter(input);
    }

    public MyState<T> Current { get { return current; } }

    public void Update()
    {
        current.Update();
    }

    public void LateUpdate()
    {
        current.LateUpdate();
    }

    public void FixedUpdate()
    {
        current.FixedUpdate();
    }
}
