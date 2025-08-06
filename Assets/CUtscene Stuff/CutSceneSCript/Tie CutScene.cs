using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TieCutScene : MonoBehaviour
{
  
    [Header("Images to fade between (make sure all are the same size and stacked)")]
    public Image FrameOne;
    public Image FrameTwo;
    public AudioSource Heels;
    public GameObject button;
   

    [Header("Timing Settings")]
    public float startDelay = 2f;
    public float fadeDuration = 1f;
    public float delayBetweenFades = 2f;

    private void Awake()
    {
        if (FrameOne == null)
        {
            FrameOne = GameObject.Find("Frame 1")?.GetComponent<Image>();
            if (FrameOne == null)
                Debug.LogError("FrameOne not found or missing Image component");
        }
        
        if (FrameTwo == null)
        {
            FrameTwo = GameObject.Find("Frame 2")?.GetComponent<Image>();
            if (FrameTwo == null)
                Debug.LogError("FrameOne not found or missing Image component");
        }
    }

    void Start()
    {
      
      
        StartCoroutine(DelayedFadeSequence());
        StartCoroutine(StopSoundAfterDelay(Heels, 7f));
    }

    
    public IEnumerator StopSoundAfterDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
    }
    void FadeImage1()
    {
        LeanTween.alpha(FrameOne.rectTransform, 0f, fadeDuration).setOnComplete(() =>
        {
       
            Invoke(nameof(FadeImage2), delayBetweenFades);
        });
    }
    
    IEnumerator DelayedFadeSequence()
    {
        
        yield return new WaitForSeconds(startDelay + 0.5f); 

        if (FrameOne == null)
        {
            Debug.LogError("FrameOne is not assigned");
            yield break;
        }

        FadeImage1();
    }

    void FadeImage2()
    {
        button.SetActive(true);
        LeanTween.alpha(FrameTwo
            .rectTransform, 0f, fadeDuration).setOnComplete(() =>
        {
          
        });
    }

    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
