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
        yield return new WaitUntil(() =>
        {
            return txt.text.Length < 1;
        });

        Debug.Log("Showing: " + text);
        GetComponent<Animator>().SetTrigger("In");
        txt.text = text;
        yield return new WaitForSeconds(time);

        GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.2f);
        txt.text = "";
    }

    TMP_Text txt;
    private void Start()
    {
        instance = this;
        txt = gameObject.GetComponent<TMP_Text>();
        txt.text = "";
    }
}
