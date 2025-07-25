using Firebase.Database;
using Firebase.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public GameObject uploadButton;
    public GameObject editButton;
    public void HideUpload()
    {
        editButton.SetActive(false);
        uploadButton.SetActive(false);
    }

    public void PlayLevel()
    {
        if (file.StartsWith("online:"))
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Macabre/Level/Online/");

            string levelId = file.Substring("online:".Length);
            string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Macabre/Level/Online/" + levelId + ".mlvl";
            if (File.Exists(localFolder))
            {
                Debug.LogWarning("Playing Chached version!");
                PlayerPrefs.SetString("LastLoadedLevel", localFolder.Split("Macabre/")[1]);
                SceneManager.LoadScene("LevelHost");
                return;
            }

            DownloadLevel(levelId, (data) =>
            {
                Debug.Log($"Writing file data to {localFolder}...");
                File.WriteAllText(localFolder, levelId + "|" + data);

                Debug.Log("Opening Level...");
                PlayerPrefs.SetString("LastLoadedLevel", localFolder.Split("Macabre/")[1]);
                SceneManager.LoadScene("LevelHost");
                return;
            }, (error) =>
            {

            });

            return;
        }
       
       
        string filePath = file.Split("Macabre/")[1];
        
        Debug.Log($"Set level path: {filePath}");

        PlayerPrefs.SetString("LastLoadedLevel", filePath);
        SceneManager.LoadScene("LevelHost");
    }


    public void EditLevel()
    {
        if (file.StartsWith("online:"))
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Macabre/Level/Online/");

            string levelId = file.Substring("online:".Length);
            string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Macabre/Level/Online/" + levelId + ".mlvl";
            if (File.Exists(localFolder))
            {
                Debug.LogWarning("Playing Chached version!");
                PlayerPrefs.SetString("LastLoadedLevel", localFolder.Split("Macabre/")[1]);
                SceneManager.LoadScene("LevelEdit");
                return;
            }

            DownloadLevel(levelId, (data) =>
            {
                Debug.Log($"Writing file data to {localFolder}...");
                File.WriteAllText(localFolder, levelId + "|" + data);

                Debug.Log("Opening Level...");
                PlayerPrefs.SetString("LastLoadedLevel", localFolder.Split("Macabre/")[1]);
                SceneManager.LoadScene("LevelEdit");
                return;
            }, (error) =>
            {

            });

            return;
        }


        string filePath = file.Split("Macabre/")[1];

        Debug.Log($"Set level path: {filePath}");

        PlayerPrefs.SetString("LastEditLevelName", GetComponentInChildren<TMP_Text>().text);
        PlayerPrefs.SetString("LastLoadedLevel", filePath);
        SceneManager.LoadScene("LevelEdit");
    }

    public void DownloadLevel(string levelId, Action<string> onSuccess, Action<string> onError)
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        string path = $"levelData/{levelId}";

        dbRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                string error = task.Exception?.Message ?? "Unknown error";
                Debug.LogError($"Failed to download level: {error}");
                onError?.Invoke(error);
                return;
            }

            DataSnapshot snapshot = task.Result;

            if (!snapshot.Exists || snapshot.Value == null)
            {
                Debug.LogWarning($"Level {levelId} not found.");
                onError?.Invoke("Level not found.");
                return;
            }

            string levelData = snapshot.Value.ToString();
            Debug.Log($"Level {levelId} downloaded successfully.");
            onSuccess?.Invoke(levelData);
        });
    }

    public string file;
    public void UploadThisLevel()
    {

        GameObject.FindAnyObjectByType<LevelButtonLoader>().popup.SetActive(true);
        GameObject.Find("UPLOAD_INFO").GetComponent<TMP_Text>().text = "Uploading...";
        GameObject.Find("LVLID").GetComponent<TMP_Text>().text = "";
        string[] fileData = File.ReadAllText(file).Split("|");

        string firstLine = fileData[0];
        string rest = string.Join("|", fileData.Skip(1));

        string id = GenerateId();
        UploadLevel(id, firstLine, rest, (success) =>
        {
            GameObject.Find("UPLOAD_INFO").GetComponent<TMP_Text>().text = "Upload Complete!";
            GameObject.Find("LVLID").GetComponent<TMP_Text>().text = $"Level ID:\n{id}";
        });
    }

    public string GenerateId()
    {
        string chars = "qwertyuiopasdfghjklzxcvbnm1234567890";

        string id = "";
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                id += chars[UnityEngine.Random.Range(0, chars.Length)];
            }

            id += "-";
        }

        id = id.Substring(0, id.Length - 1);
        return id;
    }

    public void UploadLevel(string levelId, string metadata, string levelData, Action<bool> onComplete)
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        string uid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (uid == null)
        {
            Debug.LogError("User not authenticated");
            onComplete?.Invoke(false);
            return;
        }

        var levelListPath = $"levelList/{levelId}";
        var levelDataPath = $"levelData/{levelId}";
        var userUploadPath = $"userUploads/{uid}";

        var updates = new System.Collections.Generic.Dictionary<string, object>();

        updates[levelListPath] = metadata;
        updates[levelDataPath] = levelData;
        updates[userUploadPath] = ServerValue.Timestamp; // Store server timestamp for rate limiting

        dbRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to upload level: " + task.Exception);
                onComplete?.Invoke(false);
            }
            else
            {
                Debug.Log("Level uploaded successfully.");
                onComplete?.Invoke(true);
            }
        });
    }
}
