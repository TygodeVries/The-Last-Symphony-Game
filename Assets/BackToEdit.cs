using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class BackToEdit : MonoBehaviour
{
    GameInput.UINavActions uiInput;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;
    }

    // Update is called once per frame
    void Update()
    {
        if (uiInput.Menu.WasPressedThisFrame())
        {
            SceneManager.LoadScene("LevelEdit");
        }
    }
}
