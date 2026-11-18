using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [Tooltip("Drag your UI Text (TextMeshPro) component here.")]
    public TextMeshProUGUI fireCountText;

    public Button restartButton;

    private int totalFires;
    private bool gameStarted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Gameplay dimulai oleh VRHeightCalibration setelah kalibrasi selesai.
        UpdateUI();
    }

    public void BeginGame()
    {
        if (gameStarted)
        {
            return;
        }

        gameStarted = true;

        FireObject[] fires = FindObjectsOfType<FireObject>();
        totalFires = fires.Length;

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }

        UpdateUI();
    }

    public void OnFireExtinguished()
    {
        if (!gameStarted)
        {
            return;
        }

        totalFires--;

        if (totalFires < 0)
        {
            totalFires = 0;
        }

        UpdateUI();

        if (totalFires <= 0)
        {
            Debug.Log("All fires extinguished! Fire drill passed.");

            if (fireCountText != null)
            {
                fireCountText.text = "All fires extinguished! Fire drill passed.";
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Restart Button reference is missing on GameManager!");
            }
        }
    }

    private void UpdateUI()
    {
        if (fireCountText != null)
        {
            fireCountText.text = "Fires Remaining: " + totalFires;
        }
        else
        {
            Debug.LogWarning("Fire Count Text reference is missing on GameManager!");
        }
    }
}