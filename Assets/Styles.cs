using UnityEngine;

public class Styles : MonoBehaviour
{
    [SerializeField] public Material none;
    [SerializeField] public Material danger;

    public static Styles GetInstance()
    {
        return FindAnyObjectByType<Styles>();
    }
}


public enum TileStyle
{
    None,
    Danger
}