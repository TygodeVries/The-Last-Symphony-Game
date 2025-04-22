using System.Collections;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    LineRenderer r;

    public void SetPoints(Vector3 start, Vector3 end)
    {
        r = GetComponent<LineRenderer>();
        r.SetPosition(0, start + new Vector3(0, 0.5f, 0));
        r.SetPosition(1, end + new Vector3(0, 0.5f, 0));

        StartCoroutine(DeleteLater());
    }

    IEnumerator DeleteLater()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
