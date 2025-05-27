using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour
{
    [Header("Enabled Actions")]
    public bool Move;
    public bool Shoot;
    public bool Granade;

    private List<string> availablePlayerActions = new List<string>();
    public void ResetActions()
    {
        if(Move)
            RestoreAction("Move");

        if(Shoot)
            RestoreAction("Attack Shoot");
    }

    private void Update()
    {
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in tiles)
        {
            tile.RenderSelected();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    public void RestoreAction(string action)
    {
        if (!availablePlayerActions.Contains(action))
            availablePlayerActions.Add(action);
    }

    public void CheckForEndOfGame()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if(enemies.Length == 0)
        {
            StartCoroutine(StartVictory());
        }

    }

    public void PlayerDeath()
    {
        StartCoroutine(StartLoss());
    }
    private IEnumerator StartLoss()
    {
        ToolTip.Set("<sprite name=\"b\"> Continue");
        if (GameOver)
            yield break;

        GameOver = true;
        loseCanvas.SetActive(true);
    }


    public GameObject loseCanvas;
    public GameObject winCanvas;
    public IEnumerator StartVictory()
    {
        ToolTip.Set("<sprite name=\"b\"> Continue");
        if (GameOver)
            yield break;
        winCanvas.SetActive(true);
        GameOver = true;

        FindAnyObjectByType<Player>().GetComponentInChildren<Animator>().SetTrigger("Win");

        yield return new WaitForSeconds(1);
    }

    public bool GameOver = false;

    public bool HasAction(string id)
    {

        bool result = availablePlayerActions.Contains(id);
        return result;
    }

    public void UseAction(string id)
    {
        availablePlayerActions.Remove(id);
    }

    private Player player;
    [SerializeField] private TMP_Text NotificationBar;

    public void Start()
    {
        player = FindAnyObjectByType<Player>();
        this.InvokeRepeating("CheckForEndOfGame", 1, 0.5f);

        if(Granade)
            RestoreAction("Attack Granade");

        if (EnemyStartsBattle)
        {
            StartCoroutine(StartEnemyTurn());
        }
        else
            StartPlayerTurn();

    }

    [SerializeField] private bool EnemyStartsBattle = false;

    [SerializeField] private UnityEvent PlayerTurnStarts;

    public void StartPlayerTurn()
    {
        CheckForEndOfGame();
        if (GameOver)
            return;
        ToolTip.Set("<sprite name=\"a\"> Select\n<sprite name=\"Inputs_1\"> Navigate");
        ResetActions();

        PlayerTurnStarts.Invoke();
        Notification.SetText("Your Turn", 1.5f);
        CameraSystem.SetTarget(null);
        player.OpenActionGUI();
    }

    public IEnumerator StartEnemyTurn()
    {
        if (GameOver)
            yield break;

        ToolTip.Set("");
        Notification.SetText("Ghost Turn", 1.5f);
        Enemy[] enemies = UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            yield return enemy.TakeTurn();
            yield return new WaitForSeconds(0.1f);
        }

        StartPlayerTurn();
    }
}
