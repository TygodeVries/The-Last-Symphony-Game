using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioQueue : MonoBehaviour
{
    private Queue<AudioClip> clipQueue = new Queue<AudioClip>();
    public AudioClip filler;

    public AudioSource mainSource;
    public AudioSource backSource;

    public List<AudioClip> library;
    public AudioSource Thunder;

    public void PlayFromLibrary(int ind)
    {
        AddClipQueue(library[ind]);
    }

    public void PromoteToFiller(int ind)
    {
        filler = library[ind];
    }

    public bool PlayFirstClipAtStart;
    
    public void AddClipQueue(AudioClip clip)
    {
        clipQueue.Enqueue(clip);
    }

    public void ForceNext()
    {
        isRealSource = true;
    }

    public IEnumerator FadeIn(AudioSource source, float duration = 1f, float targetVolume = 1f)
    {
        float currentTime = 0f;
        source.volume = 0f;
        source.Play();

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public IEnumerator FadeOut(AudioSource source, float duration = 1f)
    {
        float currentTime = 0f;
        float startVolume = source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    private void Start()
    {
        StartCoroutine(Loop());
        if(PlayFirstClipAtStart)
        {
            PlayFromLibrary(0);
        }
    }

    bool isRealSource = true;
    public IEnumerator Loop()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (clipQueue.Count > 0)
            {
                AudioClip audioClip = clipQueue.Dequeue();
                mainSource.clip = audioClip;
                mainSource.Play();

                if (!isRealSource)
                {
                    Thunder.Play();
                    StartCoroutine(FadeOut(backSource));
                    StartCoroutine(FadeIn(mainSource));
                }

                isRealSource = true;
                yield return new WaitForSeconds(mainSource.clip.length - 1);
            }
            else
            {
                backSource.clip = filler;

                if (isRealSource)
                {
                    backSource.Play();
                    Thunder.Play();
                    StartCoroutine(FadeOut(mainSource));
                    StartCoroutine(FadeIn(backSource, targetVolume:0.5f));
                }

                isRealSource = false;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
