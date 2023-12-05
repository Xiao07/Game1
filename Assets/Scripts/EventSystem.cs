using System;
using System.Collections.Generic;

// 例如，一个简单的事件系统
public class EventSystem
{
    private Dictionary<string, Action> eventListeners = new Dictionary<string, Action>();

    public void AddListener(string eventName, Action listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = listener;
        }
        else
        {
            eventListeners[eventName] += listener;
        }
    }

    public void RemoveListener(string eventName, Action listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] -= listener;
        }
    }

    public void TriggerEvent(string eventName)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName]?.Invoke();
        }
    }
}
