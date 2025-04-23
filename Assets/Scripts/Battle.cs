using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour
{
    private List<string> availablePlayerActions = new List<string>();
    public void ResetActions()
    {
        availablePlayerActions.Clear();

        availablePlayerActions.Add("Move");
        availablePlayerActions.Add("Attack Shoot");
        availablePlayerActions.Add("End");
        availablePlayerActions.Add("Attack Granade");
    }

    public void CheckForEndOfGame()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if(enemies.Length == 0)
        {
            SceneManager.LoadScene("Victory");
        }

        Player[] player = FindObjectsByType<Player>(sortMode: FindObjectsSortMode.None);

        if (player.Length == 0)
        {
            SceneManager.LoadScene("Loss");
        }
    }

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

        StartPlayerTurn();
    }

    [SerializeField] private UnityEvent PlayerTurnStarts;

    public void StartPlayerTurn()
    {

        ResetActions();

        PlayerTurnStarts.Invoke();
        StartCoroutine(TitleInOut("Player Turn", 1.5f));
        CameraSystem.SetTarget(null);
        player.OpenActionGUI();
    }

    public IEnumerator TitleInOut(string msg, float time)
    {
        NotificationBar.text = msg;
        yield return new WaitForSeconds(time);
        NotificationBar.text = "";
    }

    public IEnumerator StartEnemyTurn()
    {
        StartCoroutine(TitleInOut("Enemy Turn", 1.5f));
        Enemy[] enemies = UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            yield return enemy.TakeTurn();
            yield return new WaitForSeconds(0.1f);
        }

        StartPlayerTurn();
    }
}
