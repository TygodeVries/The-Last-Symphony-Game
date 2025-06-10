using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;

        DontDestroyOnLoad(this);
    }

    GameInput.UINavActions uiInput;

    public Selectable menuKey;

    public bool MenuOpen;

    // Update is called once per frame
    void Update()
    {
        if(uiInput.Menu.WasPressedThisFrame())
        {
            menuKey.Select();
            MenuOpen = !MenuOpen;

            if(MenuOpen)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                Time.timeScale = 0.0f;
            }

            else
            {
                CloseMenu();
            }
        }
    }

    public void CloseMenu()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
