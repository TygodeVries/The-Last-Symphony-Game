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
        RestoreAction("Move");

        RestoreAction("Attack Shoot");

        RestoreAction("End");
       
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

        RestoreAction("Attack Granade");
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
