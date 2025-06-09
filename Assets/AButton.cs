using UnityEngine;
using UnityEngine.UI;

public class AButton : MonoBehaviour
{
    GameInput.UINavActions uiInput;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        button = GetComponent<Button>();
        uiInput = gameInput.UINav;
    }


    Button button;
    // Update is called once per frame
    void Update()
    {
        if(uiInput.Confirm.WasPressedThisFrame())
        {
            button.onClick.Invoke();
        }
    }
}
