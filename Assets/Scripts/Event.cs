using UnityEngine;
using UnityEngine.Events;

public class Event : MonoBehaviour
{ 
    public void Call()
    {
        e.Invoke();
    }

    public UnityEvent e;
}
