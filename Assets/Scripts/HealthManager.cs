using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public  int playerHealth = 100;
    public int playerMaxHealth = 100;
    public int enemyHealth = 100;
    public int maxEnemeyHealth = 100;
    
    
    [SerializeField] private Image playerHealthBarImage;
    [SerializeField] private Image enemyHealthBarImage;

    [SerializeField] private List<Sprite> playerHealthSprites; 
    [SerializeField] private List<Sprite> enemyHealthSprites; 
   public AudioSource NormalTrack;
   public AudioSource LowTrack;
   private bool isLowHealthTrackPlaying;

    
    public bool hasPassed75 = false;
    public bool hasPassed50 = false;
    public bool hasPassed25 = false;
    
    public bool hasFired75= false;
    public bool hasFired50= false;
    public bool hasFired25= false;
    
    
    
    [SerializeField] DiceRoller2D _diceRoller;
    [SerializeField] GameObject gamblingUI;
    [SerializeField] float gamblingUIPopupDuration = 10f;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private bool playerTookHit;

    public bool hasFinishedGambleRoll { get; private set; } = false;
    
    void Start()
    {
        UpdateUI();
        NormalTrack.Play();
        Debug.Log(NormalTrack.clip.name);
        _diceRoller.OnRoll += HandleDiceResult;
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void DamagePlayer(int damage)
    {
        playerHealth -= damage;
        playerHealth = Mathf.Clamp(playerHealth, 0,playerMaxHealth);

        UpdateUI();
        CheckPlayerHP();
    }

    public void DamageEnemy(int damage)
    {
        enemyHealth -= damage;
        enemyHealth = Mathf.Clamp(enemyHealth, 0, maxEnemeyHealth);
        UpdateUI();
        CheckPlayerHP();
    }

    public void HealPlayer(int heal)
    {
        playerHealth += heal;
        playerHealth = Mathf.Clamp(playerHealth, 0,playerMaxHealth);
        UpdateUI();
    }

    public void HealEnemy(int heal)
    {
        enemyHealth += heal;
        enemyHealth = Mathf.Clamp(enemyHealth, 0, maxEnemeyHealth);
        UpdateUI();
    }

    void UpdateUI()
    {
        CheckPlayerHP();
    
        UpdateHearts(playerHealthBarImage, playerHealthSprites, playerHealth, playerMaxHealth);
        UpdateHearts(enemyHealthBarImage, enemyHealthSprites, enemyHealth, maxEnemeyHealth);

        Debug.Log("PLAYER HP: " + playerHealth);
        Debug.Log("ENEMY HP: " + enemyHealth);
    }
    
    void UpdateHearts(Image targetImage, List<Sprite> spriteList, int currentHealth, int maxHealth)
    {
        int index = 0;
        float percent = (float)currentHealth / maxHealth * 100;

        if (percent < 10) index = 10;
        else if (percent < 20) index = 9;
        else if (percent < 30) index = 8;
        else if (percent < 40) index = 7;
        else if (percent < 50) index = 6;
        else if (percent < 60) index = 5;
        else if (percent < 70) index = 4;
        else if (percent < 80) index = 3;
        else if (percent < 90) index = 2;
        else if (percent < 100) index = 1;
        else index = 0;

        
        index = Mathf.Clamp(index, 0, spriteList.Count - 1);

        targetImage.sprite = spriteList[index];

        Debug.Log($"Health: {currentHealth}, Health%: {percent:F1}%, Sprite Index: {index}");
        Debug.Log(spriteList[index]+ " <--- THIS IS THE INDEX OF THE LIST");
    }

    void CheckPlayerHP()
    {
        if (playerHealth < 75 && !hasPassed75 && !hasFired75)
        {
            hasPassed75 = true;
            hasFired75 = true;
            Debug.Log("Player dropped below 75 HP!");
            TriggerGamble();
        }

        if (playerHealth < 50 && !hasPassed50 && !hasFired50)
        {
            hasPassed50 = true;
            hasFired50 = true;
            Debug.Log("Player dropped below 50 HP!");
            TriggerGamble();
        }

        if (playerHealth < 25 && !hasPassed25  && !hasFired25)
        {
            hasPassed25 = true;
            hasFired25 = true;
            Debug.Log("Player dropped below 25 HP!");
            TriggerGamble();
        }
        
        if (playerHealth <= 50 && !isLowHealthTrackPlaying)
        {
            NormalTrack.Stop();
            LowTrack.Play();
            isLowHealthTrackPlaying = true;
        }
        else if (playerHealth >= 50 && isLowHealthTrackPlaying)
        {
            LowTrack.Stop(); 
            isLowHealthTrackPlaying = false;
            NormalTrack.Play();
        }
       
    
        if (playerHealth >= 75)
        {
            hasPassed75 = false;
            hasFired75 = false;
        }

        if (playerHealth >= 50)
        {
            hasPassed50 = false;
            hasFired50 = false;
        }

        if (playerHealth >= 25)
        {
            hasPassed25 = false;
            hasFired25 = false;
        }
    }

    public void TriggerGamble()
    {
        dialogueManager.Gambling();
        Debug.Log("GAMBLER YOU FAILED");
        if (gamblingUI != null && !gamblingUI.activeSelf)
        {
            hasFinishedGambleRoll = false; 
            gamblingUI.SetActive(true);
        
        }
    }

    void HideLowHealthPanel()
    {
        gamblingUI.SetActive(false);
    }

    void OnDestroy()
    {
        if (_diceRoller != null)
            _diceRoller.OnRoll -= HandleDiceResult;
    }

    void HandleDiceResult(int result)
    {
        // Healing from doubles
        if (_diceRoller.Doubles)
        {
            int dieValue = result / 2;

            if (dieValue % 2 == 0)
            {
                HealPlayer(10);
                Debug.Log("Doubles rolled and even value! Healed player 10 HP.");
            }
            else
            {
                HealEnemy(10);
                Debug.Log("Doubles rolled and odd value! Healed enemy 10 HP.");
            }
        }

        // Damage logic based on result value
        if (result % 2 == 0)
        {
            DamageEnemy(result);
            Debug.Log($"Rolled EVEN ({result})! Player dealt {result} damage to enemy.");
            playerTookHit = false;
        }
        else
        {
            DamagePlayer(result);
            Debug.Log($"Rolled ODD ({result})! Enemy dealt {result} damage to player.");
            playerTookHit = true;
        }
        
        hasFinishedGambleRoll = true;
        dialogueManager.ReactionDice(playerTookHit);
    }
    
 
    
}