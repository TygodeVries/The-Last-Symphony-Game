using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TileBehaviorEvent : TileBehavior
{
    [SerializeField] private float EventLenght = 0f;
    [SerializeField] private UnityEvent OnStep;

    public override IEnumerator Step()
    {
        OnStep.Invoke();
        yield return new WaitForSeconds(EventLenght);
    }
}
