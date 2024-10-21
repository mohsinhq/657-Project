using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class BattleScript : MonoBehaviour
{
    // Health values
    public int playerHealth = 150;
    public int companion1Health = 80;
    public int enemyHealth = 150;
    public int enemyDamage = 10;

    // Defense value for the player and companion
    private int playerDefense = 0;
    private int companion1Defense = 0;

    // UI Elements for health bars
    public Slider playerHealthBar;
    public Slider companion1HealthBar;
    public Slider enemyHealthBar;

    // UI for damage text and buttons
    public TMP_Text damageText;
    public TMP_Text beginText;
    public TMP_Text statusText;

    // Card buttons and texts
    public Button weaponCardButton;
    public Button magic1CardButton;
    public Button magic2CardButton;
    public Button defenseCardButton;
    public TMP_Text weaponCardText;
    public TMP_Text magic1CardText;
    public TMP_Text magic2CardText;
    public TMP_Text defenseCardText;
    public Button playButton;

    // Card definitions
    public List<string> weaponCards = new List<string> { "Pistol", "Sword", "AR", "Bow & Arrow" };
    public List<string> magicCards = new List<string> { "Fire", "Ice", "Poison", "Storm" };
    public List<string> defenseCards = new List<string> { "Lvl 1 Defense", "Lvl 2 Defense", "Lvl 3 Defense" };

    // Player's selected cards
    private string playerWeaponCard;
    private string playerMagicCard1;
    private string playerMagicCard2;
    private string playerDefenseCard;

    // Companion's weapon and defense cards
    private string companion1WeaponCard;
    private string companion1DefenseCard;

    // Enemy's weapon card
    private string enemyWeaponCard;

    // Flags to manage turns
    private bool playerTurn = true;
    private bool playerWon = false;
    private string selectedCardType = "";

    public Camera mainCamera;  // Reference to the Main Camera

    void Start()
    {
        AssignRandomCards();
        StartCoroutine(ShowBeginMessage());

        // Initialize health bars
        playerHealthBar.maxValue = playerHealth;
        playerHealthBar.value = playerHealth;

        companion1HealthBar.maxValue = companion1Health;
        companion1HealthBar.value = companion1Health;

        enemyHealthBar.maxValue = enemyHealth;
        enemyHealthBar.value = enemyHealth;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;  // If no camera is assigned, use the Main Camera
        }

        // Add listeners to the buttons for player actions
        weaponCardButton.onClick.AddListener(() => SelectCard("Weapon"));
        magic1CardButton.onClick.AddListener(() => SelectCard("Magic1"));
        magic2CardButton.onClick.AddListener(() => SelectCard("Magic2"));
        defenseCardButton.onClick.AddListener(() => SelectCard("Defense"));
        playButton.onClick.AddListener(() => ExecuteTurn());
    }

    // Randomly assign cards to the player, companion, and enemy
    void AssignRandomCards()
    {
        playerWeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];

        // Ensure unique magic cards are assigned
        var shuffledMagicCards = new List<string>(magicCards);
        shuffledMagicCards = shuffledMagicCards.OrderBy(x => Random.value).ToList();
        playerMagicCard1 = shuffledMagicCards[0];
        playerMagicCard2 = shuffledMagicCards[1];

        playerDefenseCard = defenseCards[Random.Range(0, defenseCards.Count)];

        // Assign only one weapon card to the companion
        companion1WeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];
        companion1DefenseCard = defenseCards[Random.Range(0, defenseCards.Count)];

        enemyWeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];

        // Update the UI with the card names
        weaponCardText.SetText(playerWeaponCard);
        magic1CardText.SetText(playerMagicCard1);
        magic2CardText.SetText(playerMagicCard2);
        defenseCardText.SetText(playerDefenseCard);
    }

    void SelectCard(string cardType)
    {
        selectedCardType = cardType;
        damageText.SetText($"Player selected {cardType} card!");
    }

    // This is where the selected card action is executed when Play is clicked
    void ExecuteTurn()
    {
        if (playerTurn && !string.IsNullOrEmpty(selectedCardType))
        {
            if (selectedCardType == "Weapon")
            {
                PlayerAttack(playerWeaponCard);
            }
            else if (selectedCardType == "Magic1")
            {
                PlayerAttack(playerMagicCard1);
            }
            else if (selectedCardType == "Magic2")
            {
                PlayerAttack(playerMagicCard2);
            }
            else if (selectedCardType == "Defense")
            {
                PlayerDefend(playerDefenseCard);
            }

            playerTurn = false;
            Invoke("Companion1Attack", 1f);
        }
    }

    // Player attack method
    void PlayerAttack(string card)
    {
        if (enemyHealth > 0 && playerHealth > 0)
        {
            int damage = CalculateCardDamage(card);
            enemyHealth -= damage;
            UpdateHealthBars();
            damageText.SetText($"Player used {card} and dealt {damage} damage!");

            CheckForEnd();
        }
    }

    // Player defend method
    void PlayerDefend(string card)
    {
        switch (card)
        {
            case "Lvl 1 Defense":
                playerDefense = 5;
                break;
            case "Lvl 2 Defense":
                playerDefense = 10;
                break;
            case "Lvl 3 Defense":
                playerDefense = 15;
                break;
        }

        damageText.SetText($"Player selected {card}, defense set to {playerDefense}!");
    }

    // Companion 1 attack method
    void Companion1Attack()
    {
        if (enemyHealth > 0)
        {
            int damage = CalculateCardDamage(companion1WeaponCard);
            enemyHealth -= damage;
            UpdateHealthBars();
            damageText.SetText($"Companion 1 dealt {damage} damage!");

            CheckForEnd();
            if (enemyHealth > 0)
            {
                Invoke("enemyAttack", 2f);  // Trigger enemy attack after 2 seconds
            }
        }
    }

    // Enemy attack method
    void enemyAttack()
    {
        if (enemyHealth > 0)
        {
            int[] targets = { 0, 1 };  // 0 for player, 1 for companion
            System.Random rnd = new System.Random();
            int target = targets.OrderBy(x => rnd.Next()).First();

            if (target == 0)
            {
                int finalDamage = Mathf.Max(0, enemyDamage - playerDefense);
                playerHealth -= finalDamage;
                damageText.SetText($"Enemy attacked the player and dealt {finalDamage} damage!");
            }
            else
            {
                int finalDamage = Mathf.Max(0, enemyDamage - companion1Defense);
                companion1Health -= finalDamage;
                damageText.SetText($"Enemy attacked Companion 1 and dealt {finalDamage} damage!");
            }

            CheckForEnd();
            playerTurn = true;

            // Reset defenses after enemy attack
            playerDefense = 0;
            companion1Defense = 0;
        }
        else
        {
            StartCoroutine(EndGame());
        }
    }

    // Calculate damage based on the card type
    int CalculateCardDamage(string card)
    {
        int damage = 0;

        switch (card)
        {
            case "Pistol":
                damage = 10;
                break;
            case "Sword":
                damage = 15;
                break;
            case "AR":
                damage = 20;
                break;
            case "Bow & Arrow":
                damage = 18;
                break;
            case "Fire":
                damage = 20;
                break;
            case "Ice":
                damage = 15;
                break;
            case "Poison":
                damage = 12;
                break;
            case "Storm":
                damage = 25;
                break;
            default:
                damage = 0;
                break;
        }

        return damage;
    }

    // Method to update the health bars
    void UpdateHealthBars()
    {
        playerHealthBar.value = playerHealth;
        enemyHealthBar.value = enemyHealth;
        companion1HealthBar.value = companion1Health;
    }

    // Coroutine to end the game and show result
    IEnumerator EndGame()
    {
        string result = playerWon ? "Victory" : "Defeat...";
        statusText.SetText(result);

        weaponCardButton.interactable = false;
        magic1CardButton.interactable = false;
        magic2CardButton.interactable = false;
        defenseCardButton.interactable = false;

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("ExplorationScene");
    }

    void CheckForEnd()
    {
        if (enemyHealth <= 0)
        {
            playerWon = true;
            StartCoroutine(EndGame());
        }
        else if (playerHealth <= 0 && companion1Health <= 0)
        {
            playerWon = false;
            StartCoroutine(EndGame());
        }
    }

    // Coroutine to show the "Begin" text at the start of the battle
    IEnumerator ShowBeginMessage()
    {
        string[] beginStates = { "Begin...", "Begin..", "Begin.", "Begin" };
        int index = 0;

        for (int i = 0; i < beginStates.Length; i++)
        {
            beginText.SetText(beginStates[index]);
            index = (index + 1) % beginStates.Length;
            yield return new WaitForSeconds(1f);
        }

        beginText.SetText("");
    }
}
