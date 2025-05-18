using System;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class DialogAction
{
    public DialogActionType actionType;
}

public enum DialogActionType
{

}