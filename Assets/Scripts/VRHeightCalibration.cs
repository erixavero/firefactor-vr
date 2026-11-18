using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

public class VRHeightCalibration : MonoBehaviour
{
    [Header("XR References")]
    [SerializeField]
    private XROrigin xrOrigin;

    [Header("Game References")]
    [Tooltip("GameManager yang berada di dalam Scene Manager.")]
    [SerializeField]
    private GameManager gameManager;

    [Tooltip("Object gameplay yang dinonaktifkan selama kalibrasi.")]
    [SerializeField]
    private GameObject[] gameplayObjects;

    [Tooltip("Panel yang ditampilkan selama proses kalibrasi.")]
    [SerializeField]
    private GameObject calibrationPanel;

    [Header("Calibration Settings")]
    [SerializeField]
    private float trackingWaitTime = 1.5f;

    private void Awake()
    {
        SetGameplayActive(false);
    }

    private IEnumerator Start()
    {
        if (xrOrigin == null)
        {
            xrOrigin = FindObjectOfType<XROrigin>();
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (xrOrigin == null)
        {
            Debug.LogError("XROrigin tidak ditemukan.", this);
            StartGameplay();
            yield break;
        }

        if (calibrationPanel != null)
        {
            calibrationPanel.SetActive(true);
        }

        xrOrigin.RequestedTrackingOriginMode =
            XROrigin.TrackingOriginMode.Floor;

        // Menunggu headset mendapatkan posisi dan tinggi aktual.
        yield return new WaitForSecondsRealtime(trackingWaitTime);
        yield return null;

        ApplyFloorHeight();
        StartGameplay();
    }

    private void ApplyFloorHeight()
    {
        if (xrOrigin.CameraFloorOffsetObject == null)
        {
            Debug.LogWarning(
                "Camera Floor Offset Object belum diatur pada XROrigin.",
                xrOrigin);
            return;
        }

        Vector3 offsetPosition =
            xrOrigin.CameraFloorOffsetObject.transform.localPosition;

        // Mode Floor sudah menggunakan lantai nyata sebagai titik 0.
        offsetPosition.y = 0f;

        xrOrigin.CameraFloorOffsetObject.transform.localPosition =
            offsetPosition;

        Debug.Log("VR height calibration selesai.");
    }

    private void StartGameplay()
    {
        SetGameplayActive(true);

        if (calibrationPanel != null)
        {
            calibrationPanel.SetActive(false);
        }

        if (gameManager != null)
        {
            gameManager.BeginGame();
        }
        else
        {
            Debug.LogWarning("GameManager tidak ditemukan.", this);
        }
    }

    private void SetGameplayActive(bool isActive)
    {
        if (gameplayObjects == null)
        {
            return;
        }

        foreach (GameObject gameplayObject in gameplayObjects)
        {
            if (gameplayObject != null)
            {
                gameplayObject.SetActive(isActive);
            }
        }
    }
}