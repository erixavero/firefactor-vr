using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtinguisherPin : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main Fire Extinguisher script.")]
    public FireExtinguisher extinguisher;

    [Tooltip("How far the pin needs to be pulled from its starting position to detach.")]
    public float pullDistanceThreshold = 0.15f;

    [Tooltip("Optional sound when the pin is pulled out.")]
    public AudioSource pullSound;

    private XRGrabInteractable grabInteractable;
    private Vector3 initialLocalPosition;
    private bool isPulled = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        initialLocalPosition = transform.localPosition;

        if (grabInteractable != null)
        {
            // Listen for when the player finally lets go of the pin
            grabInteractable.selectExited.AddListener(OnPinReleased);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnPinReleased);
        }
    }

    void Update()
    {
        if (isPulled || extinguisher == null) return;

        // Calculate distance between the pin's current position and its starting slot on the extinguisher
        float distance = Vector3.Distance(transform.localPosition, initialLocalPosition);

        if (distance > pullDistanceThreshold)
        {
            PullPinOut();
        }
    }

    private void PullPinOut()
    {
        isPulled = true;

        // Unlock the main extinguisher
        if (extinguisher != null)
        {
            extinguisher.RemovePin();
        }

        // Play pull sound
        if (pullSound != null && !pullSound.isPlaying)
        {
            pullSound.Play();
        }

        // Unparent the pin from the extinguisher so it is no longer locked to the body,
        // but keep the XRGrabInteractable active so the player continues holding it!
        transform.SetParent(null);

        // Ensure gravity is enabled so it will fall later when dropped
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    private void OnPinReleased(SelectExitEventArgs args)
    {
        // Once the player physically releases the grip button after pulling it out,
        // make sure the Rigidbody is fully non-kinematic so it drops to the floor.
        if (isPulled)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }
}