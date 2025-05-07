using UnityEngine;

public class WalkPlayer : MonoBehaviour
{
    private Rigidbody body;
    private Camera cam;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update()
    {
        body.linearVelocity = new Vector3(Input.GetAxis("Horizontal"), -5, Input.GetAxis("Vertical")) * 4;
    }
}
