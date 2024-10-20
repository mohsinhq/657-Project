using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnBasedCombat : MonoBehaviour
{
    // Health values
    public int playerHealth = 150;
    public int companion1Health = 80;
    public int companion2Health = 90;
    public int enemyHealth = 150;

    // UI Elements for health bars
    public Slider playerHealthBar;
    public Slider companion1HealthBar;
    public Slider companion2HealthBar;
    public Slider enemyHealthBar;

    // UI for damage text and buttons
    public TMP_Text damageText;
    public Button pistolButton;
    public Button fireButton;
    public Button iceButton;

    // Damage values for player abilities
    private int pistolDamage = 10;
    private int fireDamage = 20;
    private int iceDamage = 15;

    // Companion damage values
    private int companion1Damage = 5;
    private int companion2Damage = 7;

    // enemy attack damage
    private int enemyDamage = 10;

    // Flags to manage turns
    private bool playerTurn = true;

    void Start()
    {
        // Initialize health bars
        playerHealthBar.maxValue = playerHealth;
        playerHealthBar.value = playerHealth;

        companion1HealthBar.maxValue = companion1Health;
        companion1HealthBar.value = companion1Health;

        companion2HealthBar.maxValue = companion2Health;
        companion2HealthBar.value = companion2Health;

        enemyHealthBar.maxValue = enemyHealth;
        enemyHealthBar.value = enemyHealth;

        // Add listeners to the buttons for player actions
        pistolButton.onClick.AddListener(() => PlayerAttack(pistolDamage, "Pistol"));
        fireButton.onClick.AddListener(() => PlayerAttack(fireDamage, "Fire"));
        iceButton.onClick.AddListener(() => PlayerAttack(iceDamage, "Ice"));
    }

    // Method for the player to attack the enemy
    void PlayerAttack(int damage, string attackType)
    {
        if (playerTurn && enemyHealth > 0 && playerHealth > 0)
        {
            // Player attacks the enemy
            enemyHealth -= damage;
            UpdateHealthBars();
            damageText.SetText($"Player used {attackType} and dealt {damage} damage!");

            // End player turn and trigger companions' turn
            playerTurn = false;
            Invoke("Companion1Attack", 1f);  // Companion 1 attacks after 1 second
        }
    }

    // Method for Companion 1 to attack the enemy
    void Companion1Attack()
    {
        if (enemyHealth > 0)
        {
            enemyHealth -= companion1Damage;
            UpdateHealthBars();
            damageText.SetText($"Companion 1 dealt {companion1Damage} damage!");

            // Trigger Companion 2 attack after 1 second
            Invoke("Companion2Attack", 1f);
        }
    }

    // Method for Companion 2 to attack the enemy
    void Companion2Attack()
    {
        if (enemyHealth > 0)
        {
            enemyHealth -= companion2Damage;
            UpdateHealthBars();
            damageText.SetText($"Companion 2 dealt {companion2Damage} damage!");

            // Trigger enemy attack after 2 seconds
            Invoke("enemyAttack", 2f);
        }
    }

    // Method for the enemy to randomly attack either the player or companions
    void enemyAttack()
    {
        if (enemyHealth > 0)
        {
            int target = Random.Range(0, 3); // Randomly select 0 (Player), 1 (Companion 1), or 2 (Companion 2)

            if (target == 0)
            {
                // enemy attacks the player
                playerHealth -= enemyDamage;
                damageText.SetText($"enemy attacked the player and dealt {enemyDamage} damage!");
            }
            else if (target == 1)
            {
                // enemy attacks Companion 1
                companion1Health -= enemyDamage;
                damageText.SetText($"enemy attacked Companion 1 and dealt {enemyDamage} damage!");
            }
            else if (target == 2)
            {
                // enemy attacks Companion 2
                companion2Health -= enemyDamage;
                damageText.SetText($"enemy attacked Companion 2 and dealt {enemyDamage} damage!");
            }

            // Update all health bars after the attack
            UpdateHealthBars();

            // End enemy turn and switch back to player's turn
            playerTurn = true;

            // Check for end of game
            CheckForEnd();
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

    // Check if the game has ended
    void CheckForEnd()
    {
        if (enemyHealth <= 0)
        {
            damageText.SetText("Player and companions have won!");
            EndGame();
        }
        else if (playerHealth <= 0 && companion1Health <= 0 && companion2Health <= 0)
        {
            damageText.SetText("enemy has won!");
            EndGame();
        }
    }

    // End the game by disabling the buttons
    void EndGame()
    {
        pistolButton.interactable = false;
        fireButton.interactable = false;
        iceButton.interactable = false;
    }
}
