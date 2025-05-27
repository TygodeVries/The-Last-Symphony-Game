using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Feedbacker : MonoBehaviour
{
    public List<Button> smilies;
    public GameObject ui;

    public RawImage screenShotImage;
    public GameObject loadingIndicator;

    public TMP_InputField inputField;
    public TMP_Dropdown dropDown;

    private string filename;
    private Texture2D screenshotTexture;
    public int ind;
    private bool open = false;

    public void Open()
    {
        if (!open)
            StartCoroutine(TakeScreen());
        else
            StartCoroutine(Close());
    }

    public IEnumerator Close()
    {
        open = false;
        yield return new WaitForEndOfFrame();
        ui.SetActive(false);
    }

    public IEnumerator TakeScreen()
    {
        Guid guid = Guid.NewGuid();
        filename = $"{Application.persistentDataPath}/{guid}.png";

        open = true;
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log("Saving to " + filename);
        yield return new WaitForSeconds(0.5f); // wait for the file to be written

        byte[] rawData = System.IO.File.ReadAllBytes(filename);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(rawData);
        tex = ScaleTexture(tex, 1280, 720);

        byte[] finalData = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, finalData);

        screenshotTexture = tex;
        screenShotImage.texture = tex;
        ui.SetActive(true);
    }

    public void Select(int i)
    {
        for (int m = 0; m < smilies.Count; m++)
        {
            smilies[m].interactable = true;
        }

        ind = i;
        smilies[i].interactable = false;
    }

    public void Submit()
    {
        StartCoroutine(SubmitFeedback());
    }

    private IEnumerator SubmitFeedback()
    {
        StartCoroutine(Close());
        loadingIndicator.SetActive(true);

        string[] smiles = { ":(", ":|", ":)", ":D" };
        string cloudName = "de8u6k7qs";
        string uploadPreset = "unity_webgl";

        // Upload image to Cloudinary
        byte[] imageData = screenshotTexture.EncodeToPNG();

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, "screenshot.png", "image/png");
        form.AddField("upload_preset", uploadPreset);

        UnityWebRequest uploadRequest = UnityWebRequest.Post($"https://api.cloudinary.com/v1_1/{cloudName}/image/upload", form);
        yield return uploadRequest.SendWebRequest();

        if (uploadRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Cloudinary upload failed: " + uploadRequest.error);
            loadingIndicator.SetActive(false);
            yield break;
        }

        // Parse the response to get the image URL
        string jsonResponse = uploadRequest.downloadHandler.text;
        string imageUrl = GetJsonField(jsonResponse, "secure_url");

        string gameData = "Scene: " + SceneManager.GetActiveScene().name;
        string smile = smiles[ind];
        string type = dropDown.options[dropDown.value].text;
        string text = inputField.text;

        // Send feedback to Google Script
        FeedbackData data = new FeedbackData
        {
            type = type,
            smile = smile,
            screenshot = imageUrl,
            text = text,
            game = gameData
        };

        string jsonPayload = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest feedbackRequest = new UnityWebRequest(
            "https://script.google.com/macros/s/AKfycbwUzFgNVxDtCEVamOeGxBz4zJBql82ixrqikEdwGyOK-0-dUROLTWw8xPaPnIL65ZPcJw/exec", "POST");

        feedbackRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        feedbackRequest.downloadHandler = new DownloadHandlerBuffer();

        // THIS IS THE KEY: prevents CORS preflight
        feedbackRequest.SetRequestHeader("Content-Type", "text/plain;charset=utf-8");

        // Enable redirect following (important for Google Apps Script responses)
        feedbackRequest.redirectLimit = 4;

        yield return feedbackRequest.SendWebRequest();

        if (feedbackRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Feedback send failed: " + feedbackRequest.error);
            Debug.LogError("Response: " + feedbackRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("Feedback sent successfully: " + feedbackRequest.downloadHandler.text);
        }

        loadingIndicator.SetActive(false);

    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / result.width, (float)i / result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }

    [Serializable]
    public class FeedbackData
    {
        public string type;
        public string smile;
        public string screenshot;
        public string text;
        public string game;
    }

    private string GetJsonField(string json, string fieldName)
    {
        int index = json.IndexOf($"\"{fieldName}\"");
        if (index == -1) return null;
        int start = json.IndexOf(":", index) + 1;
        int firstQuote = json.IndexOf("\"", start) + 1;
        int endQuote = json.IndexOf("\"", firstQuote);
        return json.Substring(firstQuote, endQuote - firstQuote);
    }
}
