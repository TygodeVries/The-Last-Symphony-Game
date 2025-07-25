using Firebase.Database;
using Firebase.Extensions;
using System;
using System.IO;
using System.Runtime.Remoting;
using TMPro;
using UnityEngine;

public class LevelButtonLoader : MonoBehaviour
{
    public GameObject popup;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform content;
    public void Start()
    {
        LoadLayout(LayoutType.Local);
    }

    public void GoToLocal()
    {
        LoadLayout(LayoutType.Local);
    }

    public void GoToOnline()
    {
        LoadLayout(LayoutType.Online);
    }

    public void LoadOnlineLayout()
    {

        infoText.text = "Loading Levels...";
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        dbRef.Child("levelList").GetValueAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted || task.IsCanceled)
            {
                infoText.text = "Failed to load levels...";
                Debug.LogError("Failed to fetch online levels: " + task.Exception);
                return;
            }

            if (!task.Result.Exists)
            {
                infoText.text = "No online levels found.";
                Debug.Log("No online levels found.");
                return;
            }

            foreach (var childSnapshot in task.Result.Children)
            {
                string levelId = childSnapshot.Key;
                string metadata = childSnapshot.Value.ToString();

                // Instantiate a button for each level
                GameObject buttonObj = Instantiate(levelButtonPrefab, content);
                TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();

                label.text = metadata;
                buttonObj.GetComponentInChildren<LevelButton>().file = "online:" + levelId;
                buttonObj.GetComponentInChildren<LevelButton>().HideUpload();
            }

            infoText.text = "";
        });
    }

    public void LoadLayout(LayoutType layoutType)
    {
        infoText.text = "Loading Levels...";
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        if (layoutType == LayoutType.Local)
        {
            LoadLocalLayout();
        }

        if (layoutType == LayoutType.Online)
        {
            LoadOnlineLayout();
        }
    }

    private async void LoadLocalLayout()
    {
        string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Macabre/Level";
        if(!Directory.Exists(localFolder))
        {
            Debug.Log("Creating level folder...");
            Directory.CreateDirectory(localFolder);
        }

        string[] files = Directory.GetFiles(localFolder);

        foreach(string file in files)
        {
            if (!file.EndsWith(".mlvl"))
            {
                continue;
            }

            Debug.Log($"Found level at {file}.");
            string levelData = (await File.ReadAllTextAsync(file)).Split("|")[0];

            GameObject gm = GameObject.Instantiate(levelButtonPrefab, parent: content);
            
            gm.GetComponentInChildren<TMP_Text>().text = levelData;
            gm.GetComponentInChildren<LevelButton>().file = file;
        }

        infoText.text = "";

        if(files.Length == 0)
        {
            infoText.text = "No levels found...";
        }
    }
}

public enum LayoutType
{
    Local,
    Online
}