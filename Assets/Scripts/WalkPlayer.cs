using UnityEngine;

public class WalkPlayer : MonoBehaviour
{

    [SerializeField] private float PlayerSpeed;
    private Rigidbody body;
    private Camera cam;

    private Animator animator;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update()
    {
        body.linearVelocity = new Vector3(Input.GetAxis("Horizontal") * PlayerSpeed, body.linearVelocity.y, Input.GetAxis("Vertical") * PlayerSpeed);

        animator.SetBool("IsWalking", body.linearVelocity.magnitude > 0.5f);

        if (body.linearVelocity.magnitude > 0.5f)
        {
            float f = Mathf.Atan2(body.linearVelocity.z, body.linearVelocity.x);

            f *= Mathf.Rad2Deg;

            cur = Mathf.LerpAngle(cur, f, Time.deltaTime * 5);

            Debug.Log(cur);

            float x = Mathf.Cos((cur / 180f) * Mathf.PI);
            float y = Mathf.Sin((cur / 180f) * Mathf.PI);

            transform.forward = new Vector3(x, 0, y);
        }
    }

    float cur = 0;
}
