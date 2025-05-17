using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] public bool DebugMode;

    [SerializeField] private float MinimumPlayerDistance = 3f;
    [SerializeField] private float MaximumPlayerDistance = 10f;

    [SerializeField] private int InOthersVisionScore = 3;
    [SerializeField] private int NearHidingPlaceScore = 3;
    [SerializeField] private int InfrontHidingPlaceScore = -3;
    [SerializeField] private int BehindHidingPlaceScore = 10;

    [SerializeField] private int PlayerInVisionScoreHealthy = 4;
    [SerializeField] private int PlayerInVisionScoreAlmostDead = -5;

    [SerializeField] private int HealthyThreshold = 10;
    [SerializeField] private int NotInRangeScore = -1000;

    [SerializeField] private float ShotRiskinessThreshold = 0.5f;

    [SerializeField] private float TileAlreadyOccupiedScore = -10000;

    private Animator animator;

    public void OnDrawGizmos()
    {
        if (DebugMode)
            StartCoroutine(Walk());
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public IEnumerator TakeTurn()
    {
        Living player = FindAnyObjectByType<Player>().GetComponent<Living>();

        Shot shot = new Shot(GetComponent<Living>().gameObject, player.gameObject, new Vector3(0, 0.1f, 0));

        float c = shot.GetHitChance();
        Debug.Log("Current Hit Chance: " + c);

        if (c > ShotRiskinessThreshold)
        {
            animator.SetTrigger("Shoot");
            yield return StartCoroutine(Shoot(shot));
        }

        animator.SetBool("IsWalking", true);
        yield return StartCoroutine(Walk());
        animator.SetBool("IsWalking", false);


    }

    public IEnumerator Walk()
    {
        Debug.Log("Walking...");
        CameraSystem.SetTarget(transform);
        Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        float[] score = new float[tiles.Length];

        Living me = GetComponent<Living>();
        Player player = FindAnyObjectByType<Player>();

        Debug.Log($"Checking {tiles.Length} tiles!");

        for(int i = 0; i < tiles.Length; i++)
        {
            List<Tile> path = GridWalker.CalculatePath(tiles[i], me.GetComponent<GridWalker>().tile);

            Debug.Log(path);

            if(path == null)
            {
                tiles[i] = null;
            }
        }

        ShotChangeEffector[] effectors = FindObjectsByType<ShotChangeEffector>(FindObjectsSortMode.None);
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                continue;
            }

            TileEffector tileEffector = tiles[i].GetComponent<TileEffector>();

            if (tileEffector != null)
            {
                if(tileEffector.OnlyEffectIfHealthy)
                {
                    if (me.HealthPoints > HealthyThreshold)
                    {
                        score[i] += tileEffector.BaseScore;
                    }
                }
                else
                {
                    score[i] += tileEffector.BaseScore;
                }
                    
            }

            float tileDistanceToPlayer = Vector3.Distance(tiles[i].transform.position, player.transform.position);

            if (tileDistanceToPlayer > MaximumPlayerDistance || tileDistanceToPlayer < MinimumPlayerDistance)
            {
                score[i] += NotInRangeScore;
            }

            // Check if friends can back me up
            foreach(Enemy enemy in enemies)
            {
                if (enemy == this)
                    continue;

                Shot see = new Shot(enemy.GetComponent<Living>().gameObject, tiles[i].gameObject, new Vector3(0, 0.1f, 0));
                if(see.GetHitChance() > 0.5f)
                {
                    score[i] += InOthersVisionScore;
                }
            }

            if (tiles[i].IsOccupied(this.GetComponent<GridWalker>()))
            {
                score[i] += TileAlreadyOccupiedScore;
            }

            // Check if we can hide
            foreach(ShotChangeEffector effector in effectors)
            {
                float effectorDistance = Vector3.Distance(tiles[i].transform.position, effector.transform.position);
               
                if (effectorDistance < effector.DistanceBypass)
                {
                    score[i] += NearHidingPlaceScore; // near hiding place

                    float EffectorDistanceToPlayer = Vector3.Distance(effector.transform.position, player.transform.position);
                    float TileDistanceToPlayer = Vector3.Distance(tiles[i].transform.position, player.transform.position);


                    if (EffectorDistanceToPlayer < TileDistanceToPlayer)
                    {
                        score[i] += BehindHidingPlaceScore;
                    }
                    else
                    {
                        score[i] += InfrontHidingPlaceScore;
                    }
                }
            }

            Shot shot = new Shot(Object.FindAnyObjectByType<Player>().gameObject, tiles[i].gameObject, new Vector3(0, 0.1f, 0));

            if(shot.GetHitChance() > ShotRiskinessThreshold)
            {
                if(me.HealthPoints > HealthyThreshold)
                {
                    // Lets go attack!
                    score[i] += PlayerInVisionScoreHealthy;
                }
                else
                {
                    // Run away!!!!
                    score[i] += PlayerInVisionScoreAlmostDead;
                }
            }
        }

        float best = FirstNotNull(score, tiles).Item1;
        int bestIndex = FirstNotNull(score, tiles).Item2;
        for (int i = 0; i < score.Length; i++)
        {
            if (tiles[i] == null)
                continue;

            if (score[i] > best)
            {
                best = score[i];
                bestIndex = i;
            }
        }

        if(best < 0)
        {
            Debug.LogError("Very low score, may cause visual bugs!");
        }

#if UNITY_EDITOR
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
                continue;

            if (i == bestIndex)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(tiles[i].transform.position, new Vector3(0.6f, 1f, 0.6f));
            }
            else
            {
                Gizmos.color = new Color(score[i] / best, 0, 0);
                Gizmos.DrawCube(tiles[i].transform.position, new Vector3(0.2f, 0.3f, 0.2f));
            }
        }
#endif

        if (DebugMode)
            yield break;

        if (tiles[bestIndex] == null)
        {
            Debug.LogError($"Could not find a perfect tile :(, {bestIndex} is null.");
        }

        yield return GetComponent<GridWalker>().Navigate(tiles[bestIndex]);
        CameraSystem.SetTarget(null);
    }

    public (float, int) FirstNotNull(float[] objects, Tile[] tiles)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
                return (objects[i], i);
        }

        Debug.LogError("EVERYTHING IS BROKEN? NO TILES FOR ME???");
        return (-1000, 0);
    }

    public IEnumerator Shoot(Shot shot)
    {
        CameraSystem.SetTarget(transform); // Look at us
        yield return new WaitForSeconds(1f);
        Projectile.instance.DrawShot(shot);
        CameraSystem.SetTarget(shot.target.transform);   // Look at player  
        yield return new WaitForSeconds(1f);

        float c = shot.GetHitChance();
        Debug.Log("Chance of hit hitting, " + c);
        if (Random.Range(0f, 1f) <= c)
        {
            // Shot hit
            Notification.SetText("Hit!", 1f);
            shot.target.GetComponent<Living>().Damage(20);
        }
        else
        {
            // Shot miss
            Notification.SetText("Miss!", 1f);
            Debug.Log("Miss!");
        }

        yield return new WaitForSeconds(1f);
        CameraSystem.SetTarget(null);
    }
}
