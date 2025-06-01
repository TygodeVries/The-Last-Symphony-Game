using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class Dialog : MonoBehaviour
{
    public List<DialogAction> dialog;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject dialogBox;
    public IEnumerator StartDialog()
    {
        int ind = 0; yield return new WaitForEndOfFrame();

        while (ind < dialog.Count)
        {
            DialogAction action = dialog[ind];

            Debug.Log("Starting dialog action: " + action.actionType);
            if(action.actionType == DialogActionType.NOTIFICATION)
            {
                Notification.SetText(action.stringArgument, action.numberArgument);
            }

            else if(action.actionType == DialogActionType.WAIT_TILL_TURN)
            {
                yield return new WaitUntil(() => { return turnThisFrame; });
            }

            else if (action.actionType == DialogActionType.WAIT)
            {
                yield return new WaitForSeconds(action.numberArgument);
            }

            else if (action.actionType == DialogActionType.DIALOG)
            {
                StartCoroutine(SetDialog(action.stringArgument, action.numberArgument));
            }
                turnThisFrame = false;
            yield return new WaitForEndOfFrame();
            ind++;
        }
    }

    public IEnumerator SetDialog(string text, float time)
    {
        dialogBox.SetActive(true);
        Time.timeScale = 0;
        for (int i = 0; i < text.Length; i++)
        {
                yield return new WaitForSecondsRealtime(0.05f);
            dialogText.text += text[i];
        }


        yield return new WaitUntil(() =>
        {
            return uiInput.Back.WasPressedThisFrame();
        });


        Time.timeScale = 1;
        dialogText.text = "";
        dialogBox.SetActive(false);
    }

    private bool turnThisFrame;
    public void PlayerTurnStarted()
    {
        turnThisFrame = true;
    }

    GameInput.UINavActions uiInput;
    private void Start()
    {
        var gameInput = new GameInput();
        gameInput.Enable();
        uiInput = gameInput.UINav;

        StartCoroutine(StartDialog());

    }
}

[Serializable]
public class DialogAction
{
    public DialogActionType actionType;
    public string stringArgument;
    public float numberArgument;
}

public enum DialogActionType
{
    NOTIFICATION,
    WAIT_TILL_TURN,
    WAIT,
    DIALOG
}