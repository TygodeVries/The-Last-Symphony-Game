using UnityEngine;

public class CheatCode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public GameObject levelWinCanvas;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.U) && Input.GetKeyDown(KeyCode.T))
        {
            levelWinCanvas.SetActive(true);
        }
    }
}
