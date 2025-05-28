using NUnit.Framework;
using System;
using UnityEngine;

public class SelectEnemy : MonoBehaviour
{
    Enemy[] enemies;
    int selected;


    GameInput.UINavActions uiInput;
    void Start()
    {
        var input = new GameInput();
        uiInput = input.UINav;
        input.Enable();

        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    public void OnApplicationQuit()
    {
        uiInput.Disable();
    }


    public GameObject ChanceText;
    public Enemy GetSelected()
    {
        return enemies[selected];
    }

    private void OnEnable()
    {
        ChanceText.SetActive(true);
        cursor.gameObject.SetActive(true);    
    }

    private void OnDisable()
    {
        ChanceText.SetActive(false);
        cursor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);


        if (uiInput.Right.WasPressedThisFrame())
            selected++;
        if (uiInput.Left.WasPressedThisFrame())
            selected--;

        if (selected < 0)
            selected = enemies.Length - 1;

        if (selected == enemies.Length)
            selected = 0;

        CameraSystem.SetTarget(enemies[selected].transform);
        cursor.transform.position = enemies[selected].transform.position + cursorOffset;

        enemies[selected].GetComponent<Living>().SetPreview(20);

        foreach (Enemy enemy in enemies)
        {
            if (enemies[selected] != enemy && enemy.GetComponent<Living>().currentPrev != 0)
            {
                enemy.GetComponent<Living>().SetPreview(0);
            }
        }
    }

    [SerializeField] private GameObject cursor;
    [SerializeField] private Vector3 cursorOffset = new Vector3(0, 1, 0);
}
