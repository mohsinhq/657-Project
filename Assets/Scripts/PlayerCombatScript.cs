using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnBasedCombat : MonoBehaviour
{
    // Health values
    public int playerHealth = 100;
    public int bossHealth = 150;

    // UI Elements for health bars
    public Slider playerHealthBar;
    public Slider bossHealthBar;  // This will represent the enemy's health bar on top of the enemy

    // UI for damage text and buttons
    public TMP_Text damageText;
    public Button pistolButton;
    public Button fireButton;
    public Button iceButton;

    // Damage values for player abilities
    private int pistolDamage = 10;
    private int fireDamage = 20;
    private int iceDamage = 15;

    // Boss attack damage
    private int bossDamage = 10;

    // Flags to manage turns
    private bool playerTurn = true;

    void Start()
    {
        // Initialize health bars
        playerHealthBar.maxValue = playerHealth;
        playerHealthBar.value = playerHealth;

        bossHealthBar.maxValue = bossHealth;  // This is the health bar for the enemy
        bossHealthBar.value = bossHealth;

        // Add listeners to the buttons for player actions
        pistolButton.onClick.AddListener(() => PlayerAttack(pistolDamage, "Pistol"));
        fireButton.onClick.AddListener(() => PlayerAttack(fireDamage, "Fire"));
        iceButton.onClick.AddListener(() => PlayerAttack(iceDamage, "Ice"));
    }

    // Method for the player to attack the boss
    void PlayerAttack(int damage, string attackType)
    {
        if (playerTurn && bossHealth > 0 && playerHealth > 0)
        {
            // Player attacks the boss
            bossHealth -= damage;
            UpdateHealthBars();
            damageText.SetText($"Player used {attackType} and dealt {damage} damage!");

            // End player turn and start boss turn
            playerTurn = false;
            Invoke("BossAttack", 2f); // Boss attacks after 2 seconds
        }
    }

    // Method for the boss to attack the player
    void BossAttack()
    {
        if (bossHealth > 0 && playerHealth > 0)
        {
            // Boss attacks the player
            playerHealth -= bossDamage;
            UpdateHealthBars();
            damageText.SetText($"Boss attacked and dealt {bossDamage} damage!");

            // End boss turn and switch back to player's turn
            playerTurn = true;
        }

        // Check if someone has won
        CheckForEnd();
    }

    // Method to update the health bars
    void UpdateHealthBars()
    {
        // Update the player's health bar
        playerHealthBar.value = playerHealth;

        // Update the enemy's health bar
        bossHealthBar.value = bossHealth;
    }

    // Check if the game has ended
    void CheckForEnd()
    {
        if (bossHealth <= 0)
        {
            damageText.SetText("Player has won!");
            EndGame();
        }
        else if (playerHealth <= 0)
        {
            damageText.SetText("Boss has won!");
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
