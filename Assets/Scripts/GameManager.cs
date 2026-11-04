using UnityEngine;
using TMPro; // Required for TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [Tooltip("Drag your UI Text (TextMeshPro) component here.")]
    public TextMeshProUGUI fireCountText;

    private int totalFires = 0;

    void Awake()
    {
        // Set up a simple singleton pattern so other scripts can easily talk to the GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Automatically find all FireObject scripts currently in the scene
        FireObject[] fires = FindObjectsOfType<FireObject>();
        totalFires = fires.Length;
        
        UpdateUI();
    }

    // Called by a FireObject when it gets put out
    public void OnFireExtinguished()
    {
        totalFires--;
        if (totalFires < 0) totalFires = 0;
        
        UpdateUI();

        if (totalFires == 0)
        {
            Debug.Log("All fires extinguished! Fire drill passed.");
            // Optional: You can trigger a "You Win" UI screen or event here
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