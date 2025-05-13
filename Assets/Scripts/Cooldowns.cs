using TMPro;
using UnityEngine;

public class Cooldowns : MonoBehaviour
{
    [SerializeField] private int GranadeCooldown;
    private int currentGranadeCooldown = -1;

    [SerializeField] private GameObject GranadeCooldownUI;
    
    public void ResetCooldown()
    {
        currentGranadeCooldown = GranadeCooldown;
    }

    public void NextRound()
    {
        if (FindAnyObjectByType<Battle>().Granade)
            currentGranadeCooldown--;
        else
            return;

        if (currentGranadeCooldown < 0)
        {
            GranadeCooldownUI.SetActive(false);
            FindAnyObjectByType<Battle>().RestoreAction("Attack Granade");
        }
        else
        {
            GranadeCooldownUI.SetActive(true);
            GranadeCooldownUI.GetComponentInChildren<TMP_Text>().text = $"Cooldown: {currentGranadeCooldown + 1}";
        }
    }
}
