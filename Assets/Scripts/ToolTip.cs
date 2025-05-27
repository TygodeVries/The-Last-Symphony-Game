using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    private static TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public static void Set(string text)
    {
        ToolTip.text.text = text;
    }

}
