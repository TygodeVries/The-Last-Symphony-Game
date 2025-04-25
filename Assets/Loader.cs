using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
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
        yield return new WaitForSeconds(1.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(next);

        while(!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}