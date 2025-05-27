using UnityEngine;

public class AcceptTerms : MonoBehaviour
{
    public void Accept()
    {
        Destroy(transform.root.gameObject);
    }
}
