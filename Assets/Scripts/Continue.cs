using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{ 
    public void DoContinue()
    {
        OnContinue.Invoke();

        if(inLevelEditor)
        {
            SceneManager.LoadScene("LevelEdit");
        }
    }

    public void Update()
    {
        if (uiInput.Confirm.WasPressedThisFrame())
            DoContinue();
    }


    bool inLevelEditor;
    GameInput.UINavActions uiInput;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();

        uiInput = gameInput.UINav;

        if(OnContinue.GetPersistentEventCount() == 0)
        {
            inLevelEditor = true;
            Debug.LogWarning("Level Editor Detected!");
        }
    }


    public UnityEvent OnContinue;
}
