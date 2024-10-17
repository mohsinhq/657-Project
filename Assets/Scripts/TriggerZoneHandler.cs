using UnityEngine;
using UnityEngine.UI;  // For UI components
using UnityEngine.SceneManagement;  // For scene transitions

public class TriggerZoneHandler : MonoBehaviour
{
    public GameObject promptPanel;  // UI Panel to display prompt
    public Button yesButton;
    public Button noButton;
    private bool isPromptActive = false;

    void Start()
    {
        promptPanel.SetActive(false);  // Hide prompt on start
        yesButton.onClick.AddListener(OnYesButton);
        noButton.onClick.AddListener(OnNoButton);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("PromptZone"))  // Orange zone
            {
                ShowPrompt();
            }
            else if (gameObject.CompareTag("ForcedZone"))  // Red zone
            {
                ForceBattle();
            }
        }
    }

    void ShowPrompt()
    {
        promptPanel.SetActive(true);
        isPromptActive = true;
    }

    void OnYesButton()
    {
        if (isPromptActive)
        {
            SceneManager.LoadScene("BattleScene");
        }
    }

    void OnNoButton()
    {
        if (isPromptActive)
        {
            promptPanel.SetActive(false);  // Hide prompt and continue exploring
            isPromptActive = false;
        }
    }

    void ForceBattle()
    {
        SceneManager.LoadScene("BattleScene");  // Automatically load battle scene
    }
}
