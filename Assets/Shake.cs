using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shake : MonoBehaviour
{
    public float left;
    public float right;
    public float time;
    public float delay;
    public void Do()
    {
        StartCoroutine(DoStuffAsync());
    }

    private IEnumerator DoStuffAsync()
    {
        yield return new WaitForSeconds(delay);
        Gamepad.current.SetMotorSpeeds(left, right);
        yield return new WaitForSeconds(time);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
}
