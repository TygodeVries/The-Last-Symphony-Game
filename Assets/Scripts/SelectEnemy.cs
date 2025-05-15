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


    public Enemy GetSelected()
    {
        return enemies[selected];
    }

    private void OnEnable()
    {
        cursor.gameObject.SetActive(true);    
    }

    private void OnDisable()
    {
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

    }

    [SerializeField] private GameObject cursor;
    [SerializeField] private Vector3 cursorOffset = new Vector3(0, 1, 0);
}
