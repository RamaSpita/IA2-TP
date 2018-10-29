using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

    public delegate void EventReceiver(params object[] parameterContainer);
    private static Dictionary<EventInputs, EventReceiver> _events;

    public static void SubscribeToEvent(EventInputs events, EventReceiver listener)
    {
        if (_events == null)
            _events = new Dictionary<EventInputs, EventReceiver>();
        if (!_events.ContainsKey(events))
            _events.Add(events, null);
        _events[events] += listener;
    }

    public static void UnsubscribeToEvent(EventInputs events, EventReceiver listener)
    {
        if (_events != null && _events.ContainsKey(events))
            _events[events] -= listener;
    }

    public static void TriggerEvent(EventInputs events)
    {
        TriggerEvent(events, null);
    }

    public static void TriggerEvent(EventInputs events, params object[] parameters)
    {
        if(_events != null && _events.ContainsKey(events) && _events[events] != null)
        {
            _events[events](parameters);
        }
    }
    public static void UnsubsCribeAll()
    {
        _events = new Dictionary<EventInputs, EventReceiver>();
    }
}
