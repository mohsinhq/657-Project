using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TriggerZoneHandler : MonoBehaviour
{
    public GameObject promptPanel;  // The Panel with the prompt message
    public TMP_Text promptMessage;  // The UI Text element for the prompt message
    public Button yesButton;  // "Yes" button
    public Button noButton;   // "No" button
    public Transform player;  // The player’s transform to move the player when "No" is clicked
    public Vector3 outsideBoxPosition;  // The new position to move the player when "No" is clicked

    private bool isPromptActive = false;
    private bool isLoading = false;

    void Start()
    {
        promptPanel.SetActive(false);  // Hide the panel at the start
        yesButton.onClick.AddListener(OnYesButton);
        noButton.onClick.AddListener(OnNoButton);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameObject.CompareTag("PromptZone"))
        {
            ShowPrompt();
        }
        else if (gameObject.CompareTag("ForcedZone") && !isLoading)  // Red zone
        {
            // Automatically start the loading process when entering red zone
            StartCoroutine(StartLoadingAnimation());
        }
    }

    public void ShowPrompt()
    {
        promptPanel.SetActive(true);  // Show the panel with the prompt
        promptMessage.SetText("Do you wish to enter battle?");  // Set the prompt message using SetText()
        isPromptActive = true;

        // Make sure the buttons are visible
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    public void OnYesButton()
    {
        if (isPromptActive)
        {
            // Hide both buttons
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            // Start the loading sequence
            StartCoroutine(StartLoadingAnimation());
        }
    }

    public void OnNoButton()
    {
        if (isPromptActive)
        {
            // Hide both buttons
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            // Hide the prompt panel
            promptPanel.SetActive(false);

            // Move the player to the outsideBoxPosition
            player.position = outsideBoxPosition;

            // Mark the prompt as inactive
            isPromptActive = false;
        }
    }

    IEnumerator StartLoadingAnimation()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        isLoading = true;  // Prevent multiple loading processes
        promptMessage.SetText("Loading");  // Initial message
        promptMessage.alignment = TextAlignmentOptions.Center;

        // Set the background color to red
        promptPanel.GetComponent<Image>().color = Color.red;

        string[] loadingStates = { "Loading", "Loading.", "Loading..", "Loading..." };
        int index = 0;

        // Loop through the loading animation for 5 seconds
        for (int i = 0; i < 5; i++)
        {
            promptMessage.SetText(loadingStates[index]);  // Update the message
            index = (index + 1) % loadingStates.Length;  // Cycle through the states
            yield return new WaitForSeconds(1f);  // Wait for 1 second between updates
        }

        // After 5 seconds, load the battle scene
        SceneManager.LoadScene("BattleScene");
    }
}
