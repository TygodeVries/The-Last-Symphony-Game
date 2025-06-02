using UnityEngine;

public class Protection : MonoBehaviour
{
    public bool xPositive;
    public bool zPositive;

    public bool xNegative;
    public bool zNegative;

    public void OnDrawGizmosSelected()
    {
        if (xPositive)
            Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0) + transform.position, new Vector3(0.1f, 1, 1f));

        if (xNegative)
            Gizmos.DrawCube(new Vector3(-0.5f, 0.5f, 0) + transform.position, new Vector3(0.1f, 1, 1f));

        if (zPositive)
            Gizmos.DrawCube(new Vector3(0, 0.5f, 0.5f) + transform.position, new Vector3(1f, 1, 0.1f));

        if (zNegative)
            Gizmos.DrawCube(new Vector3(0, 0.5f, -0.5f) + transform.position, new Vector3(1f, 1, 0.1f));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.1f);

        if (xPositive)
            Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0) + transform.position, new Vector3(0.1f, 1, 1f));

        if (xNegative)
            Gizmos.DrawCube(new Vector3(-0.5f, 0.5f, 0) + transform.position, new Vector3(0.1f, 1, 1f));

        if (zPositive)
            Gizmos.DrawCube(new Vector3(0, 0.5f, 0.5f) + transform.position, new Vector3(1f, 1, 0.1f));

        if (zNegative)
            Gizmos.DrawCube(new Vector3(0, 0.5f, -0.5f) + transform.position, new Vector3(1f, 1, 0.1f));
    }
}
