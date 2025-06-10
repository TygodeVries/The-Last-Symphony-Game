using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    GameInput.UINavActions uiInput;

    public List<Placeable> placeables = new List<Placeable>();
    public int selected;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;
    }
    public RawImage preview;

    public void SaveAndPlay()
    {
        string levelData = "";

        LevelObject[] levelObjects = FindObjectsByType<LevelObject>(FindObjectsSortMode.None);
        foreach (LevelObject levelObject in levelObjects)
        {
            levelData += $"{levelObject.id} {Mathf.RoundToInt(levelObject.transform.position.x)} {Mathf.RoundToInt(levelObject.transform.position.z)}\n";
        }

        if (!Directory.Exists(LoadLevel.LevelFolder()))
        {
            Directory.CreateDirectory(LoadLevel.LevelFolder());
            Debug.Log("Created Level Folder");
        }

        File.WriteAllText(LoadLevel.LevelPath(), levelData);

        FindAnyObjectByType<Loader>().Goto("LevelHost");
    }

    Vector3 internalPosistion = Vector3.zero;
    void Update()
    {

        if(uiInput.EditorRight.WasPressedThisFrame())
        {
            selected++;
            if (selected >= placeables.Count)
                selected = 0;
        }

        if (uiInput.EditorLeft.WasPressedThisFrame())
        {
            selected--;
            if (selected < 0)
                selected = placeables.Count - 1;
        }

        Vector2 rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 input = rawInput * Time.deltaTime * 3;
        internalPosistion += new Vector3(input.x, 0, input.y);

        if(rawInput.magnitude < 0.1f)
        {
            internalPosistion = round(internalPosistion);
        }

        transform.position = Vector3.Lerp(transform.position, round(internalPosistion), Time.deltaTime * 5);


        preview.texture = placeables[selected].icon;
        if (uiInput.Back.WasPressedThisFrame())
        {
            LevelObject[] objs = FindObjectsByType<LevelObject>(FindObjectsSortMode.None);

            LevelObject toDelete = null;
            foreach (LevelObject obj in objs)
            {
                if(Vector3.Distance(obj.transform.position, transform.position) < 0.5f)
                {
                    if(toDelete == null || obj.importance > toDelete.importance)
                    {
                        toDelete = obj;
                    }
                }
            }

            Destroy(toDelete.gameObject);
        }

        if(uiInput.Confirm.WasPressedThisFrame())
        {
            GameObject.Instantiate(placeables[selected].prefab, round(internalPosistion), placeables[selected].prefab.transform.rotation);
        }
    }

    Vector3 round(Vector3 a)
    {
        return new Vector3(Mathf.Round(a.x), Mathf.Round(a.y), Mathf.Round(a.z));
    }
}
[Serializable]
public class Placeable
{
    public GameObject prefab;
    public Texture2D icon;
}