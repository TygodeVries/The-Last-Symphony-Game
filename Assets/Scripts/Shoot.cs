
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

    GameInput.UINavActions uiInput;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;
    }
    public void OnApplicationQuit()
    {
        uiInput.Disable();
    }


    private bool ShotFired;
    public void OnEnable()
    {
        ShotFired = false;
    }


    bool riggedInFavor = false;
    public void Update()
    {
        if (ShotFired)
            return;

        Enemy enemy = GetComponent<SelectEnemy>().GetSelected();
        if (enemy == null)
            return;

        Living target = enemy.GetComponent<Living>();
        Shot shot = new Shot(GetComponentInParent<Living>().gameObject, target.transform.root.gameObject, new Vector3(0, 0.1f, 0));

        float chance = shot.GetHitChance();

        if (uiInput.Back.WasPressedThisFrame())
        {
            FindAnyObjectByType<Player>().OpenActionGUI();
            CameraSystem.SetTarget(null);
            inTimingState = false;
            ShootingUI.SetActive(false);
            GetComponent<SelectEnemy>().enabled = true;
            ToolTip.Set("");
            gameObject.SetActive(false);

            return;
        }

        if (inTimingState)
        {

            time += Time.deltaTime * Speed;

            timingImage.fillAmount = (Mathf.Sin(time) + 1) / 2;

            UnityEngine.Debug.Log(Experiments.disableTimingGameS);
            if (uiInput.Confirm.WasPressedThisFrame() || Experiments.disableTimingGameS)
            {
                int damangeAmount = 0;
                if (Random.Range(0f, 1f) <= chance || (Mathf.Approximately(chance, 0.5f) && riggedInFavor))
                {
                    damangeAmount = 10;
                    if (timingImage.fillAmount > 0.8f || Experiments.disableTimingGameS)
                    {
                        damangeAmount = 20;
                    }
                    Notification.SetText($"Did {damangeAmount} damage!", 1);
                    riggedInFavor = false;
                }
                else
                {
                    riggedInFavor = true;
                    Notification.SetText("Miss!", 1f);
                }

                

                ShootingUI.SetActive(false);
                inTimingState = false;
                GetComponent<SelectEnemy>().enabled = true;
                StartCoroutine(DoThing(target, shot, damangeAmount));
                ShotFired = true;

            }
            return;
        }
        else
        {
            
        }

        ChanceText.text = "Chance: " + Mathf.RoundToInt(chance * 100) + "%";
        if (uiInput.Confirm.WasPressedThisFrame())
        {
            ShootingUI.SetActive(true);
            inTimingState = true;
            GetComponent<SelectEnemy>().enabled = false;
            ToolTip.Set("<sprite name=\"a\"> Play\n<sprite name=\"b\"> Back");
        }
    }

    IEnumerator DoThing(Living target, Shot shot, int damageAmount)
    {
        GetComponent<SelectEnemy>().enabled = false;
        FindAnyObjectByType<Player>().transform.LookAt(target.transform.position);
        GameObject.FindGameObjectsWithTag("Player Animator")[0].GetComponent<Animator>().SetTrigger("Lyre");
        target.transform.LookAt(transform.position);


        shot.shooter = gameObject;
        shot.target = target.gameObject;
        yield return new WaitForSeconds(2);
        Projectile.instance.DrawShot(shot, damageAmount > 0);
        yield return new WaitForSeconds(1.2f);
        CameraSystem.SetTarget(target.transform);

        if (damageAmount > 0)
        {
            target.Damage(damageAmount);
            yield return new WaitForSeconds(1);
        }

        if(target.HealthPoints < 1)
        {
            // Death animation
            yield return new WaitForSeconds(5);
        }

        FindAnyObjectByType<Battle>().UseAction("Attack Shoot");
        CameraSystem.SetTarget(null);
        FindAnyObjectByType<Battle>().StartCoroutine(FindAnyObjectByType<Battle>().StartEnemyTurn());

        GetComponent<SelectEnemy>().enabled = true;
        gameObject.SetActive(false);

    }
}

public class Shot
{
    public GameObject shooter;
    public GameObject target;
    private Vector3 eyeLevel;

    public Shot(GameObject shooter, GameObject target, Vector3 eyeLevel)
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
            ProjectileEarlyDeathPoint = hit.point;
        }   

        return baseRate;
    }

    private static float LossOverDistance(float distance)
    {
        return 1.0f;

        // Cut later on, so its unused.
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