using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollerUI : MonoBehaviour
{
    [SerializeField] Button _rollButton;
    [SerializeField] TMP_Text _resultsText, _doublesText;
    [SerializeField] DiceRoller2D _diceRoller;
    [SerializeField] GameObject gamblingPanel;
    [SerializeField] float closeDelay = 5f;

    void OnEnable()
    {
        _rollButton.onClick.AddListener(RollDice);
        _diceRoller.OnRoll += HandleRoll;
    }

    void OnDisable()
    {
        _rollButton.onClick.RemoveListener(RollDice);
        _diceRoller.OnRoll -= HandleRoll;
    }

    void HandleRoll(int obj)
    {
        _resultsText.text = $"You rolled a {obj}";
        _doublesText.text = _diceRoller.Doubles ? "Doubles!" : "";

        // Hide panel after delay
        Invoke(nameof(CloseGamblePanel), closeDelay);
        Invoke(nameof(ClearResults), closeDelay);
    }

    public void RollDice()
    {
        ClearResults();
        _rollButton.interactable = false; // Disable the roll button
        _diceRoller.RollDice();
    }

    void ClearResults()
    {
        _resultsText.text = "";
        _doublesText.text = "";
    }
    
    void CloseGamblePanel()
    {
        if (gamblingPanel != null)
        {
            gamblingPanel.SetActive(false);
            _rollButton.interactable = true; // Reset button for next time
        }
    }
}