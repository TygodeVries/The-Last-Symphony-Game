using System.Globalization;
using TMPro;
using UnityEngine;

public class Wave : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    float time;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        TMP_Text TextComponent = GetComponent<TMP_Text>();
        TMP_TextInfo textInfo = TextComponent.textInfo;

        for (int m = 0; m < textInfo.meshInfo.Length; m++)
        {

            for (int i = 0; i < textInfo.meshInfo[m].vertices.Length; i++)
            {
                Vector3 vert = textInfo.meshInfo[m].vertices[i];

                vert.y += Mathf.Cos(time * 4 + vert.x) / 300;

                textInfo.meshInfo[m].vertices[i] = vert;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }
}
