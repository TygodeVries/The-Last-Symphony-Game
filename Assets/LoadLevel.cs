using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] public List<IdObject> objectTypes;
    public GameObject Requirements;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] lines = File.ReadAllLines("C:\\Users\\zttde\\AppData\\Roaming\\Dansmacabre\\level.txt");

        foreach (string line in lines)
        {
            string type = line.Split(' ')[0];
            
            int x = int.Parse(line.Split(' ')[1]);
            int z = int.Parse(line.Split(' ')[2]);

            foreach(IdObject idObject in objectTypes)
            {
                if(idObject.id == type)
                {
                    GameObject.Instantiate(idObject.prefab, new Vector3(x, 0, z), idObject.prefab.transform.rotation);
                }
            }
        }


        GameObject.Instantiate(Requirements);
        Tile.AutoTileAll();
        GridWalker.SnapAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class IdObject
{
    public string id;
    public GameObject prefab;
}