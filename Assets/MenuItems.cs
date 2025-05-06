using UnityEngine;

public class MenuItems : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        Camera.main.GetComponent<Animator>().SetBool("Settings", true);
    }
}
