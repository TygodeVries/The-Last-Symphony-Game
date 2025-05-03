using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LoadLater : MonoBehaviour
{
    public float Amount;
    public UnityEvent unityEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DoLater());
    }

    IEnumerator DoLater()
    {
        yield return new WaitForSeconds(Amount);
        unityEvent.Invoke();
    }
}
