using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Safety Pin Settings")]
    [Tooltip("Is the safety pin currently attached?")]
    public bool pinAttached = true;
    [Tooltip("Optional sound when trying to spray with the pin still in.")]
    public AudioSource pinBlockedAudio;

    [Header("UI Feedback")]
    [Tooltip("TextMeshPro UI text element for warning messages.")]
    public TextMeshProUGUI warningText;
    private Coroutine hideWarningCoroutine;

    [Header("Extinguisher Settings")]
    public ParticleSystem sprayParticles;
    public AudioSource sprayAudio;
    public Transform nozzlePoint;

    [Header("Raycast Settings")]
    public float maxSprayDistance = 5f;
    public LayerMask fireLayer;
    public float shrinkSpeed = 0.5f;

    [Header("Haptic Settings")]
    [Range(0f, 1f)] public float blockedHapticAmplitude = 0.5f;
    public float blockedHapticDuration = 0.2f;
    [Range(0f, 1f)] public float sprayHapticAmplitude = 0.2f;
    private float sprayHapticTimer = 0f;
    private float sprayHapticInterval = 0.1f; 

    private XRGrabInteractable grabInteractable;
    private bool isVrShooting = false;
    private bool isMouseShooting = false;
    private bool isShooting = false;
    private bool mouseWasReleased = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
            grabInteractable.activated.AddListener(OnActivated);
            grabInteractable.deactivated.AddListener(OnDeactivated);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
            grabInteractable.activated.RemoveListener(OnActivated);
            grabInteractable.deactivated.RemoveListener(OnDeactivated);
        }
    }

    private void OnReleased(SelectExitEventArgs args) => isVrShooting = false;
    private void OnActivated(ActivateEventArgs args) => isVrShooting = true;
    private void OnDeactivated(DeactivateEventArgs args) => isVrShooting = false;

    void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            mouseWasReleased = true;
        }

        isMouseShooting = mouseWasReleased && Input.GetMouseButton(0);
        bool shouldShoot = isVrShooting || isMouseShooting;

        if (shouldShoot && pinAttached)
        {
            if (!isShooting)
            {
                if (pinBlockedAudio != null && !pinBlockedAudio.isPlaying)
                {
                    pinBlockedAudio.Play();
                }

                TriggerHaptic(blockedHapticAmplitude, blockedHapticDuration);
                ShowWarningMessage("Spray is locked, pull the pin first!");
            }
            shouldShoot = false; 
        }

        if (shouldShoot && !isShooting)
        {
            StartSpray();
        }
        else if (!shouldShoot && isShooting)
        {
            StopSpray();
        }

        if (isShooting)
        {
            sprayHapticTimer += Time.deltaTime;
            if (sprayHapticTimer >= sprayHapticInterval)
            {
                sprayHapticTimer = 0f;
                TriggerHaptic(sprayHapticAmplitude, sprayHapticInterval);
            }

            PerformSprayRaycast();
        }
    }

    public void RemovePin()
    {
        pinAttached = false;
        isVrShooting = false; 
        
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        Debug.Log("Safety pin removed! Extinguisher unlocked.");
    }

    private void ShowWarningMessage(string message)
    {
        if (warningText == null) return;

        warningText.text = message;
        warningText.gameObject.SetActive(true);

        if (hideWarningCoroutine != null)
        {
            StopCoroutine(hideWarningCoroutine);
        }
        hideWarningCoroutine = StartCoroutine(HideWarningAfterDelay(5f));
    }

    private IEnumerator HideWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    private void TriggerHaptic(float amplitude, float duration)
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            var interactor = grabInteractable.firstInteractorSelecting;
            // Fixed: Removed the invalid .Interactors sub-namespace prefix here
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerInteractor.SendHapticImpulse(amplitude, duration);
            }
        }
    }

    private void StartSpray()
    {
        isShooting = true;
        sprayHapticTimer = 0f;
        if (sprayParticles != null && !sprayParticles.isPlaying) sprayParticles.Play();
        if (sprayAudio != null && !sprayAudio.isPlaying) sprayAudio.Play();
    }

    private void StopSpray()
    {
        isShooting = false;
        if (sprayParticles != null && sprayParticles.isPlaying) sprayParticles.Stop();
        if (sprayAudio != null && sprayAudio.isPlaying) sprayAudio.Stop();
    }

    private void PerformSprayRaycast()
    {
        Transform origin = nozzlePoint != null ? nozzlePoint : transform;
        RaycastHit hit;
        float sphereRadius = 0.3f;

        if (Physics.SphereCast(origin.position, sphereRadius, origin.forward, out hit, maxSprayDistance, fireLayer))
        {
            FireObject fire = hit.collider.GetComponent<FireObject>();
            if (fire != null)
            {
                fire.ShrinkFire(shrinkSpeed * Time.deltaTime);
            }
        }
    }
}