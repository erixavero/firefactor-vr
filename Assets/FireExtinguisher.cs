using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Safety Pin Settings")]
    [Tooltip("Is the safety pin currently attached?")]
    public bool pinAttached = true;
    [Tooltip("Optional sound when trying to spray with the pin still in.")]
    public AudioSource pinBlockedAudio;

    [Header("Extinguisher Settings")]
    public ParticleSystem sprayParticles;
    public AudioSource sprayAudio;
    public Transform nozzlePoint;

    [Header("Raycast Settings")]
    public float maxSprayDistance = 5f;
    public LayerMask fireLayer;
    public float shrinkSpeed = 0.5f;

    private XRGrabInteractable grabInteractable;
    private bool isVrShooting = false;
    private bool isMouseShooting = false;
    private bool isShooting = false;
    
    // Prevents mouse click from triggering spray if already held down when pressing Play
    private bool mouseWasReleased = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
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
        // Check if mouse button is released at least once after starting game
        if (!Input.GetMouseButton(0))
        {
            mouseWasReleased = true;
        }

        // Only allow mouse shooting if the button was actually released first
        isMouseShooting = mouseWasReleased && Input.GetMouseButton(0);

        bool shouldShoot = isVrShooting || isMouseShooting;

        // Prevent shooting if the safety pin is still attached!
        if (shouldShoot && pinAttached)
        {
            if (!isShooting && pinBlockedAudio != null && !pinBlockedAudio.isPlaying)
            {
                pinBlockedAudio.Play();
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
            PerformSprayRaycast();
        }
    }

    // Called by the safety pin script when pulled out
    public void RemovePin()
    {
        pinAttached = false;
        isVrShooting = false; 
        Debug.Log("Safety pin removed! Extinguisher unlocked.");
    }

    private void StartSpray()
    {
        isShooting = true;
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