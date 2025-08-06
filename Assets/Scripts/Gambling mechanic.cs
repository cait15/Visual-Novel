using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Gamblingmechanic : MonoBehaviour
{
    [Header("References")]
    public HealthManager healthManager;
    public DialogueManager dialogueManager;

    [Header("UI Elements")]
    public GameObject gamblePanel;
    public Text diceResultText;
    public Button rollDiceButton;

    [Header("Judge Dialogues")]
    public List<DialogueNode> judgeDialogues;

    [Header("Reactions")]
    public List<DialogueNode> playerReactionLoss;
    public List<DialogueNode> opponentReactionLoss;

    private DialogueNode resumeAfterGambleNode;

    private void Start()
    {
        rollDiceButton.onClick.AddListener(RollDice);
        gamblePanel.SetActive(false);
    }

    public void TriggerGamble(DialogueNode currentNode)
    {
        // Store where to resume
        resumeAfterGambleNode = currentNode.nextDefaultNode;

        // Pick a random judge line
        DialogueNode judgeLine = judgeDialogues[Random.Range(0, judgeDialogues.Count)];
        dialogueManager.DisplayNode(judgeLine);

        // Delay before showing the gamble panel
        Invoke(nameof(OpenGamblePanel), 2.5f);
    }

    private void OpenGamblePanel()
    {
        gamblePanel.SetActive(true);
        diceResultText.text = "Click to roll the dice!";
        rollDiceButton.interactable = true;
    }

    private void RollDice()
    {
        rollDiceButton.interactable = false;

        int roll = Random.Range(1, 7); // 1 to 6
        int damage = CalculateDamage(roll);
        string resultMessage = $"You rolled a {roll}! ";

        if (roll % 2 == 0)
        {
            // Even roll - damage enemy
            healthManager.DamageEnemy(damage);
            resultMessage += $"Enemy takes {damage} damage!";
            DialogueNode oppLossReaction = opponentReactionLoss[Random.Range(0, opponentReactionLoss.Count)];
            dialogueManager.DisplayNode(oppLossReaction);
        }
        else
        {
            // Odd roll - damage player
            healthManager.DamagePlayer(damage);
            resultMessage += $"You take {damage} damage!";
            DialogueNode mcLossReaction = playerReactionLoss[Random.Range(0, playerReactionLoss.Count)];
            dialogueManager.DisplayNode(mcLossReaction);
        }

        diceResultText.text = resultMessage;

   
        Invoke(nameof(ResumeMainDialogue), 3f);
    }

    private int CalculateDamage(int roll)
    {
        switch (roll)
        {
            case 1: return 2;
            case 2: return 4;
            case 3: return 4;
            case 4: return 6;
            case 5: return 6;
            case 6: return 8;
            default: return 2;
        }
    }

    private void ResumeMainDialogue()
    {
        gamblePanel.SetActive(false);
        if (resumeAfterGambleNode != null)
        {
            dialogueManager.DisplayNode(resumeAfterGambleNode);
        }
    }
}
