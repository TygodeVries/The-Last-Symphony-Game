using UnityEngine;

public class CameraMagnet : MonoBehaviour
{
    public float Size;
    public Vector3 offset;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + offset, Size);
    }
}
