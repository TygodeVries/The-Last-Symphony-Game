using UnityEngine;

public class VanishOrSpanish : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        a = true;
    }

    bool a;

    float b = 1;
    public void Update()
    {
        if(a)
        {
            b += Time.deltaTime * 3;
            transform.Rotate(0,  b * b, 0);
            transform.Translate(0, Time.deltaTime, 0);
            if(b > 7)
                Destroy(gameObject);
        }
    }
}
