using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private Transform target;
    private Vector3 offset = new Vector3(-1, 2, -1);
    public static void SetTarget(Transform transform)
    {
        FindObjectsByType<CameraSystem>(FindObjectsSortMode.None)[0].target = transform;
    }

    private Vector3 start;
    private void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * 3);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, start, Time.deltaTime * 3);
        }
    }
}
