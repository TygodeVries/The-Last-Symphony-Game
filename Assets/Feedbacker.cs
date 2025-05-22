
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Feedbacker : MonoBehaviour
{
    public List<Button> smilies;
    public GameObject ui;

    public RawImage screenShotImage;
    public void OnEnable()
    {
       
    }

    public TMP_InputField inputField;
    public TMP_Dropdown dropDown;

    public void Submit()
    {
        string[] smiles = { ":(", ":|", ":)", ":D" };

        string filename = Application.persistentDataPath + "/feedback_screenshot.png";
        byte[] fileData = File.ReadAllBytes(filename);

        Guid guid = Guid.NewGuid();

        

        SendAsync(dropDown.itemText.text, smiles[ind], $"{guid.ToString()}.png", inputField.text);
    }

    public async void SendAsync(string type, string smile, string screenshot, string text)
    {

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://script.google.com/macros/s/AKfycbzWygH_6hyPcYjGx18TpnySeY68xRVvKBQjbndKiRjd3vx8Fw9oqrtiEg4em-sbC-FIYg/exec"),
            Headers =
    {
        { "User-Agent", "insomnia/11.1.0" },
    },
            Content = new StringContent($"{{\n\t\"type\": \"{type}\",\n\t\"smile\": \"{smile}\",\n\t\"screenshot\": \"{screenshot}\",\n\t\"text\": \"{text}\"\n}}")
            {
                Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
            }
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Debug.Log(body);
        }
    }

    public void Open()
    {
        if (!open)
            StartCoroutine(TakeScreen());
        else
            StartCoroutine(Close());
    }

    bool open = false;

    public IEnumerator Close()
    {
        open = false;
        yield return new WaitForEndOfFrame();
        ui.SetActive(false);
    }

    public IEnumerator TakeScreen()
    {
        open = true;
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/feedback_screenshot.png");
        Debug.Log("Saving to " + Application.persistentDataPath + "/feedback_screenshot.png");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        string filename = Application.persistentDataPath + "/feedback_screenshot.png";
        var rawData = System.IO.File.ReadAllBytes(filename);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(rawData);

        tex = ScaleTexture(tex, 1280, 720);

        File.WriteAllBytes(Application.persistentDataPath + "/feedback_screenshot.png", tex.EncodeToPNG());

        screenShotImage.texture = tex;
        ui.SetActive(true);
    }
    public int ind;
    public void Select(int i)
    {
        for (int m = 0; m < smilies.Count; m++)
        {
            smilies[m].interactable = true;
        }

        ind = i;

        smilies[i].interactable = false;
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }
}
