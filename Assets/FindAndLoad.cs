using UnityEngine;
using UnityEngine.SceneManagement;

public class FindAndLoad : MonoBehaviour
{
    public string sceneName;
    public void DoNow()
    {
        if(sceneName == "this")
        {
            FindAnyObjectByType<Loader>().Goto(SceneManager.GetActiveScene().name);
        }

        Debug.Log("Loading scene: " + sceneName + ".... " + FindAnyObjectByType<Loader>());
        FindAnyObjectByType<Loader>().Goto(sceneName);

        FindAnyObjectByType<PauseMenu>().CloseMenu();
    }
}
