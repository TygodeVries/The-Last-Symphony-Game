
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
    [SerializeField] public TMP_Text ChanceText;


    [SerializeField] private float Speed;
    [SerializeField] public GameObject ShootingUI;
    [SerializeField] public Image timingImage;

    bool inTimingState;
    float time;


    public void Update()
    {
        Living target = GetComponent<SelectEnemy>().GetSelected().GetComponent<Living>();
        Shot shot = new Shot(GetComponentInParent<Living>(), target.transform.root.gameObject, new Vector3(0, 0.1f, 0));

        float chance = shot.GetHitChance();

        if (inTimingState)
        {
            time += Time.deltaTime * Speed;

            timingImage.fillAmount = (Mathf.Sin(time) + 1) / 2;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Projectile.instance.DrawShot(shot);

                if (Random.Range(0f, 1f) <= chance)
                {
                    int am = 10;
                    if (timingImage.fillAmount > 0.9f)
                    {
                        am = 20;
                    }
                    target.Damage(am);
                    Notification.SetText($"Did {am} damage!", 1);
                }
                else
                {
                    Notification.SetText("Miss!", 1f);
                }

                ShootingUI.SetActive(false);
                inTimingState = false;
                GetComponent<SelectEnemy>().enabled = true;
                StartCoroutine(DoThing(target.transform));
            }
            return;
        }

        ChanceText.text = "Chance: " + Mathf.RoundToInt(chance * 100) + "%";
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShootingUI.SetActive(true);
            inTimingState = true;
            GetComponent<SelectEnemy>().enabled = false;
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
        return 
            ShootHitRay(shooter.transform.position + eyeLevel, target, shooter.gameObject) 
            *
            LossOverDistance(Vector3.Distance(shooter.transform.position, target.transform.position));
    }

    public Vector3 ProjectileEarlyDeathPoint;
    private float ShootHitRay(Vector3 origin, GameObject target, GameObject shooter)
    {
        Vector3 direction = (target.transform.position - origin).normalized;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, 300);

        float baseRate = 1.0f;
        foreach (RaycastHit hit in hits.OrderBy(h => h.distance))
        {
            ShotChangeEffector effector = hit.collider.GetComponent<ShotChangeEffector>();
            if (effector == null)
                continue;

            // We are close enough, so we can pass ;)
            if (Vector3.Distance(origin, effector.transform.position) < effector.DistanceBypass)
                continue;

            baseRate *= effector.PassChance;
        }

        return baseRate;
    }

    private static float LossOverDistance(float distance)
    {
        float alwaysHit = 3;
        float halfHit = 20;

        if (distance < alwaysHit)
            return 1f;

        if (distance > halfHit)
            return 0.5f;

        float t = (distance - alwaysHit) / halfHit;
        return 1f - t * 1f;
    }

}