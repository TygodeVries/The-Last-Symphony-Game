using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    public UnityEvent TriggerEnter;

    public void OnTriggerEnter(Collider other)
    {
        TriggerEnter.Invoke();
    }
}
