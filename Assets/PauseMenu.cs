using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool MenuOpen;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpen = !MenuOpen;

            if(MenuOpen)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                Time.timeScale = 0.0f;
            }

            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
    }
}
