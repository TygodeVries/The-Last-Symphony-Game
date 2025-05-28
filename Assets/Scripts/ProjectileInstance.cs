using System.Collections;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    LineRenderer r;

    public void SetPoints(Vector3 start, Vector3 end)
    {
        StartCoroutine(Move(start, end));
    }

    IEnumerator Move(Vector3 start, Vector3 end)
    {
        Debug.DrawLine(start, end, Color.blue, 100);
        CameraSystem.SetTarget(this.transform);
        float elapsed = 0f;
        yield return new WaitForSeconds(0.4f);

        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / 1f);
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = end;

        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }
}
