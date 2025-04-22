using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public IEnumerator TakeTurn()
    {
        Living player = FindAnyObjectByType<Player>().GetComponent<Living>();

        Shot shot = new Shot(GetComponent<Living>(), player.gameObject, new Vector3(0, 0.1f, 0));

        float c = shot.GetHitChance();
        Debug.Log("Current Hit Chance: " + c);

        if (c < 0.25f)
        {
            yield return StartCoroutine(Walk());
        }

        else
        {
            yield return StartCoroutine(Shoot(shot));
        }
    }

    public IEnumerator Walk()
    {
        Debug.Log("Walking...");
        CameraSystem.SetTarget(transform);
        Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        float[] score = new float[tiles.Length];

        Living me = GetComponent<Living>();


        Debug.Log($"Checking {tiles.Length} tiles!");
        ShotChangeEffector[] effectors = FindObjectsByType<ShotChangeEffector>(FindObjectsSortMode.None);
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        for (int i = 0; i < tiles.Length; i++)
        {
            if (Vector3.Distance(tiles[i].transform.position, me.transform.position) > 10)
            {
                score[i] -= 1000f;
            }

            // Check if friends can back me up
            foreach(Enemy enemy in enemies)
            {
                Shot see = new Shot(enemy.GetComponent<Living>(), tiles[i].gameObject, new Vector3(0, 0.1f, 0));
                if(see.GetHitChance() > 0.5f)
                {
                    score[i] += 3f;
                    Debug.Log("Found a friend!");
                }
            }

            if (tiles[i].IsOccupied())
            {
                score[i] -= 1000f;
            }

            foreach(ShotChangeEffector effector in effectors)
            {
                float effectorDistance = Vector3.Distance(tiles[i].transform.position, effector.transform.position);
               
                if (effectorDistance < effector.DistanceBypass)
                {
                    score[i] += 2f;
                }

                Player player = FindAnyObjectByType<Player>();

                float EffectorDistanceToPlayer = Vector3.Distance(effector.transform.position, player.transform.position);
                float TileDistanceToPlayer = Vector3.Distance(tiles[i].transform.position, player.transform.position);


                if (effectorDistance < TileDistanceToPlayer)
                {
                    // FUCK, WRONG SIDE OF THE WALL
                    score[i] -= 5f;
                }
                else
                {
                    score[i] += 2f;
                }
            }

            Shot shot = new Shot(Object.FindAnyObjectByType<Player>().GetComponent<Living>(), tiles[i].gameObject, new Vector3(0, 0.1f, 0));

            if(shot.GetHitChance() > 0.5f)
            {
                if(me.HealthPoints > 10)
                {
                    // Lets go attack!
                    score[i] += 10f;
                }
                else
                {
                    // Run away!!!!
                    score[i] -= 2f;
                }
                Debug.Log("Can shoot people here, and people can shoot me!");
            }
        }

        float best = 0;
        int bestIndex = -1;
        for (int i = 0; i < score.Length; i++)
        {
            if (score[i] > best)
            {
                best = score[i];
                bestIndex = i;
                Debug.Log(best);
                Debug.DrawLine(tiles[i].transform.position, tiles[i].transform.position + new Vector3(0, 1, 0) * best, Color.green, 10);
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            MeshRenderer renderer = tiles[i].GetComponent<MeshRenderer>();
            Material m = new Material(renderer.material.shader);

            float a = score[i] / best;
            m.color = new Color(a, 0, 0);

            if (i == bestIndex)
            {
                m.color = new Color(0, 1, 0);
            }

            renderer.material = m;
        }

        yield return GetComponent<GridWalker>().Navigate(tiles[bestIndex]);
        CameraSystem.SetTarget(null);
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
