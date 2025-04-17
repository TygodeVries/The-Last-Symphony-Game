using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public IEnumerator TakeTurn()
    {
        Living player = FindAnyObjectByType<Player>().GetComponent<Living>();

        Shot shot = new Shot(GetComponent<Living>(), player.gameObject, new Vector3(0, 0.1f, 0));

        if (shot.GetHitChance() < 0.5f)
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
        CameraSystem.SetTarget(transform);
        Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        float[] score = new float[tiles.Length];

        Living me = GetComponent<Living>();


        Debug.Log($"Checking {tiles.Length} tiles!");
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
                    score[i] += 1f;
                    Debug.Log("Found a friend!");
                }
            }

            Shot shot = new Shot(Object.FindAnyObjectByType<Player>().GetComponent<Living>(), tiles[i].gameObject, new Vector3(0, 0.1f, 0));

            if(shot.GetHitChance() > 0.5f)
            {
                if(me.HealthPoints > 20)
                {
                    // Lets go attack!
                    score[i] += 4f;
                }
                else
                {
                    // Run away!!!!
                    score[i] -= 4f;
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

        yield return GetComponent<GridWalker>().Navigate(tiles[bestIndex]);
        CameraSystem.SetTarget(null);
    }

    public IEnumerator Shoot(Shot shot)
    {
        CameraSystem.SetTarget(transform); // Look at us
        yield return new WaitForSeconds(1f);
        CameraSystem.SetTarget(shot.target.transform);   // Look at player  

        yield return new WaitForSeconds(1f);

        if (Random.Range(0f, 1f) <= shot.GetHitChance())
        {
            // Shot hit
            shot.target.GetComponent<Living>().Damage(20);
        }
        else
        {
            // Shot miss
            Debug.Log("Miss!");
        }

        yield return new WaitForSeconds(1f);
        CameraSystem.SetTarget(null);
    }
}
