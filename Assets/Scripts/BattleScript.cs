using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class BattleScript : MonoBehaviour
{
    // Instruction panel and start battle button
    public GameObject instructionsPanel;
    public Button startBattleButton;


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
    public TMP_Text centerText;
    //public TMP_Text statusText;
    public TMP_Text scrollableTextFeed;
    public TMP_Text statusPromptsText;

    //Combining Cards feature variables
    private bool isCombining = false;  
    private int combinationCount = 0;  
    private const int maxCombinations = 2;

    // Selected Combined Card Variables
    private string CombinationWeaponCard = "";
    private string CombinationMagicCard = "";

    //Defence Card Variables
    private int defenseCount = 2; 
    private const int maxDefenseUses = 2;

    // Card buttons and texts
    public Button weaponCardButton;
    public Button magic1CardButton;
    public Button magic2CardButton;
    public Button defenseCardButton;
    public Button combineCardsButton;
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

    public Camera mainCamera;


    void Start()
    {
        // Showing the instruction panel and disable game buttons
        instructionsPanel.SetActive(true);
        weaponCardButton.interactable = false;
        magic1CardButton.interactable = false;
        magic2CardButton.interactable = false;
        defenseCardButton.interactable = false;
        combineCardsButton.interactable = false;

        
        startBattleButton.onClick.AddListener(StartBattle);
        combineCardsButton.onClick.AddListener(ToggleCombineMode);
        weaponCardButton.onClick.AddListener(() => SelectCard("Weapon"));
        magic1CardButton.onClick.AddListener(() => SelectCard("Magic1"));
        magic2CardButton.onClick.AddListener(() => SelectCard("Magic2"));
        defenseCardButton.onClick.AddListener(() => SelectCard("Defense"));
        playButton.onClick.AddListener(() => ExecuteTurn());
    }

    public void StartBattle()
    {
        instructionsPanel.SetActive(false);
        StartCoroutine(ShowBeginMessage());

        // Initialize health bars after battle starts
        playerHealthBar.maxValue = playerHealth;
        playerHealthBar.value = playerHealth;
        companion1HealthBar.maxValue = companion1Health;
        companion1HealthBar.value = companion1Health;
        enemyHealthBar.maxValue = enemyHealth;
        enemyHealthBar.value = enemyHealth;

        // Assign cards after battle begins
        AssignRandomCards();
    }


    // Randomly assigning cards to the player, companion, and enemy
    void AssignRandomCards()
    {
        playerWeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];

        // Ensuring unique magic cards are assigned
        var shuffledMagicCards = new List<string>(magicCards);
        shuffledMagicCards = shuffledMagicCards.OrderBy(x => Random.value).ToList();
        playerMagicCard1 = shuffledMagicCards[0];
        playerMagicCard2 = shuffledMagicCards[1];

        playerDefenseCard = defenseCards[Random.Range(0, defenseCards.Count)];

        // Assigning only one weapon card to the companion
        companion1WeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];
        companion1DefenseCard = defenseCards[Random.Range(0, defenseCards.Count)];
        enemyWeaponCard = weaponCards[Random.Range(0, weaponCards.Count)];

        // Updating the UI with the card names
        weaponCardText.SetText(playerWeaponCard);
        magic1CardText.SetText(playerMagicCard1);
        magic2CardText.SetText(playerMagicCard2);
        defenseCardText.SetText(playerDefenseCard);
    }

    // Method triggers the combine mode, disabling/enabling the defence mode 
    void ToggleCombineMode()
    {
        isCombining = !isCombining;

        if (isCombining)
        {
            if (combinationCount >= maxCombinations)
            {
                statusPromptsText.SetText("You have reached the maximum number of combinations.");
                combinationCount++;
            }
            else
            {
                statusPromptsText.SetText("Combining cards: Select one weapon and one magic card."); 
            }
            // Disable combine card button as player cannot combine anymore
            combineCardsButton.interactable = false;
        }
        else
        {
            // Reseting the selection if the player cancels the combination
            statusPromptsText.SetText("Combination canceled. Play a single card.");
            defenseCardButton.interactable = true;  
        }

        return;
    }

    // Method to reset the card selection
    void ResetSelection()
    {
        selectedCardType = "";
        CombinationWeaponCard = "";
        CombinationMagicCard = "";
        defenseCardButton.interactable = true; 
        isCombining = false; 
    }

    // Method to select the card based on the card type and update the status text accordingly
    void SelectCard(string cardType)
    {
        if (isCombining)
        {
            if (cardType == "Weapon")
            {
                CombinationWeaponCard = playerWeaponCard;
                statusPromptsText.SetText($"Selected weapon: {CombinationWeaponCard}. Now select a magic card.");
            }
            else if (cardType == "Magic1" || cardType == "Magic2")
            {
                CombinationMagicCard = cardType == "Magic1" ? playerMagicCard1 : playerMagicCard2;
                statusPromptsText.SetText($"Selected magic: {CombinationMagicCard}. Now press play to attack.");
            }
        }
        else
        {
            selectedCardType = cardType;  // Single card play
            statusPromptsText.SetText($"Player selected {cardType} card!");
        }
    }

    // This is where the selected card action is executed when Play is clicked
    void ExecuteTurn()
    {
        if (playerTurn)
        {
            int totalDamage = 0;

            // Handle card combination only if the player is combining and has attempts left
            if (isCombining && combinationCount < maxCombinations)
            {
                if (!string.IsNullOrEmpty(CombinationWeaponCard) && !string.IsNullOrEmpty(CombinationMagicCard))
                {
                    totalDamage += CalculateCardDamage(CombinationWeaponCard);
                    totalDamage += CalculateCardDamage(CombinationMagicCard);
                    AddToFeed("You", $"Combined {CombinationWeaponCard} and {CombinationMagicCard}", totalDamage);

                    combinationCount++;
                    statusPromptsText.SetText($"You have {maxCombinations - combinationCount} combination attempts left.");

                    ResetSelection();
                }
                else
                {
                    statusPromptsText.SetText("You must select both a weapon and a magic card to combine.");
                    return;
                }
            }
            // Handle single card play if the player is not combining
            else if (!string.IsNullOrEmpty(selectedCardType)) 
            {
                if (selectedCardType == "Weapon")
                {
                    totalDamage = CalculateCardDamage(playerWeaponCard);
                    AddToFeed("You", $"Used {playerWeaponCard}", totalDamage);
                }
                else if (selectedCardType == "Magic1")
                {
                    totalDamage = CalculateCardDamage(playerMagicCard1);
                    AddToFeed("You", $"Used {playerMagicCard1}", totalDamage);
                }
                else if (selectedCardType == "Magic2")
                {
                    totalDamage = CalculateCardDamage(playerMagicCard2);
                    AddToFeed("You", $"Used {playerMagicCard2}", totalDamage);
                }
                else if (selectedCardType == "Defense")
                {
                    if (defenseCount > 0)
                    {
                        PlayerDefend(playerDefenseCard);
                        defenseCount--;
                        statusPromptsText.SetText($"You have {defenseCount} defense card attempts left.");

                        if (defenseCount == 0)
                        {
                            defenseCardButton.interactable = false;
                            statusPromptsText.SetText("You have used all your defense card attempts.");
                        }
                    }
                    return;
                }
            }
            else // If no card is selected, prompt the player to select a card
            {
                statusPromptsText.SetText("No card selected. Please select a card.");
                return;
            }

            // Apply damage to the enemy and reset for the next turn
            enemyHealth -= totalDamage;
            UpdateHealthBars();
            CheckForEnd();
            playerTurn = false;
            Invoke("Companion1Attack", 1f);

            // Reset selection after the turn
            ResetSelection();
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
            statusPromptsText.SetText($"Player used {card} and dealt {damage} damage!");

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

        statusPromptsText.SetText($"Player selected {card}, defense set to {playerDefense}!");
    }

    // Companion 1 attack method
    void Companion1Attack()
    {
        if (enemyHealth > 0)
        {
            int damage = CalculateCardDamage(companion1WeaponCard);
            enemyHealth -= damage;
            UpdateHealthBars();
            AddToFeed("Companion", $"Used {companion1WeaponCard}", damage);

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
            int[] targets = { 0, 1 };
            System.Random rnd = new System.Random();
            int target = targets.OrderBy(x => rnd.Next()).First();

            if (target == 0)
            {
                int finalDamage = Mathf.Max(0, enemyDamage - playerDefense);
                playerHealth -= finalDamage;
                AddToFeed("Enemy", "Attacked you", finalDamage);
            }
            else if (target == 1)
            {
                int finalDamage = Mathf.Max(0, enemyDamage - companion1Defense);
                companion1Health -= finalDamage;
                AddToFeed("Enemy", "Attacked Companion 1", finalDamage);
            }

            // Update health bars and check for end of game
            CheckForEnd();
            playerTurn = true;

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

    // method to format the feed text and add it to the scrollable feed
    void AddToFeed(string actor, string action, int damage)
    {
        string coloredActor = "";
        if (actor == "You")
        {
            coloredActor = "<color=red>You:</color>";
        }
        else if (actor == "Companion")
        {
            coloredActor = "<color=red>Companion:</color>";
        }
        else if (actor == "Enemy")
        {
            coloredActor = "<color=red>Enemy:</color>";
        }

        // Add the new entry to the top of the scrollable feed
        string newEntry = $"{coloredActor} {action} dealt {damage} damage.\n";
        scrollableTextFeed.text = newEntry + scrollableTextFeed.text;

        // Scroll to the top of the feed so latest is at the top
        Canvas.ForceUpdateCanvases();
        scrollableTextFeed.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();
    }


    // Coroutine to end the game and show result
    IEnumerator EndGame()
    {
        string result = playerWon ? "Victory" : "Defeat...";
        centerText.SetText(result);

        weaponCardButton.interactable = false;
        magic1CardButton.interactable = false;
        magic2CardButton.interactable = false;
        defenseCardButton.interactable = false;

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("ExplorationScene");
    }

    // Method to check if the game has ended and calls the end game coroutine
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

    // Coroutine to give the player a countdown before the game starts
    IEnumerator ShowBeginMessage()
    {
        string[] beginStates = { "Begin...", "Begin..", "Begin.", "Begin" };
        int index = 0;

        for (int i = 0; i < beginStates.Length; i++)
        {
            centerText.SetText(beginStates[index]);
            index = (index + 1) % beginStates.Length;
            yield return new WaitForSeconds(1f);
        }

        centerText.SetText("");

        // Enable buttons after the countdown
        weaponCardButton.interactable = true;
        magic1CardButton.interactable = true;
        magic2CardButton.interactable = true;
        defenseCardButton.interactable = true;
        combineCardsButton.interactable = true;
    }
}
