using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class Granade : MonoBehaviour
{
    Camera cam;
    Player player;

    [SerializeField] private float DamageRange;
    [SerializeField] private int DamageAmount;

    public void Start()
    {
        cam = Camera.main;
        player = FindAnyObjectByType<Player>();
        transform.localPosition = new Vector3();
    }

    public void Update()
    {
        Vector3 a = cam.transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        a.y = 0;
        a.Normalize();
        transform.position += a * Time.deltaTime;

        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in tiles)
        {
            if(Vector3.Distance(tile.transform.position, Round(transform.position)) < DamageRange)
            {
                tile.MarkSelected();
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(StartDo());
        }
    }

    public Vector3 Round(Vector3 a)
    {
        return new Vector3(Mathf.Round(a.x), Mathf.Round(a.y), Mathf.Round(a.z));
    }

    public IEnumerator StartDo()
    {
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            float DistanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (DistanceToEnemy < DamageRange)
            {
                CameraSystem.SetTarget(enemy.transform);
                yield return new WaitForSeconds(1);
                enemy.GetComponent<Living>().Damage(DamageAmount);
            }
        }
        yield return new WaitForSeconds(1);
        FindAnyObjectByType<Battle>().UseAction("Attack Granade");
        FindAnyObjectByType<Player>().OpenActionGUI();
        FindAnyObjectByType<Cooldowns>().ResetCooldown();
        CameraSystem.SetTarget(null);

        gameObject.SetActive(false);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, DamageRange);
    }
}
