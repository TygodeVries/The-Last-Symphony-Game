using UnityEngine;

public class SceneMovement : MonoBehaviour
{
    public float TeleportHome = 0;
    private Vector3 start;
    private void Start()
    {
        start = transform.position;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, TeleportHome));
    }

    public void Update()
    {
        transform.Translate(0, 0, Time.deltaTime * -8);

        if(transform.position.z < TeleportHome)
        {
            transform.Translate(0, 0, 40);
        }
    }
}
