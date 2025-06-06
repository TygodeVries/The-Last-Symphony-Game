using UnityEngine;

public class VanishOrSpanish : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
