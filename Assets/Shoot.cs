
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public TMP_Text ChanceText;

    public void Update()
    {
        Living target = GetComponent<SelectEnemy>().GetSelected().GetComponent<Living>();
        Shot shot = new Shot(GetComponentInParent<Living>(), target.transform.root.gameObject, new Vector3(0, 0.1f, 0));

        float chance = shot.GetHitChance();

        ChanceText.text = "Chance: " + Mathf.RoundToInt(chance * 100) + "%";
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Projectile.instance.DrawShot(shot);

            if (Random.Range(0f, 1f) <= chance)
            {
                target.Damage(20);
            }
            else
            {
                Notification.SetText("Miss!", 1f);
            }

            StartCoroutine(DoThing(target.transform));
        }
    }

    IEnumerator DoThing(Transform target)
    {
        CameraSystem.SetTarget(target);

        yield return new WaitForSeconds(1);
        FindAnyObjectByType<Battle>().UseAction("Attack Shoot");
        FindAnyObjectByType<Player>().OpenActionGUI();
        gameObject.SetActive(false);

        CameraSystem.SetTarget(null);
    }
}

public class Shot
{
    public Living shooter;
    public GameObject target;
    private Vector3 eyeLevel;

    public Shot(Living shooter, GameObject target, Vector3 eyeLevel)
    {
        this.shooter = shooter;
        this.target = target;
        this.eyeLevel = eyeLevel;
    }

    public float GetHitChance()
    {
        return GetHitChanceI(shooter.transform.position + eyeLevel, target, shooter.gameObject, 1.0f, 0) * LossOverDistance(Vector3.Distance(shooter.transform.position, target.transform.position));
    }

    public Vector3 ProjectileEarlyDeathPoint;
    private float GetHitChanceI(Vector3 origin, GameObject target, GameObject shooter, float baseChance, int age)
    {
        if(age > 100)
        {
            Debug.LogError($"Ran out of age.\n{target.name}, {shooter.name}");
            return 0f;
        }

        try
        {
            Vector3 direction = (target.transform.position - origin);

            Debug.DrawRay(origin + direction * 0.1f, direction, Color.red, 3);

            RaycastHit[] hits = Physics.RaycastAll(origin + (direction.normalized * 0.1f), direction, 300);

            foreach (RaycastHit hit in hits.OrderBy(h => h.distance))
            {
                if (hit.collider.transform.root == shooter.transform.root) // Skip self
                {
                    continue;
                }

                if (hit.collider.transform.root.gameObject == target.transform.root.gameObject)
                {
                    return baseChance;
                }

                ShotChangeEffector effector = hit.collider.transform.root.GetComponent<ShotChangeEffector>();
                if (effector != null)
                {
                    if(effector.PassChance == 0)
                    {
                        ProjectileEarlyDeathPoint = hit.point;
                    }

                    baseChance *= effector.PassChance;
                }

                return GetHitChanceI(origin, target, hit.collider.transform.root.gameObject, baseChance, age + 1);
            }

            return 0f;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Raycast error! " + e + ", age is " + age);
            return 0f;
        }
    }

    private static float LossOverDistance(float distance)
    {
        float alwaysHit = 3;
        float halfHit = 20;


        Debug.Log(distance);
        if (distance < alwaysHit)
            return 1f;

        if (distance > halfHit)
            return 0.5f;

        float t = (distance - alwaysHit) / halfHit;
        return 1f - t * 0.5f;
    }

}