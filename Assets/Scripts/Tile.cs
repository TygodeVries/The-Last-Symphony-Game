using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]public bool CanNavigateInto = true;

    [HideInInspector] [SerializeField] public List<TileConnection> connections;


    public bool IsOccupied()
    {
        GridWalker[] walkers = FindObjectsByType<GridWalker>(FindObjectsSortMode.None);
        foreach(GridWalker walker in walkers)
        {
            Vector3 walkerPos = walker.transform.position;

            if (Vector3.Distance(transform.position, walkerPos) < 1)
                return true;
        }


        return false;
    }

    private TileStyle currentStyle = TileStyle.None;
    public void SetStyle(TileStyle style)
    {
        if (currentStyle == style) return;

        if (style == TileStyle.None)
        {
            GetComponent<MeshRenderer>().material = Styles.GetInstance().none;
        }

        if (style == TileStyle.Danger)
        {
            GetComponent<MeshRenderer>().material = Styles.GetInstance().danger;
        }

        currentStyle = style;
    }

    public static void ClearAllStyle()
    {
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);

        foreach (Tile tile in tiles)
        {
            tile.SetStyle(TileStyle.None);
        }
    }

    public TileConnection GetConnection(TileDirection direction)
    {
        foreach (TileConnection connection in connections)
            if (connection.direction == direction)
                return connection;
        return null;
    }
    public void AddConnection(TileDirection direction, Tile tile)
    {
        if (tile == null)
            return;

        connections.Add(new TileConnection(direction, tile));
    }

    public void RemoveConnections()
    {
        connections = new List<TileConnection>();
    }

    public void OnDrawGizmos()
    {
        if(connections == null) return;

        Vector3 offset = Vector3.zero;
        if(!CanNavigateInto)
        {
            Gizmos.color = Color.red;
            offset = new Vector3(0, 0.01f, 0);
        }

        foreach (TileConnection connection in connections)
        {
            if (connection.tile.CanNavigateInto)
            {
                Gizmos.DrawLine(transform.position + offset, connection.tile.transform.position + offset);
            }
        }
    }

    public IEnumerator StepBehavior()
    {
        foreach(TileBehavior b in GetComponents<TileBehavior>())
        {
            yield return StartCoroutine(b.Step());
        }

        Debug.Log("Completed Behavior!");
    }

    public List<Tile> GetConnections()
    {
        List<Tile> tiles = new List<Tile>();
        foreach(TileConnection connection in connections)
        {
            tiles.Add(connection.tile);
        }
        return tiles;
    }
}


public class TileUI : EditorWindow
{
    public static bool RenderType;

    [MenuItem("Tygo/Tile Utils")]
    public static void ShowWindow()
    {
        GetWindow<TileUI>("Tile Utils");
    }

    void OnGUI()
    {
        GUILayout.Label("Editor Tile Utils", EditorStyles.boldLabel);

        if(GUILayout.Button("Auto Tile"))
        {
            Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);

            GameObject level = GameObject.Find("Level");
            if (level == null)
                level = new GameObject("Level");

            int done = 0;
            int total = tiles.Length;
            foreach(Tile tile in tiles)
            {
                tile.RemoveConnections();

                tile.AddConnection(TileDirection.XPositive, FindTileNear(tiles, tile.transform.position + new Vector3(1, 0, 0)));
                tile.AddConnection(TileDirection.XNegative, FindTileNear(tiles, tile.transform.position + new Vector3(-1, 0, 0)));

                tile.AddConnection(TileDirection.ZPositive, FindTileNear(tiles, tile.transform.position + new Vector3(0, 0, 1)));
                tile.AddConnection(TileDirection.ZNegative, FindTileNear(tiles, tile.transform.position + new Vector3(0, 0, -1)));

                done++;
                EditorUtility.DisplayProgressBar("Connecting Tiles", $"Processed: {done} out of {total}", ((float) total / (float) done) * 100);

                if (tile.transform.parent != level)
                    tile.transform.parent = level.transform;

                tile.SetStyle(TileStyle.None);
            }

            EditorUtility.ClearProgressBar();
        }

        if (GUILayout.Button("Snap Walkers"))
        {
            Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
            GridWalker[] gridWalkers = UnityEngine.Object.FindObjectsByType<GridWalker>(FindObjectsSortMode.None);

            foreach (GridWalker gridWalker in gridWalkers)
            {
                Tile nearest = null;
                float nearestDistance = 1000;
                foreach (Tile tile in tiles)
                {
                    float distance = Vector3.Distance(gridWalker.transform.position, tile.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearest = tile;
                    }
                }

                if (nearest != null)
                {
                    gridWalker.transform.position = nearest.transform.position;
                    gridWalker.SetTile(nearest);
                }
                else
                {
                    Debug.LogWarning($"Could not find good location for {gridWalker.transform.name}");
                }
            }
        }


        GUILayout.Label("Debugging", EditorStyles.boldLabel);
        RenderType = GUILayout.Toggle(RenderType, "Render Behaviour");
    }

    private Tile FindTileNear(Tile[] tiles, Vector3 location)
    {
        foreach (Tile tile in tiles)
        {
            if (Vector3.Distance(tile.transform.position, location) < 0.1f)
                return tile;
        }

        return null;
    }
}

public enum TileDirection
{
    XPositive,
    XNegative,
    ZPositive,
    ZNegative
}

[System.Serializable]
public class TileConnection
{
    public TileDirection direction;
    public Tile tile;

    public TileConnection(TileDirection direction, Tile tile)
    {
        this.direction = direction;
        this.tile = tile;
    }
}