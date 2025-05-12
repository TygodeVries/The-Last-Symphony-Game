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

    float time = 1;


    private bool isWalking = false;
    void Update()
    {
        time -= Time.deltaTime;
        if(time < 0)
        {
            time = 1;
            animator.SetInteger("Random", Random.Range(0, 100));
        }

        body.linearVelocity = new Vector3(Input.GetAxis("Horizontal") * PlayerSpeed, body.linearVelocity.y, Input.GetAxis("Vertical") * PlayerSpeed);

        if(isWalking && body.linearVelocity.magnitude < 0.5f)
        {
            isWalking = false;
            animator.SetTrigger("StartWalking");
            Debug.Log("[Animator] Start Walking");
        }
        
        if (!isWalking && body.linearVelocity.magnitude > 0.5f)
        {
            isWalking = true;
            animator.SetTrigger("StopWalking");
            Debug.Log("[Animator] Stop Walking");

        }

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
