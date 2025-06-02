using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraSystems : MonoBehaviour
{
    public List<Vector3> postisions;

    int i = 0;
    public void Next()
    {
        GetComponent<CameraSystem>().defaultPos = postisions[i];
        i++;
    }
}
