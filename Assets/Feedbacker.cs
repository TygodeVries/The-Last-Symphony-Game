
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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

    public async void Submit()
    {
        StartCoroutine(Close());
        loadingIndicator.SetActive(true);
        string[] smiles = { ":(", ":|", ":)", ":D" };

        Guid guid = Guid.NewGuid();
        byte[] fileData = File.ReadAllBytes(filename);

        string cloudName = "de8u6k7qs";

        Cloudinary cloudinary = new Cloudinary($"cloudinary://746957665321922:UiOhpWSh0-Bw3-kjb6zDo1QTRN8@de8u6k7qs");
        cloudinary.Api.Secure = true;

        ImageUploadParams image = new ImageUploadParams()
        {
            File = new FileDescription(filename),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = false
        };

        var uploadResult = await cloudinary.UploadAsync(image);
        Console.WriteLine(uploadResult.JsonObj);

        string gameData = "Scene: " + SceneManager.GetActiveScene().name;

        SendAsync(dropDown.options[dropDown.value].text, smiles[ind], uploadResult.Url.ToString(), inputField.text, gameData);
    }

    public async void SendAsync(string type, string smile, string screenshot, string text, string game)
    {

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = System.Net.Http.HttpMethod.Post,
            RequestUri = new Uri("https://script.google.com/macros/s/AKfycbwKYvdDrEaKODFIzP66g6lmFoLwh5zjBrS_EoYmFNpHb38bnQsIcA_hCGWOYO01QCwiYQ/exec"),
            Headers =
    {
        { "User-Agent", "insomnia/11.1.0" },
    },
            Content = new StringContent($"{{\n\t\"type\": \"{type}\",\n\t\"smile\": \"{smile}\",\n\t\"screenshot\": \"{screenshot}\",\n\t\"text\": \"{text}\",\n\t\"game\": \"{game}\"\n}}")
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

        loadingIndicator.SetActive(false);
    }


    string filename;
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
        Guid guid = Guid.NewGuid();
        filename = Application.persistentDataPath + $"/{guid.ToString()}.png";

        open = true;
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log("Saving to " + filename);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        var rawData = System.IO.File.ReadAllBytes(filename);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(rawData);

        tex = ScaleTexture(tex, 1280, 720);

        File.WriteAllBytes(Application.persistentDataPath + $"/{guid.ToString()}.png", tex.EncodeToPNG());

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
