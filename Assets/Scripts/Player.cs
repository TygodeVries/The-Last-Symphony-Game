using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public GridWalker walker;
    [SerializeField] private GameObject navigateObject;
    [SerializeField] private GameObject actionUIObject; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        battle = FindFirstObjectByType<Battle>();
        walker = GetComponent<GridWalker>();
    }

    Battle battle;
    public IEnumerator WalkTo(Tile tile)
    {
        battle.UseAction("Move");
        Debug.Log("Starting...");
        CameraSystem.SetTarget(transform);

        Debug.Log("Before Navigate");
        yield return StartCoroutine(walker.Navigate(tile));
        Debug.Log("After Navigate");

        CameraSystem.SetTarget(null);
        Debug.Log("Back to null!");
    }

    public void OpenActionGUI()
    {
        actionUIObject.SetActive(true);
        actionUIObject.GetComponent<ActionUI>().Validate();
    }

    public void PerformAction(string id)
    {
        actionUIObject.SetActive(false);

        if(id == "Move")
        {
            navigateObject.SetActive(true);
        }

        else if(id == "Attack Shoot")
        {
            AttackObjects[0].SetActive(true);
        }

        else if(id == "Attack Granade")
        {
            AttackObjects[1].SetActive(true);
        }

        else
        {
            Debug.LogError($"Unknown Action: {id}");
        }
    }

    public List<GameObject> AttackObjects = new List<GameObject>();
}
