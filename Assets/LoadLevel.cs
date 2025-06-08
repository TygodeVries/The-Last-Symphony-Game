using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] public List<IdObject> objectTypes;
    public GameObject Requirements;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!File.Exists("C:\\Users\\zttde\\AppData\\Roaming\\Dansmacabre\\level.txt"))
        {
            SceneManager.LoadScene("LevelEdit");
        }

        string[] lines = File.ReadAllLines("C:\\Users\\zttde\\AppData\\Roaming\\Dansmacabre\\level.txt");

        foreach (string line in lines)
        {
            string type = line.Split(' ')[0];
            
            int x = int.Parse(line.Split(' ')[1]);
            int z = int.Parse(line.Split(' ')[2]);

            bool found = false;
            foreach(IdObject idObject in objectTypes)
            {
                if(idObject.id == type)
                {
                    Debug.Log($"Spawning {type} at {x}, {z}");
                    GameObject.Instantiate(idObject.prefab, new Vector3(x, 0, z), idObject.prefab.transform.rotation);
                    found = true;
                }
            }

            if(!found)
            {
                Debug.LogError("Could not find " + type);
            }
        }


        if(Requirements != null)
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