using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    public abstract void OnNotify(object typeOfStatistic, NotificationType notificationType);
}

public abstract class Subject : MonoBehaviour
{
    private List<Observer> _observers = new List<Observer>();

    public void AddObserver(Observer observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(object value, NotificationType notificationType)
    {
        foreach(var observer in _observers)
        {
            observer.OnNotify(value, notificationType);
        }
    }
}
