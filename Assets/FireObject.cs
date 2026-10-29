using UnityEngine;

public class FireObject : MonoBehaviour
{
    [Header("Fire Settings")]
    [Tooltip("Minimum scale before the fire is completely extinguished.")]
    public float minScale = 0.05f;

    // This method is called by the FireExtinguisher raycast every frame it hits the fire
    public void ShrinkFire(float shrinkAmount)
    {
        // Shrink the fire uniformly
        transform.localScale -= Vector3.one * shrinkAmount;

        // Clamp scale so it never drops below zero
        transform.localScale = new Vector3(
            Mathf.Max(0, transform.localScale.x),
            Mathf.Max(0, transform.localScale.y),
            Mathf.Max(0, transform.localScale.z)
        );

        // Check if the fire is small enough to be put out
        if (transform.localScale.x <= minScale)
        {
            ExtinguishFire();
        }
    }

    private void ExtinguishFire()
    {
        Destroy(gameObject);
    }
}