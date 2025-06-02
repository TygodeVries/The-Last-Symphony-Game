using UnityEngine;
using UnityEngine.Events;

public class Continue : MonoBehaviour
{ 
    public void DoContinue()
    {
        OnContinue.Invoke();
    }

    public void Update()
    {
        if (uiInput.Confirm.WasPressedThisFrame())
            DoContinue();
    }

    GameInput.UINavActions uiInput;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;
    }


    public UnityEvent OnContinue;
}
