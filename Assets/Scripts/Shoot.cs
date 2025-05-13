
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        CameraSystem.SetTarget(null);
        FindAnyObjectByType<Battle>().StartCoroutine(FindAnyObjectByType<Battle>().StartEnemyTurn());

        gameObject.SetActive(false);

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
            ShootHitRay(shooter.transform.position + eyeLevel, target.transform.position + eyeLevel, target, shooter.gameObject) 
            *
            LossOverDistance(Vector3.Distance(shooter.transform.position, target.transform.position));
    }

    public Vector3 ProjectileEarlyDeathPoint;
    private float ShootHitRay(Vector3 origin, Vector3 shootTarget, GameObject gameobjectTarget, GameObject shooter)
    {
        string debugMsg = "";

        Vector3 direction = (shootTarget - origin).normalized;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, 300);

        float baseRate = 1.0f;

        debugMsg += "This is a shot debug message! Good luck.";
        debugMsg += $"Base rate is {baseRate}";
        foreach (RaycastHit hit in hits.OrderBy(h => h.distance))
        {
            if(hit.collider.transform.root == gameobjectTarget.transform.root)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    File.WriteAllText("debug.txt", debugMsg);
                    Process.Start("notepad.exe", "debug.txt");
                }

                return baseRate;
            }

            ShotChangeEffector effector = hit.collider.GetComponent<ShotChangeEffector>();
            if (effector == null)
            {
                debugMsg += $"Passed {hit.collider}, No effector. Range remains {baseRate}\n";
                continue;
            }

            // We are close enough, so we can pass ;)
            if (Vector3.Distance(origin, effector.transform.position) < effector.DistanceBypass)
            {
                debugMsg += $"Passed {hit.collider}, But we are close. Range remains {baseRate}\n";
                continue;
            }

            baseRate *= effector.PassChance;
            debugMsg += $"Passed {hit.collider}, Range is now {baseRate}.\n";

            UnityEngine.Debug.DrawLine(hit.point, origin, Color.red, 10);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            File.WriteAllText("debug.txt", debugMsg);
            Process.Start("notepad.exe", "debug.txt");
        }

        return baseRate;
    }

    private static float LossOverDistance(float distance)
    {
        return 1.0f;

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