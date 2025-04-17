using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ProjectileSelection : MonoBehaviour
{
    private InputRotation rotation;
    public int range;

    private Tile startTile;
    private void Start()
    {
        startTile = FindAnyObjectByType<Player>().walker.tile;
        Debug.Log(startTile.name);
        rotation = GetComponent<InputRotation>();
    }


    private List<Tile> selection = new List<Tile>();
    // Update is called once per frame
    void Update()
    {
        selection.Clear();
        Tile.ClearAllStyle();

        Tile t = startTile;
        for (int i = 0; i < range; i++)
        { 
            if (t.GetConnection(rotation.facing) == null)
                break;

            t = t.GetConnection(rotation.facing).tile;
            if (t == null)
                break;

            t.SetStyle(TileStyle.Danger);
            selection.Add(t);
        }
    }
}
