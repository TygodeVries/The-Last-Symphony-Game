using UnityEngine;

public class Billboard : MonoBehaviour
{

    private Camera cam;
    void Update()
    {
        transform.LookAt(cam.transform.position);
        transform.Rotate(0, 180, 0);
    }

    private void Start()
    {
        cam = Camera.main;
    }
}
