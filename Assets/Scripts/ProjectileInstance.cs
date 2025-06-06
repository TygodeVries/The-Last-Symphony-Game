using System.Collections;
using Unity.VisualScripting;
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

        float speed = 5f;
        float distance = Vector3.Distance(start, end);
        float duration = distance / speed;
        float elapsed = 0f;

        yield return new WaitForSeconds(0.4f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }


}
