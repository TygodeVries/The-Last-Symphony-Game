using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceFastLoad : MonoBehaviour
{
    public string name;
    public float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadLater());
    }

    IEnumerator LoadLater()
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(name);
    }
}
