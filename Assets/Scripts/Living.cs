using TMPro;
using UnityEngine;

public class Living : MonoBehaviour
{
    public int HealthPoints;
    public TMP_Text healthText;

    private void Start()
    {
        if (healthText != null)
        {
            healthText.text = HealthPoints + "HP";
        }
    }

    public void Damage(int amount)
    {
        HealthPoints -= amount;

        if(healthText != null)
        {
            healthText.text = HealthPoints + "HP";
        }

        if (HealthPoints <= 0)
            Destroy(this.gameObject);
    }
}
