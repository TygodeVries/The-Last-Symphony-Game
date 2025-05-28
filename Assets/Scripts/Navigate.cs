using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Navigate : MonoBehaviour
{
    [SerializeField] private Transform tileHighlight;

    [SerializeField] private int maxWalkSize = 10;

    private Player player;
    
    Tile selected;
    Camera mainCamera;
    Tile[] tiles;

    LineRenderer line;

    GameInput.UINavActions uiInput;
    public void Start()
    {
        line = GetComponent<LineRenderer>();
        tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        mainCamera = Camera.main;
        player = FindAnyObjectByType<Player>();
        selected = player.walker.tile;
        transform.position = player.transform.position;

        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;

        foreach (Tile tile in tiles)
        {
            tile.data = 9999;

            if (tile == null)
                continue;

            List<Tile> test = GridWalker.CalculatePath(tile, player.walker.tile);
            if (test == null)
                continue;

            tile.data = test.Count;
        }
    }

    public void OnApplicationQuit()
    {
        uiInput.Disable();
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

    private void OnEnable()
    {
        Start();

    }



    bool isWalking;
    public void Update()
    {

        tileHighlight.transform.parent = null;
        tileHighlight.transform.position = Vector3.Lerp(tileHighlight.transform.position, selected.transform.position, Time.deltaTime * 6);

        if (isWalking)
        {
            tileHighlight.gameObject.SetActive(false);
            return;
        }
        else
        {
            tileHighlight.gameObject.SetActive(true);

            foreach(Tile tile in tiles)
            {
                if (tile == null)
                    continue;

                if (tile.data < maxWalkSize)
                    tile.MarkSelected();
            }
        }

        if (uiInput.Confirm.WasPressedThisFrame())
        {
            StartCoroutine(Walk());
        }

        if(uiInput.Back.WasPressedThisFrame())
        {
            tileHighlight.gameObject.SetActive(false);
            CameraSystem.SetTarget(null);
            player.OpenActionGUI();
            gameObject.SetActive(false);
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
            if (tile == null)
                continue;

            if (tile.data >= maxWalkSize)
                continue;

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

        List<Tile> path = GridWalker.CalculatePath(selected, player.walker.tile);

        selected = nearest;
        UpdateLine(path);

        Protection protection = selected.GetComponent<Protection>();
        if (protection != null)
        {
            if (protection.xPositive)
                shields[0].SetActive(true);
            else
                shields[0].SetActive(false);

            if (protection.xNegative)
                shields[1].SetActive(true);
            else
                shields[1].SetActive(false);

            if (protection.zPositive)
                shields[2].SetActive(true);
            else
                shields[2].SetActive(false);

            if (protection.zNegative)
                shields[3].SetActive(true);
            else
                shields[3].SetActive(false);
        }
        else
        {
            shields[0].SetActive(false);
            shields[1].SetActive(false);
            shields[2].SetActive(false);
            shields[3].SetActive(false);
        }
    }

    [SerializeField] public List<GameObject> shields;

    public void UpdateLine(List<Tile> tiles)
    {

        line.positionCount = tiles.Count;
        for (int i = 0; i < tiles.Count; i++)
        {
            line.SetPosition(i, tiles[i].transform.position + new Vector3(0, 0.1f, 0));
        }
    }
}
