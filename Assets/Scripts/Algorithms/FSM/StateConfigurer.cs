using System.Collections.Generic;


namespace StateConfig
{
    public class StateConfigurer<T>
    {
        MyState<T> instance;
        Dictionary<T, Transition<T>> transitions = new Dictionary<T, Transition<T>>();

        public StateConfigurer(MyState<T> state)
        {
            instance = state;
        }

        public StateConfigurer<T> SetTransition(T input, MyState<T> target)
        {
            transitions.Add(input, new Transition<T>(input, target));
            return this;
        }

        public void Done()
        {
            instance.Configure(transitions);
        }
    }

    public static class StateConfigurer
    {
        public static StateConfigurer<T> Create<T>(MyState<T> state)
        {
            return new StateConfigurer<T>(state);
        }
    }
}
