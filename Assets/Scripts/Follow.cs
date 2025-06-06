using UnityEngine;

public class Follow : MonoBehaviour
{
    public Vector3 offset;
    public Transform target;

    CameraMagnet[] magnets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magnets = FindObjectsByType<CameraMagnet>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 goalPos = target.position + offset;

        foreach(CameraMagnet magnet in magnets)
        {
            if(Vector3.Distance(goalPos, magnet.transform.position + magnet.offset) < magnet.Size)
            {
                goalPos = magnet.transform.position;
            }
        }

        transform.position = Vector3.Lerp(transform.position, goalPos, Time.deltaTime * 4);
    }
}
