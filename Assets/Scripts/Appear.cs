using TMPro;
using UnityEngine;

public class Appear : MonoBehaviour
{
    public Vector3 offset;
    public float Size = 4;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + offset, Size);
    }

    public GameObject tracking;

    TMP_Text text;
    public void Start()
    {

        text = GetComponent<TMP_Text>();

        if (tracking == null)
        {
            tracking = FindAnyObjectByType<WalkPlayer>().gameObject;
        }

        text.color = new Color(0, 0, 0, 0);
    }

    private void Update()
    {
        if (Vector3.Distance(tracking.transform.position, transform.position + offset) < Size)
        {
            text.color = Color.Lerp(text.color, text.color = new Color(1, 1, 1, 1), Time.deltaTime * 2);
        }
        else
        {
            text.color = Color.Lerp(text.color, text.color = new Color(0, 0, 0, 0), Time.deltaTime * 2);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position + offset, Size);
    }
}
