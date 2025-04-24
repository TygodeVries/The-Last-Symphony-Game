using UnityEngine;

public class Cooldowns : MonoBehaviour
{
    [SerializeField] private int GranadeCooldown;
    private int currentGranadeCooldown = -1;

    [SerializeField] private GameObject GranadeCooldownUI;
    
    public void NextRound()
    {
        currentGranadeCooldown--;

        if(currentGranadeCooldown < 0)
        {
            GranadeCooldownUI.SetActive(false);
        }
        else
        {
            GranadeCooldownUI.SetActive(true);
        }
    }
}
