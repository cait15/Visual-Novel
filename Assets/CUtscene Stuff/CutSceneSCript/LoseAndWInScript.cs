using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseAndWInScript : MonoBehaviour
{
     [Header("Images")]
    public Image image1; 
    public Image image2; 
    public Image image3; 

    [Header("Dialogue UI")]
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("ScriptableObjects")]
    public DialoguescriptableObject[] cutsceneLines;

    [Header("Timing Settings")]
    public float startDelay = 3f;
    public float fadeDuration = 1f;
    public float typingSpeed = 0.05f;

    private int currentLineIndex = 0;
    private bool isTyping = false;

    public bool IsLose;

    public AudioSource acceptedcall;
    public AudioSource BuzzClip;
    


    private void Awake()
    {
        if (image1 == null)
        {
            image1 = GameObject.Find("Frame 1")?.GetComponent<Image>();
            if (image1 == null)
                Debug.LogError("FrameOne not found or missing Image component");
        }
        
        if (image2 == null)
        {
            image2 = GameObject.Find("Frame 2")?.GetComponent<Image>();
            if (image2 == null)
                Debug.LogError("FrameOne not found or missing Image component");
        }
        
        if ( image3 == null)
        {
            image3 = GameObject.Find("Frame 3")?.GetComponent<Image>();
            if ( image3 == null)
                Debug.LogError("FrameOne not found or missing Image component");
        }
    }

    void Start()
    {
        
        SetAlpha(image1, 1f);
        SetAlpha(image2, 1f);
        SetAlpha(image3, 1f);
        dialogueBox.SetActive(false);

        Invoke(nameof(FadeOutImage1), startDelay);

        if (IsLose)
        {
            BuzzClip.Play();
        }
       
    }

    void FadeOutImage1()
    {
        if (IsLose)
        {
            BuzzClip.Stop();
        }
        
        acceptedcall.Play();
        LeanTween.alpha(image1.rectTransform, 0f, fadeDuration).setOnComplete(() =>
        {
            SetAlpha(image2, 1f);
            dialogueBox.SetActive(true);
            DisplayNextLine();
        });
    }

    public void DisplayNextLine()
    {
     
        if (isTyping)
        {
            StopAllCoroutines();
            var fullLine = cutsceneLines[currentLineIndex].dialogueText;
            dialogueText.text = fullLine;
            isTyping = false;
            currentLineIndex++; 
            return;
        }

        
        if (currentLineIndex >= cutsceneLines.Length)
        {
            Debug.Log("End of dialogue.");
            
        }

        if (currentLineIndex < cutsceneLines.Length)
        {
             var currentLine = cutsceneLines[currentLineIndex];
             speakerNameText.text = currentLine.SpeakerName;
             StartCoroutine(TypeText(currentLine.dialogueText));
        }
        else 

        {
            SceneManager.LoadScene("HomeScreen");
        }

    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];

          
            if (currentLineIndex == cutsceneLines.Length - 1 && i == 0)
            {
                LeanTween.alpha(image2.rectTransform, 0f, fadeDuration).setOnComplete(() =>
                {
                    SetAlpha(image3, 1f);
                    
                });
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        currentLineIndex++;
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    
}
