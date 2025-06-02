using UnityEngine;

public class Experiments : MonoBehaviour
{
    [SerializeField] private bool disableTimingGame;

    public static bool disableTimingGameS;

    public static Experiments instance;

    public void Update()
    {
        disableTimingGameS = disableTimingGame;
    }
}
