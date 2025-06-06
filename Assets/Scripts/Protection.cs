using UnityEngine;

public class Protection : MonoBehaviour
{
    public bool xPositive;
    public bool xPositiveHalfWall;

    public bool zPositive;
    public bool zPositiveHalfWall;

    public bool xNegative;
    public bool xNegativeHalfWall;

    public bool zNegative;
    public bool zNegativeHalfWall;

    public void OnDrawGizmosSelected()
    {
        Vector3[] offsets = new Vector3[] { new Vector3(0.5f, 0, 0), new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 0.5f), new Vector3(0, 0, -0.5f) };
        bool[] bools = new bool[] { xPositive, xNegative, zPositive, zNegative };
        bool[] types = new bool[] { xPositiveHalfWall, xNegativeHalfWall, zPositiveHalfWall, zNegativeHalfWall };

        for (int i = 0; i < types.Length; i++)
        {
            bool isActive = bools[i];
            bool isHalfWall = types[i];

            if (isActive)
            {
                if (!isHalfWall)
                {
                    Gizmos.DrawCube(offsets[i] + transform.position, new Vector3(0.1f, 1, 0.1f));
                }
                else
                {
                    Gizmos.DrawCube(offsets[i] + transform.position, new Vector3(0.1f, 0.5f, 0.1f));
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.6f);

        Vector3[] offsets = new Vector3[] { new Vector3(0.5f, 0, 0), new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 0.5f), new Vector3(0, 0, -0.5f) };
        bool[] bools = new bool[] { xPositive, xNegative, zPositive, zNegative };
        bool[] types = new bool[] { xPositiveHalfWall, xNegativeHalfWall, zPositiveHalfWall, zNegativeHalfWall };

        for (int i = 0; i < types.Length; i++)
        {
            bool isActive = bools[i];
            bool isHalfWall = types[i];

            if (isActive)
            {
                if (!isHalfWall)
                {
                    Gizmos.DrawCube(offsets[i] + transform.position, new Vector3(0.1f, 1, 0.1f));
                }
                else
                {
                    Gizmos.DrawCube(offsets[i] + transform.position, new Vector3(0.1f, 0.5f, 0.1f));
                }
            }
        }
    }
}
