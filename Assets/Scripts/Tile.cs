using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public bool CanNavigateInto = true;

    [HideInInspector] [SerializeField] public List<TileConnection> connections;

    
    public static void AutoTileAll()
    {
        Tile[] tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);

        GameObject level = GameObject.Find("Level");
        if (level == null)
            level = new GameObject("Level");

        int done = 0;
        int total = tiles.Length;
        foreach (Tile tile in tiles)
        {
            tile.RemoveConnections();

            tile.AddConnection(TileDirection.XPositive, FindTileNear(tiles, tile.transform.position + new Vector3(1, 0, 0)));
            tile.AddConnection(TileDirection.XNegative, FindTileNear(tiles, tile.transform.position + new Vector3(-1, 0, 0)));

            tile.AddConnection(TileDirection.ZPositive, FindTileNear(tiles, tile.transform.position + new Vector3(0, 0, 1)));
            tile.AddConnection(TileDirection.ZNegative, FindTileNear(tiles, tile.transform.position + new Vector3(0, 0, -1)));

            done++;
            if (tile.transform.parent != level)
                tile.transform.parent = level.transform;

            tile.SetStyle(TileStyle.None);
        }

    }

    private static Tile FindTileNear(Tile[] tiles, Vector3 location)
    {
        foreach (Tile tile in tiles)
        {
            if (Vector3.Distance(tile.transform.position, location) < 0.1f)
                return tile;
        }

        return null;
    }

    public int data;
    private bool IsSelected;
    public void MarkSelected()
    {
        IsSelected = true;
    }

    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material defaultMaterial;
    private bool cacheSelected = false;
    public void RenderSelected()
    {
        if(IsSelected && !cacheSelected)
        {
            GetComponent<MeshRenderer>().material = selectedMaterial;
        }
        
        if(!IsSelected && cacheSelected)
        {
            GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        cacheSelected = IsSelected;
        IsSelected = false;
    }

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

    public bool IsOccupied(GridWalker ignore)
    {
        GridWalker[] walkers = FindObjectsByType<GridWalker>(FindObjectsSortMode.None);
        foreach (GridWalker walker in walkers)
        {
            if (walker == ignore)
                continue;

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

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    public void RemoveConnections()
    {
        connections = new List<TileConnection>();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
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

#if UNITY_EDITOR
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
            Tile.AutoTileAll();   
        }

        if (GUILayout.Button("Snap Walkers"))
        {
            GridWalker.SnapAll();
        }

        if (GUILayout.Button("Auto Protection"))
        {
            if(Selection.gameObjects.Length != 4)
            {
                EditorUtility.DisplayDialog("ERROR", "You must select 4 tiles", "Yeye, I get it.");
                return;
            }

            Vector3 sum = new Vector3();
            for(int i = 0; i < 4; i++)
            {
                GameObject tileObj = Selection.gameObjects[i];

                Tile tile = tileObj.GetComponent<Tile>();
                if(tile == null)
                {
                    EditorUtility.DisplayDialog("ERROR", "Non-Tile object selected.", "Yeye, I get it.");
                    return;
                }

                Protection protection = tile.GetComponent<Protection>();
                if (protection == null)
                {
                    tile.AddComponent<Protection>();
                }

                sum += tile.transform.position;
            }

            Vector3 center = sum / 4;
            for (int i = 0; i < 4; i++)
            {
                Protection protection = Selection.gameObjects[i].GetComponent<Protection>();

                protection.xNegative = false;
                protection.xPositive = false;
                protection.zNegative = false;
                protection.zPositive = false;

                if (protection.transform.position.x > center.x + 0.5f)
                {
                    protection.xNegative= true;
                }

                if (protection.transform.position.x < center.x - 0.5f)
                {
                    protection.xPositive = true;
                }

                if (protection.transform.position.z > center.z + 0.5f)
                {
                    protection.zNegative = true;
                }

                if (protection.transform.position.z < center.z - 0.5f)
                {
                    protection.zPositive = true;
                }
            }
        }


            GUILayout.Label("Debugging", EditorStyles.boldLabel);
        RenderType = GUILayout.Toggle(RenderType, "Render Behaviour");
    }
}
#endif

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