using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugLines : MonoBehaviour
{

    void Update()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Player player = FindAnyObjectByType<Player>();

        foreach (Enemy enemy in enemies) {
            Debug.DrawLine(player.transform.position, enemy.transform.position, Color.green);
        }
    }
}
