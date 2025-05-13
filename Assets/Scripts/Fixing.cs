using UnityEditor;
using UnityEngine;

public class Fixing : MonoBehaviour
{
    private void Start()
    {
        foreach(Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if(enemy.DebugMode)
            {
#if UNITY_EDITOR
                bool result = EditorUtility.DisplayDialog(
                    "Hello!",
                    "Hey Kelvin, just want you to know that you are starting the game while something still has debug mode on.",
                    "OKAY JUST RUN DAMN IT",
                    "Woops"
                );

                if (result)
                {
                    bool a = EditorUtility.DisplayDialog(
                        "Hello again!",
                        "Just want you to know that this is what is broken :D",
                        "Okay",
                        "Sorry!"
                    );
                }


#endif

            }
        }
    }
}
