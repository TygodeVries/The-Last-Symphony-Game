using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    [SerializeField] private GameObject[] uiItems;

    private List<GameObject> activeUIItems;
    private int index;

    private GameObject lastGlow;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            index++;
        if (Input.GetKeyDown(KeyCode.Q))
            index--;

        if(index < 0)
            index = activeUIItems.Count - 1;

        if(index == activeUIItems.Count)
            index = 0;

        if(lastGlow != null)
            lastGlow.GetComponent<Outline>().enabled = false;
        activeUIItems[index].GetComponent<Outline>().enabled = true;
        lastGlow = activeUIItems[index];

        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnSelectionMade.Invoke(lastGlow.name);
        }
    }

    [SerializeField] private UnityEvent<string> OnSelectionMade;

    public void Validate()
    {
        Battle battle = FindFirstObjectByType<Battle>();

        activeUIItems = uiItems.ToList();

        foreach (GameObject g in uiItems) {
            if(!battle.HasAction(g.name))
            {
                activeUIItems.Remove(g);
            }

            g.SetActive(battle.HasAction(g.name));
        }
    }
}
