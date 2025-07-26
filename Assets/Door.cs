
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<GameObject> doorOpens = new List<GameObject>();
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorOpens.Add(FindAnyObjectByType<Player>().gameObject);

        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            doorOpens.Add(enemy.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        bool shouldBeOpen = false;
        foreach(GameObject opener in doorOpens)
        {
            if(Vector3.Distance(opener.transform.position, transform.position) < 1)
            {
                shouldBeOpen = true;
            }
        }


        animator.SetBool("DoorOpen", shouldBeOpen);
    }
}
