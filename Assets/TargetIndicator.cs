using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private GameObject linePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnDisable()
    {
        Clean();
    }

    public void Clean()
    {
        while(lines.Count > 0)
        {
            Destroy(lines[0]);
            lines.RemoveAt(0);
        }
    }


    private List<GameObject> lines = new List<GameObject>();
    // Update is called once per frame
    void Update()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        while(lines.Count > enemies.Length)
        {
            lines.RemoveAt(0);
        }

        while (lines.Count < enemies.Length)
        {
            GameObject line = Object.Instantiate(linePrefab);
            lines.Add(line);
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i];
            GameObject line = lines[i];

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.positionCount = 2;
            render.SetPosition(0, origin.position);
            render.SetPosition(1, enemy.transform.position);
            TMP_Text text = line.GetComponentInChildren<TMP_Text>();

            Vector3 delta = (enemy.transform.position - origin.position);

            text.transform.position = origin.position + delta.normalized + new Vector3(0, 1, 0);
            Shot shot = new Shot(this.origin.gameObject, enemy.gameObject, new Vector3(0, 0.1f, 0));
            text.text = $"{Mathf.Round(shot.GetHitChance() * 100f)}%";

        }
    }
}
