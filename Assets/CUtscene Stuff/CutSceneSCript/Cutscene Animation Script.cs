using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CutsceneAnimationScript : MonoBehaviour
{
     [Header("UI Elements")]
    public RectTransform imageOne;
    public RectTransform imageTwo;
    public RectTransform imageThree;
    public RectTransform offscreenStartOne;
    public RectTransform offscreenStartTwo;
    public RectTransform offscreenStartThree;
    public GameObject Button;
    
    private Vector2 originalPosOne;
    private Vector2 originalPosTwo;
    private Vector2 originalPosThree;

    public Image coverImage;

    [Header("Dialogue UI")]
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Data")]
    public DialoguescriptableObject dialogueLine; 
    
    // Just one line

    [Header("Sound Effects")]
    public AudioSource heels;
    public AudioSource Gavel;
    public AudioSource Beep;


    
    
    [Header("Animation Settings")]
    public float slideDuration = 1f;
    public float delayBetweenSlides = 2f;
    public float typeSpeed = 0.05f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
       
        originalPosOne = imageOne.anchoredPosition;
        originalPosTwo = imageTwo.anchoredPosition;
        originalPosThree = imageThree.anchoredPosition;
    
     
        dialogueBox.SetActive(false);
        Button.SetActive(false);
        imageOne.anchoredPosition = offscreenStartOne.anchoredPosition;
        imageTwo.anchoredPosition = offscreenStartTwo.anchoredPosition;
        imageThree.anchoredPosition = offscreenStartThree.anchoredPosition;
    
      
        Invoke(nameof(FadeOutCover), 6f);
    }
    
    void FadeOutCover()
    {
        if (coverImage != null)
        {
            LeanTween.alpha(coverImage.rectTransform, 0f, 1f).setOnComplete(() =>
            {
                coverImage.gameObject.SetActive(false);
                SlideImageOne();
                dialogueBox.SetActive(true);
                DisplayDialogueLine();
            });
        }
        else
        {
          
            SlideImageOne();
        }
    }
    
    void SlideImageOne()
    {
        heels.Play();
        dialogueBox.SetActive(true);
        LeanTween.move(imageOne, originalPosOne, slideDuration).setEaseOutExpo().setOnComplete(() =>
        {
            Invoke(nameof(SlideImageTwo), delayBetweenSlides);
        });
    }
    
    void SlideImageTwo()
    {
        heels.Stop();
        Beep.Play();
        LeanTween.move(imageTwo, originalPosTwo, slideDuration).setEaseOutExpo().setOnComplete(() =>
        {
            Invoke(nameof(SlideImageThree), delayBetweenSlides);
        });
    }

    public void NextScene()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("this will go into the battle scene");
    }
    
    void SlideImageThree()
    {
        Beep.Stop();
        Gavel.Play();
        LeanTween.move(imageThree, originalPosThree, slideDuration).setEaseOutExpo();
        Button.SetActive(true);
        
    }
    void DisplayDialogueLine()
    {
        if (dialogueLine == null)
        {
            Debug.LogWarning("No dialogue line assigned");
            return;
        }

        speakerNameText.text = dialogueLine.SpeakerName;
        typingCoroutine = StartCoroutine(TypeText(dialogueLine.dialogueText));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }
}
