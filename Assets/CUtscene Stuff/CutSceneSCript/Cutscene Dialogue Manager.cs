using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneDialogueManager : MonoBehaviour
{
    [Header("Dialogue Queuwu")]
    [SerializeField] private DialoguescriptableObject CurrentDialogue;
    [SerializeField] private TextMeshProUGUI SpeakerName;
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private HolderDialogue sceneDialogue;
    private int indexListTracker;
    private bool isTyping;
    private DialogueQueue<DialoguescriptableObject> dialogueQueue = new DialogueQueue<DialoguescriptableObject>();
   
   
  
    private void Awake()
    {
        EnqueueDialogue();
    }

    private void EnqueueDialogue()
    {
        for (int i = 0; i < sceneDialogue.Script.Count; i++)
        {
            dialogueQueue.Enqueue(sceneDialogue.Script[i]);
        }
    }
    void Start()
    {
        CutSceneDisplayDialogue();
      
    }
    public void CutSceneDisplayDialogue()
    {
        if (dialogueQueue.isEmpty() == false)
        {
            CurrentDialogue = dialogueQueue.Dequeue();
            SpeakerName.text = CurrentDialogue.SpeakerName;
         

            StartCoroutine(TypeWritterEffect(CurrentDialogue.dialogueText));
            
        }
    }

    public void TEstButton()
    {
        Debug.Log("TEstButton");
    }
    public void NextDialogue()
    {
       Debug.Log("button works");
        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            DialogueText.text = CurrentDialogue.dialogueText;
            isTyping = false;
            Debug.Log("testing if this works");
            Debug.Log(isTyping);
            
            return;
        }
        if (!dialogueQueue.isEmpty() && isTyping == false)
        {
            CutSceneDisplayDialogue();
        }
        else if (dialogueQueue.isEmpty())
        {
           Debug.Log("SCeneEnded");
           SceneManager.LoadScene("HomeScreen");
        }
    }
    private IEnumerator TypeWritterEffect(string spokenText)
    {
        isTyping = true;
        DialogueText.text = "";
        for (int i = 0; i < spokenText.Length; i++)
        {
            DialogueText.text += spokenText[i];
            yield return new WaitForSeconds(0.1f);
        }
        isTyping = false;
    }
}
