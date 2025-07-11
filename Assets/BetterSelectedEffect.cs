using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetterSelectedEffect : MonoBehaviour
{
    Button button;
    TMP_Text buttonText;
    public void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();
        selectedSwitch = new Gradient();

        // Create the gradient to flash
        selectedSwitch.colorKeys = new GradientColorKey[] {

            new GradientColorKey(Color.green, 0),
            new GradientColorKey(Color.white, 1)
        };
    }


    private Gradient selectedSwitch;
    private void Update()
    {
        const float FlashSpeed = 10;
        bool isThisButtonSelected = EventSystem.current.currentSelectedGameObject == this.gameObject;

        if (isThisButtonSelected)
        {
            buttonText.color = selectedSwitch.Evaluate((Mathf.Cos(Time.time * FlashSpeed) / 2f) + 0.5f);
        }
        else
        {
            buttonText.color = Color.white;
        }
    }
}
