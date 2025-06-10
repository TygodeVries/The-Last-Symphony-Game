using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ChangeInEditor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
    }


    private TMP_Text text;
    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "LevelEdit")
        {
            text.text = "Save & Play";
        }
        else
        {
            text.text = "Retry";
        }
    }

    public void HandleClick()
    {
        if (SceneManager.GetActiveScene().name == "LevelEdit")
        {
            FindAnyObjectByType<Cursor>().SaveAndPlay();
            FindAnyObjectByType<PauseMenu>().CloseMenu();
        }
        else
        {
            normalHandle.Invoke();
            FindAnyObjectByType<PauseMenu>().CloseMenu();
        }
    }

    public UnityEvent normalHandle;
}
