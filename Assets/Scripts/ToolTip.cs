using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    private static TMP_Text text;

    private static Animator animatior;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animatior = GetComponent<Animator>();
        text = GetComponent<TMP_Text>();
    }

    public static void Set(string text)
    {
        if (ToolTip.text.text != text)
        {
            animatior.SetTrigger("Appear");
        }
        ToolTip.text.text = text;
    }

}
