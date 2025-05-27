using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Living : MonoBehaviour
{
    public int HealthPoints;
    public TMP_Text healthText;

    private void Start()
    {
        if (healthText != null)
        {
            healthText.text = HealthPoints + "hp";
        }
    }

    public void Damage(int amount)
    {
        OnDamage.Invoke();
        HealthPoints -= amount;

        if(healthText != null)
        {
            healthText.text = HealthPoints + "hp";
        }

        if (HealthPoints <= 0)
        {
            OnDeath.Invoke();
        }
    }
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
}
