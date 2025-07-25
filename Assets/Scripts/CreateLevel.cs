using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLevel : MonoBehaviour
{
    public TMP_InputField levelName;

    private void Update()
    {
        if(levelName.text.Length > 15)
        {
            levelName.text = levelName.text.Substring(0, 15);
        }
    }

    public void CreateTheLevel()
    {
        string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Macabre/Level";
        File.WriteAllText($"{localFolder}/{levelName.text}.mlvl", $"{levelName.text}|tile 0 0|");

        FindAnyObjectByType<LevelButtonLoader>().GoToLocal();
    }
}
