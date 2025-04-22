using UnityEngine;

public class InputRotation : MonoBehaviour
{
    public TileDirection facing = TileDirection.XPositive;

    Camera mainCamera;
    private void Start()
    {
        facing = TileDirection.XPositive;
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input.magnitude < 0.5)
            return;

        Vector3 motion = mainCamera.transform.rotation * new Vector3(input.x, 0, input.y);
        motion.y = 0;
        motion.Normalize();

        if (motion.x > 0.9)
            facing = TileDirection.XPositive;

        if (motion.x < -0.9)
            facing = TileDirection.XNegative;

        if (motion.z > 0.9)
            facing = TileDirection.ZPositive;

        if (motion.z < -0.9)
            facing = TileDirection.ZNegative;
    }
}
