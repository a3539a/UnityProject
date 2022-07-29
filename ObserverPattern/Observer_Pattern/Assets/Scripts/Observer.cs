using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void Notify();
    void AddObserver(Observer observer);
    void RemoveObserver(Observer observer);
}

public class Observer : MonoBehaviour
{
    public virtual void OnNotify() { }
}