using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
   public TextMeshProUGUI dialogueText;
   
   public TextMeshProUGUI dialogueName;

    public List<Button> choiceButtons;
    public Button continueButton; 

    public DialogueNode currentNode;
    public DialogueNode gambleContinueNode;
    public HealthManager healthManager;
    //public Gamblingmechanic gambleManager;

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.02f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    
    public Image dialogueImageHolder;
    public GameObject panel;
    
    private Coroutine iconAnimationCoroutine;
    
    [Header("Judge Dialogues")]
    public List<DialogueNode> judgeEvaluation;
    
    [Header("Judge Dialogues")]
    public List<DialogueNode> judgeDialogues;

    [Header("Reactions")]
    public List<DialogueNode> playerReactionLoss;
    public List<DialogueNode> opponentReactionLoss;

    private DialogueNode resumeAfterGambleNode;

    private bool isInGambleSequence = false; // Prevents continueButton from misfiring
    
    [SerializeField] DiceRoller2D _diceRoller;
    [SerializeField] DiceRollerUI _diceRollerUI;

    [SerializeField] private float delayBeforeSceneChange = 10f;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource audioSource;
    private string nextSceneName = null;
    public bool gamblingIsOccuring { get; private set; } = false;

    void Start()
    {
        if (currentNode != null)
            DisplayNode(currentNode);
        panel.SetActive(false);
    }
    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void DisplayNode(DialogueNode node)
    {
        currentNode = node;
        
        if (dialogueName != null && currentNode.name != null)
            dialogueName.text = currentNode.name;
        else if (dialogueName != null)
            dialogueName.text = null;
        
        // Stop any previous animation
        if (iconAnimationCoroutine != null)
            StopCoroutine(iconAnimationCoroutine);

     
        if (node.animatedSprites != null && node.animatedSprites.Count > 0)
        {
            iconAnimationCoroutine = StartCoroutine(AnimateIcon(node.animatedSprites, node.animationSpeed));
        }
        else
        {
            dialogueImageHolder.sprite = null;
        }

        // Clear old button listeners and hide all choice buttons
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        panel.SetActive(false);

        typingCoroutine = StartCoroutine(TypeText(node.dialogueText));
        
        
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
        ShowButtons();
    }

    void ShowButtons()
    {
        if (currentNode.choices != null && currentNode.choices.Length > 0)
        {
            panel.SetActive(true);
            for (int i = 0; i < currentNode.choices.Length && i < choiceButtons.Count; i++)
            {
                DialogueChoice choice = currentNode.choices[i];
                Button button = choiceButtons[i];

                button.gameObject.SetActive(true);
                TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = choice.choiceText;

                button.onClick.AddListener(() => {
                    PlayClickSound();
                    OnChoiceSelected(choice);
                });
                continueButton.gameObject.SetActive(false);
            }
        }
        else
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    public void OnContinuePressed()
    {

        if (isTyping && !gamblingIsOccuring)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentNode.dialogueText;
            isTyping = false;
            ShowButtons();
        }
        else if (currentNode.nextDefaultNode != null)
        {
            DisplayNode(currentNode.nextDefaultNode);
        }
        else
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                ChangeScene(nextSceneName);
                return;
            }

            evaluateEnding();

            foreach (Button button in choiceButtons)
                button.gameObject.SetActive(false);
        }
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        // Apply the damage
        healthManager.DamagePlayer(choice.damageToPlayer);
        healthManager.DamageEnemy(choice.damageToEnemy);

        // Check updated player health
        int playerHealth = healthManager.playerHealth;
        Debug.Log("CURRENT HP: " + playerHealth);
        // Check if player passed an HP threshold (and only the player)
        bool crossedThreshold =
            (healthManager.hasPassed75 && playerHealth < 75 ) ||
            (healthManager.hasPassed50 && playerHealth < 50) ||
            (healthManager.hasPassed25 && playerHealth < 25);
        Debug.Log("MANAGER: "  + healthManager.hasPassed75 + " PLAYER HP : "   + playerHealth 
                  + " THRESHOLD CHECKER: " + crossedThreshold);
        if (crossedThreshold)
        {
            
            
            // Trigger gambling sequence
            healthManager.TriggerGamble();
            Debug.Log("I WENT OPTION A " + playerHealth);

          
            gambleContinueNode = choice.nextNode;

           
            continueButton.interactable = false;
            Gambling();

            healthManager.hasPassed75 = false;
            healthManager.hasPassed50 = false;
            healthManager.hasPassed25 = false;
            
            return;

        }

        // If no threshold crossed, continue normally
        Debug.Log("I WENT OPTION B: " + playerHealth);
        DisplayNode(choice.nextNode);
    }
    
    IEnumerator AnimateIcon(List<Sprite> sprites, float speed)
    {
        int index = 0;

        while (true)
        {
            dialogueImageHolder.sprite = sprites[index];
            index = (index + 1) % sprites.Count;
            yield return new WaitForSeconds(speed);
        }
    }

    public void Gambling()
    {
        if (currentNode.nextDefaultNode != null)
        {
            gambleContinueNode = currentNode.nextDefaultNode;
        }
        else
        {
            foreach (Button button in choiceButtons)
                button.gameObject.SetActive(false);

            continueButton.gameObject.SetActive(false);
        }
        
        DialogueNode judgeLine = judgeDialogues[Random.Range(0, judgeDialogues.Count)];
        DisplayNode(judgeLine);
        
        continueButton.interactable = false;
    }

    public void ReactionDice(bool ready)
    {
        DialogueNode reaction;
        if (ready)
        {
            reaction = playerReactionLoss[Random.Range(0, playerReactionLoss.Count)];
            DisplayNode(reaction);
        }
        else
        {
             reaction = opponentReactionLoss[Random.Range(0, opponentReactionLoss.Count)];
            DisplayNode(reaction);
        }

        reaction.nextDefaultNode = gambleContinueNode;
        continueButton.interactable = true;
        
        
        
    }

    private void evaluateEnding()
    {
        continueButton.interactable = true;
    
        if (healthManager.playerHealth > healthManager.enemyHealth) // Player Win
        {
            DisplayNode(judgeEvaluation[0]);
            nextSceneName = "Mc Wins";
        }
        else if (healthManager.playerHealth < healthManager.enemyHealth || healthManager.playerHealth == 0) // Player Lose
        {
            DisplayNode(judgeEvaluation[1]);
            nextSceneName = "Mc Loses";
        }
        else if (healthManager.playerHealth == healthManager.enemyHealth) // Stalemate
        {
            DisplayNode(judgeEvaluation[2]);
            nextSceneName = "Tie Cut Scene";
        }
    }
    
    void ChangeScene(string scenename)
    {
        SceneManager.LoadScene(scenename); 
    }

    IEnumerator changeScene(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene(sceneName);

    }
}



