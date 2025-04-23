using System.Collections;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    [SerializeField] private float ShowFor = 4;

    private IEnumerator showText()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(ShowFor);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        StartCoroutine(showText());
    }
}
