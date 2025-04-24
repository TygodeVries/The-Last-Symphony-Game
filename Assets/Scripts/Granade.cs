using UnityEngine;

public class Granade : MonoBehaviour
{
    Camera cam;
    Player player;
    public void Start()
    {
        cam = Camera.main;
        player = FindAnyObjectByType<Player>();
        transform.localPosition = new Vector3();
    }

    public void Update()
    {
        Vector3 a = cam.transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        a.y = 0;
        a.Normalize();
        transform.position += a * Time.deltaTime;
    }
}
