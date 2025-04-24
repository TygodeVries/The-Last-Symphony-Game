using UnityEditor.Search;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject ProjectilePrefab;

    public void DrawShot(Shot shot)
    {
        Vector3 start = shot.shooter.transform.position;
        Vector3 end = shot.target.transform.position;

        if(Vector3.Distance(shot.ProjectileEarlyDeathPoint, Vector3.zero) > 0.01f)
        {
            end = shot.ProjectileEarlyDeathPoint;
        }

        Debug.LogWarning(end);
        ProjectileInstance instance = GameObject.Instantiate(ProjectilePrefab).GetComponent<ProjectileInstance>();
        
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
