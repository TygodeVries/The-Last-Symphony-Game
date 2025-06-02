using System.Collections;
using UnityEngine;

public class DestroyLater : MonoBehaviour
{

    public float After;
    public IEnumerator StartDeletion()
    {
        yield return new WaitForSeconds(After);
        Destroy(gameObject);
    }

    public void StartDelet()
    {
        StartCoroutine(StartDeletion());
    }
}
