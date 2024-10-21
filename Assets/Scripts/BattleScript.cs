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
    public int companion2Health = 90;
    public int enemyHealth = 150;
    public int enemyDamage = 10;

    // Defense value for the player
    private int playerDefense = 0;

    // UI Elements for health bars
    public Slider playerHealthBar;
    public Slider companion1HealthBar;
    public Slider companion2HealthBar;
    public Slider enemyHealthBar;

    // UI for damage text and buttons
    public TMP_Text damageText;
    public TMP_Text beginText;
    public TMP_Text statusText;

    // Card buttons and texts
    public Button weaponCardButton;
    public Button magic1CardButton;
    public Button magic2CardButton;
    public Button defenseCardButton; // New defense button
    public TMP_Text weaponCardText;
    public TMP_Text magic1CardText;
    public TMP_Text magic2CardText;
    public TMP_Text defenseCardText; // Text for the defense card
    public Button playButton;

    // Card definitions
    public List<string> weaponCards = new List<string> { "Pistol", "Sword", "AR", "Bow & Arrow" };
    public List<string> magicCards = new List<string> { "Fire", "Ice", "Poison", "Storm" };
    public List<string> defenseCards = new List<string> { "Lvl 1 Defense", "Lvl 2 Defense", "Lvl 3 Defense" };  // Defense cards

    // Player's selected cards
    private string playerWeaponCard;
    private string playerMagicCard1;
    private string playerMagicCard2;
    private string playerDefenseCard;

    // Companion's weapon cards
    private string companion1WeaponCard;
    private string companion2WeaponCard;

    // Enemy's weapon card
    private string enemyWeaponCard;

    // Flags to manage turns
    private bool playerTurn = true;
    private bool playerWon = false;
    private string selectedCardType = ""; // Stores which card the player selects (Weapon, Magic1, Magic2, Defense)

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

        companion2HealthBar.maxValue = companion2Health;
        companion2HealthBar.value = companion2Health;

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
        defenseCardButton.onClick.AddListener(() => SelectCard("Defense"));  // Defense card listener
        playButton.onClick.AddListener(() => ExecuteTurn());  // Play button executes the selected action
    }

    // Randomly assign cards to the player, companions, and enemy
    void AssignRandomCards()
    {
        // Assign 1 weapon card, 2 magic cards, and 1 defense card to the player
        playerWeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];
        playerMagicCard1 = magicCards[Random.Range(0, magicCards.Count)];
        playerMagicCard2 = magicCards[Random.Range(0, magicCards.Count)];
        playerDefenseCard = defenseCards[Random.Range(0, defenseCards.Count)];

        // Assign only weapon cards to companions and enemy
        companion1WeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];
        companion2WeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];
        enemyWeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];

        // Update the UI with the card names
        weaponCardText.SetText(playerWeaponCard);
        magic1CardText.SetText(playerMagicCard1);
        magic2CardText.SetText(playerMagicCard2);
        defenseCardText.SetText(playerDefenseCard);
    }

    // Method for the player to select a card, but the action is not executed yet
    void SelectCard(string cardType)
    {
        selectedCardType = cardType; // Stores the selected card type to be used when Play is pressed
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

            // After the player executes their turn, move to companion attacks
            playerTurn = false;
            Invoke("Companion1Attack", 1f);
        }
    }

    // Player attack method, takes the selected card to calculate damage
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

    // New method for defense
    void PlayerDefend(string card)
    {
        // Assign defense values based on the card level
        switch (card)
        {
            case "Lvl 1 Defense":
                playerDefense = 5;  // Reduces enemy damage by 5
                break;
            case "Lvl 2 Defense":
                playerDefense = 10; // Reduces enemy damage by 10
                break;
            case "Lvl 3 Defense":
                playerDefense = 15; // Reduces enemy damage by 15
                break;
        }

        damageText.SetText($"Player selected {card}, defense set to {playerDefense}!");
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
                Invoke("Companion2Attack", 1f);  // Trigger Companion 2 attack after 1 second
            }
        }
    }

    // Companion 2 attack method
    void Companion2Attack()
    {
        if (enemyHealth > 0)
        {
            int damage = CalculateCardDamage(companion2WeaponCard);
            enemyHealth -= damage;
            UpdateHealthBars();
            damageText.SetText($"Companion 2 dealt {damage} damage!");

            CheckForEnd();
            if (enemyHealth > 0)
            {
                Invoke("enemyAttack", 2f);  // Trigger enemy attack after 2 seconds
            }
        }
    }

    // Enemy attack method, randomly targets player or companions
    void enemyAttack()
    {
        if (enemyHealth > 0)
        {
            int[] targets = { 0, 1, 2 };
            System.Random rnd = new System.Random();
            foreach (int target in targets.OrderBy(x => rnd.Next()))
            {
                if (target == 0)
                {
                    // Apply player defense reduction
                    int finalDamage = Mathf.Max(0, enemyDamage - playerDefense); // Enemy damage reduced by defense
                    playerHealth -= finalDamage;
                    damageText.SetText($"Enemy attacked the player and dealt {finalDamage} damage!");
                }
                else if (target == 1)
                {
                    companion1Health -= enemyDamage;
                    damageText.SetText($"Enemy attacked Companion 1 and dealt {enemyDamage} damage!");
                }
                else if (target == 2)
                {
                    companion2Health -= enemyDamage;
                    damageText.SetText($"Enemy attacked Companion 2 and dealt {enemyDamage} damage!");
                }

                CheckForEnd();
                if (playerHealth <= 0 && companion1Health <= 0 && companion2Health <= 0)
                {
                    StartCoroutine(EndGame());
                    return;
                }
            }

            UpdateHealthBars();
            playerTurn = true;

            // Reset defense after enemy attack
            playerDefense = 0;
        }
        else
        {
            StartCoroutine(EndGame());
        }
    }

    // Method to update the health bars
    void UpdateHealthBars()
    {
        playerHealthBar.value = playerHealth;
        enemyHealthBar.value = enemyHealth;
        companion1HealthBar.value = companion1Health;
        companion2HealthBar.value = companion2Health;
    }

    // Coroutine to end the game and show result
    IEnumerator EndGame()
    {
        string result = playerWon ? "Victory" : "Defeat...";
        statusText.SetText(result);

        // Disable input buttons
        weaponCardButton.interactable = false;
        magic1CardButton.interactable = false;
        magic2CardButton.interactable = false;
        defenseCardButton.interactable = false;

        yield return new WaitForSeconds(3f);

        // Transition back to ExplorationScene
        SceneManager.LoadScene("ExplorationScene");
    }

    // Method to check if the game has ended
    void CheckForEnd()
    {
        if (enemyHealth <= 0)
        {
            playerWon = true;
            StartCoroutine(EndGame());
        }
        else if (playerHealth <= 0 && companion1Health <= 0 && companion2Health <= 0)
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
