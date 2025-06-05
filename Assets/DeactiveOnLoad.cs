using UnityEngine;

public class DeactiveOnLoad : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
