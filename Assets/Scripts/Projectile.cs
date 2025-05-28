using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject ProjectilePrefab;

    public void DrawShot(Shot shot, bool targetIsHit)
    {
        if (shot.shooter == null || shot.target == null)
            return;

        Vector3 start = shot.shooter.transform.position;
        Vector3 end = shot.target.transform.position;

        if(!targetIsHit)
        {
            end = shot.ProjectileEarlyDeathPoint;
        }

        Debug.Log("Target is: " + shot.target.gameObject.name);

        Debug.LogWarning(end);
        ProjectileInstance instance = GameObject.Instantiate(ProjectilePrefab, start, Quaternion.identity).GetComponent<ProjectileInstance>();
            
        instance.SetPoints(start, end);
    }

    public void Update()
    {   
        if(instance == null)
        {
            Debug.LogWarning("Hot reloading is not supported!");
        }
    }
    public void Start()
    {
        instance = this;
    }
    public static Projectile instance;
}
