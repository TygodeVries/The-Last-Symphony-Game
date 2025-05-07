using UnityEngine;

public class WalkPlayer : MonoBehaviour
{

    [SerializeField] private float PlayerSpeed;
    private Rigidbody body;
    private Camera cam;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update()
    {
        body.linearVelocity = new Vector3(Input.GetAxis("Horizontal") * PlayerSpeed, body.linearVelocity.y, Input.GetAxis("Vertical") * PlayerSpeed);
    }
}
