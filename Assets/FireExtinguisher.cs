using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Settings")]
    [Tooltip("The particle system representing the extinguishing spray.")]
    public ParticleSystem sprayParticles;
    
    [Tooltip("Audio source for spraying sound.")]
    public AudioSource sprayAudio;
    
    [Tooltip("The point where the spray comes out (create an empty GameObject at the nozzle).")]
    public Transform nozzlePoint;

    [Header("Raycast Settings")]
    [Tooltip("Maximum distance the spray can reach.")]
    public float maxSprayDistance = 5f;
    
    [Tooltip("The layer mask for your fire objects.")]
    public LayerMask fireLayer;
    
    [Tooltip("How fast the fire shrinks while being sprayed.")]
    public float shrinkSpeed = 0.5f;

    private XRGrabInteractable grabInteractable;
    
    // States to track input from both VR and Mouse
    private bool isVrShooting = false;
    private bool isMouseShooting = false;
    private bool isShooting = false;

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
        // Check for Left Mouse Button (Hold to spray for testing)
        isMouseShooting = Input.GetMouseButton(0);

        // We shoot if either VR trigger is pulled OR the mouse button is held down
        bool shouldShoot = isVrShooting || isMouseShooting;

        if (shouldShoot && !isShooting)
        {
            StartSpray();
        }
        else if (!shouldShoot && isShooting)
        {
            StopSpray();
        }

        // Perform the raycast logic while spraying
        if (isShooting)
        {
            PerformSprayRaycast();
        }
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
        if (sprayAudio != null && !sprayAudio.isPlaying) sprayAudio.Stop();
    }

    private void PerformSprayRaycast()
    {
        // Use nozzlePoint if assigned, otherwise use the extinguisher's position/rotation
        Transform origin = nozzlePoint != null ? nozzlePoint : transform;

        RaycastHit hit;
        float sphereRadius = 0.3f; // Thickness of the spray beam

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