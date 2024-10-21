using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleScript : MonoBehaviour
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
    public TMP_Text beginText;
    public TMP_Text statusText;
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
    private bool playerWon = false;

    public Camera battleCamera;  // Reference to the Main Camera
    public Vector3 cameraPosition = new Vector3(-37, 24, 152);  // Updated position
    public Vector3 cameraRotation = new Vector3(45, 90, 0);  // Updated rotation
    public Vector3 cameraScale = new Vector3(1, 1, 1);  // Updated scale


    void Start()
    {
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

        if (battleCamera == null)
        {
            battleCamera = Camera.main;  // If no camera is assigned, use the Main Camera
        }

        // Set the camera position, rotation, and scale
        battleCamera.transform.position = cameraPosition;
        battleCamera.transform.rotation = Quaternion.Euler(cameraRotation);
        battleCamera.transform.localScale = cameraScale;

        // Add listeners to the buttons for player actions
        pistolButton.onClick.AddListener(() => PlayerAttack(pistolDamage, "Pistol"));
        fireButton.onClick.AddListener(() => PlayerAttack(fireDamage, "Fire"));
        iceButton.onClick.AddListener(() => PlayerAttack(iceDamage, "Ice"));

        
    }

    // Coroutine to show "Begin" text for 3 seconds before starting the game
    IEnumerator ShowBeginMessage()
    {
        string[] beginStates = { "Begin...", "Begin..", "Begin.", "Begin" };
        int index = 0;

        // Loop through each state for 3 seconds (1 second per state)
        for (int i = 0; i < beginStates.Length; i++)
        {
            beginText.SetText(beginStates[index]);
            index = (index + 1) % beginStates.Length;  // Cycle through the states
            yield return new WaitForSeconds(1f);  // Wait for 1 second between updates
        }

        beginText.SetText("");
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

            // Check if the enemy is dead after the player's attack
            CheckForEnd();

            if (enemyHealth > 0)  // Continue only if the enemy is still alive
            {
                // End player turn and trigger companions' turn
                playerTurn = false;
                Invoke("Companion1Attack", 1f);  // Companion 1 attacks after 1 second
            }
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

            // Check if the enemy is dead after Companion 1's attack
            CheckForEnd();

            if (enemyHealth > 0)  // Continue only if the enemy is still alive
            {
                // Trigger Companion 2 attack after 1 second
                Invoke("Companion2Attack", 1f);
            }
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

            // Check if the enemy is dead after Companion 2's attack
            CheckForEnd();

            if (enemyHealth > 0)  // Continue only if the enemy is still alive
            {
                // Trigger enemy attack after 2 seconds
                Invoke("enemyAttack", 2f);
            }
        }
    }

    // Method for the enemy to randomly attacks all player friendlies but randomised the attack option
    void enemyAttack()
    {
        if (enemyHealth > 0)
        {
            // Create an array for the targets (Player, Companion 1, Companion 2)
            int[] targets = { 0, 1, 2 };

            // Shuffle the array to randomize the attack order
            for (int i = 0; i < targets.Length; i++)
            {
                int rnd = Random.Range(0, targets.Length);
                int temp = targets[rnd];
                targets[rnd] = targets[i];
                targets[i] = temp;
            }

            // Perform attacks on each target in the shuffled order
            foreach (int target in targets)
            {
                if (target == 0)
                {
                    // Enemy attacks the player
                    playerHealth -= enemyDamage;
                    damageText.SetText($"enemy attacked the player and dealt {enemyDamage} damage!");
                }
                else if (target == 1)
                {
                    // Enemy attacks Companion 1
                    companion1Health -= enemyDamage;
                    damageText.SetText($"enemy attacked Companion 1 and dealt {enemyDamage} damage!");
                }
                else if (target == 2)
                {
                    // Enemy attacks Companion 2
                    companion2Health -= enemyDamage;
                    damageText.SetText($"enemy attacked Companion 2 and dealt {enemyDamage} damage!");
                }

                // Check if the game ends after each attack
                CheckForEnd();

                // If the game ended, exit early
                if (playerHealth <= 0 && companion1Health <= 0 && companion2Health <= 0)
                {
                    StartCoroutine(EndGame());
                    return;
                }
            }

            // Update all health bars after the attacks
            UpdateHealthBars();

            // End enemy turn and switch back to player's turn
            playerTurn = true;
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

    IEnumerator EndGame()
    {
        string result = "Victory";

        if (!playerWon)
        {
            result = "Defeat...";
        }
        // Disable input buttons after the game ends
        pistolButton.interactable = false;
        fireButton.interactable = false;
        iceButton.interactable = false;

        // Show Victory or Defeat message
        statusText.SetText(result);
        yield return new WaitForSeconds(3f);  // Wait for 3 seconds to show the message

        // After showing the message, transition back to ExplorationScene
        SceneManager.LoadScene("ExplorationScene");
    }

    // Method to check if the game has ended
    void CheckForEnd()
    {
        if (enemyHealth <= 0)
        {
            playerWon = true;
            StartCoroutine(EndGame());
            return;
        }
        else if (playerHealth <= 0 && companion1Health <= 0 && companion2Health <= 0)
        {
            playerWon = false;
            StartCoroutine(EndGame());
            return;
        }
    }
}
