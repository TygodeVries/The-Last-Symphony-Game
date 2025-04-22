using UnityEngine;

public class ShotChangeEffector : MonoBehaviour
{
    public float PassChance = 1.0f;
    public float DistanceBypass = 2f;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DistanceBypass);
    }
}
