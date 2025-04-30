using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Navigate : MonoBehaviour
{
    [SerializeField] private Transform tileHighlight;
    [SerializeField] private GameObject ring;

    private Player player;
    
    Tile selected;
    Camera mainCamera;
    Tile[] tiles;

    LineRenderer line;

    public void Start()
    {
        line = GetComponent<LineRenderer>();
        tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        mainCamera = Camera.main;
        player = FindAnyObjectByType<Player>();
        selected = player.walker.tile;
        transform.position = player.transform.position;
    }

    IEnumerator Walk()
    {
        isWalking = true;
        CameraSystem.SetTarget(player.transform);
        yield return StartCoroutine(player.WalkTo(selected));
        gameObject.SetActive(false);
        player.OpenActionGUI();
        isWalking = false;
    }

    

    bool isWalking;
    public void Update()
    {
        if (isWalking)
        {
            ring.SetActive(false);
            return;
        }

        ring.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(Walk());
        }

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input.magnitude < 0.5)
            input = Vector3.zero;

        CameraSystem.SetTarget(transform);
        Vector3 motion = mainCamera.transform.rotation * new Vector3(input.x, 0, input.y);
        motion.y = 0;
        motion.Normalize();
        motion *= Time.deltaTime * 3;

        float newDistance = Vector3.Distance(player.transform.position, transform.position + motion);
        if (newDistance < 10)
        {
            transform.position += motion;
        }

        Tile nearest = null;
        float nearestDistance = 1000;
        foreach (Tile tile in tiles)
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = tile;
            }
        }

        if(nearest == null)
        {
            Debug.LogWarning("No tile found for selector!");
            return;
        }

        tileHighlight.transform.position = nearest.transform.position;
        selected = nearest;

        UpdateLine();
    }

    public void UpdateLine()
    {
        List<Tile> tiles = GridWalker.CalculatePath(selected, player.walker.tile);

        line.positionCount = tiles.Count;
        for (int i = 0; i < tiles.Count; i++)
        {
            line.SetPosition(i, tiles[i].transform.position + new Vector3(0, 0.1f, 0));
        }
    }
}
