using NUnit.Framework;
using System;
using UnityEngine;

public class SelectEnemy : MonoBehaviour
{
    Enemy[] enemies;
    int selected;
    void Start()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    public Enemy GetSelected()
    {
        return enemies[selected];
    }

    // Update is called once per frame
    void Update()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        if (Input.GetKeyDown(KeyCode.E))
            selected++;
        if (Input.GetKeyDown(KeyCode.Q))
            selected--;

        if (selected < 0)
            selected = enemies.Length - 1;

        if (selected == enemies.Length)
            selected = 0;

        CameraSystem.SetTarget(enemies[selected].transform);
    }
}
