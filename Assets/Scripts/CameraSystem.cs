using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private Transform target;
    [SerializeField] public Vector3 offset = new Vector3(-1, 2, -1);
    public static void SetTarget(Transform transform)
    {
        Zoom(1);
        FindObjectsByType<CameraSystem>(FindObjectsSortMode.None)[0].target = transform;
    }

    public static void Zoom(float amount)
    {
        FindObjectsByType<CameraSystem>(FindObjectsSortMode.None)[0].goalZoom = amount;
    }

    public float goalZoom = 1;
    public float zoom = 1;
    public Vector3 defaultPos;
    private void Start()
    {
        defaultPos = transform.position;
    }

    private Vector3 zoomedOffset;
    // Update is called once per frame
    void Update()
    {
        zoom = Mathf.Lerp(zoom, goalZoom, Time.deltaTime * 3);

        zoomedOffset = offset;
        zoomedOffset *= zoom;

        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + zoomedOffset, Time.deltaTime * 3);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, defaultPos, Time.deltaTime * 3);
        }
    }
}
