using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        AudioListener.volume = 1.0f;
        animator = GetComponent<Animator>();
    }


    public float startDelay;
    public void Goto(string next)
    {
        StartCoroutine(LoadScene(next, startDelay));
    }

    public IEnumerator LoadScene(string next, float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        animator.SetTrigger("Load");

        for (int i = 0; i < 15; i++)
        {
            AudioListener.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        

        AsyncOperation operation = SceneManager.LoadSceneAsync(next);

        while(!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}