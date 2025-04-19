using System.Collections;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public static void SetText(string text, float time)
    {
        instance.StartCoroutine(instance.e(text, time));
    }

    private static Notification instance;

    private IEnumerator e(string text, float time)
    {
        txt.text = text;
        yield return new WaitForSeconds(time);
        txt.text = "";
    }

    TMP_Text txt;
    private void Start()
    {
        instance = this;
        txt = gameObject.GetComponent<TMP_Text>();
    }
}
