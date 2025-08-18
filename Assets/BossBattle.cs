using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattle : MonoBehaviour
{

    public IEnumerator SetHeight(float height, float time)
    {
        float elapsed = 0f;
        float duration = (float)time;
        float startHeight = env.transform.position.y;

        while (elapsed < duration)
        {
            float t = FallCurve(elapsed / duration);

           env.transform.position = new Vector3(0,Mathf.Lerp(startHeight, height, t), 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public Transform env;
    public IEnumerator RotateTo(float tilt, float rotation, float spin, double time)
    {
        float elapsed = 0f;
        float duration = (float)time;

        float startRotation = this.rotation;
        float startSpin = this.spin;
        float startTilt = this.tilt;
       
        while (elapsed < duration)
        {
            float t = Curve(elapsed / duration);

            this.rotation = Mathf.Lerp(startRotation, rotation, t);
            this.tilt = Mathf.Lerp(startTilt, tilt, t);
            this.spin = Mathf.Lerp(startSpin, spin, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        this.rotation = rotation;
        this.tilt = tilt;
        this.spin = spin;
    }


    public float rotation = 0;
    public float tilt = 0;
    public float spin = 0;

    private void Update()
    {
        env.transform.eulerAngles = new Vector3(tilt, rotation, spin);

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(RotateTo(10, 0, 0, 3));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(RotateTo(0, 0, 0, 4));
            StartCoroutine(SetHeight(2.8f, 4));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(RotateTo(0, 0, 0, 4));
            StartCoroutine(SetHeight(0, 4));
        }
    }

    private float Curve(float x)
    { 
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    private float FallCurve(float x)
    {
        return 1 - Mathf.Pow(1 - x, 4);
    }

    public void NextTurn()
    {
        StartCoroutine(RotateTo(0, rotation + 90, 0, 3));
    }
}


