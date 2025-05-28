using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Living : MonoBehaviour
{
    public int HealthPoints;
    public Slider shownHealthSlider;
    public Slider acualHealthSlider;

    public RawImage lines;

    public Color highliteColor;
    public Color barColor;

    public int currentPrev;
    public void SetPreview(int amount)
    {
        currentPrev = amount;
        if (amount != lastPreviewAmount)
        {
            lastPreviewAmount = amount;

            Debug.Log("Starting fade..");
            ColorBlock block = acualHealthSlider.colors;
            block.disabledColor = barColor;
            acualHealthSlider.colors = block;
        }

        shownGoal = (float)(HealthPoints - amount) / (float)max;
    }

    int lastPreviewAmount = 0;

    float accualGoal;
    float shownGoal;

    private int max;
    private void Start()
    {
        max = HealthPoints;
        shownGoal = 1;
        accualGoal = 1;

        lines.uvRect = new Rect(0, 0, max / 20, 1);
    }

    public void Update()
    {
        shownHealthSlider.value = shownGoal;
        acualHealthSlider.value = accualGoal;

        ColorBlock block = acualHealthSlider.colors;
        block.disabledColor = Color.Lerp(block.disabledColor, highliteColor, Time.deltaTime * 4);
        acualHealthSlider.colors = block;
    }

    public void Damage(int amount)
    {
        OnDamage.Invoke();
        HealthPoints -= amount;

        shownGoal = (float)HealthPoints / (float)max;
        accualGoal = (float)HealthPoints / (float)max;

        if (HealthPoints <= 0)
        {
            OnDeath.Invoke();
        }
    }
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
}
