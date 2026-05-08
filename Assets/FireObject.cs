using UnityEngine;

public class FireObject : MonoBehaviour
{
    [SerializeField] private float fireHealth = 1.0f;
    [SerializeField] private float extinguishRate = 0.5f;

    private ParticleSystem[] allFireParticles;
    private float[] originalEmissionRates;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        
        // Find every particle system attached to this cube or its children
        allFireParticles = GetComponentsInChildren<ParticleSystem>();
        
        // Store the starting emission rates so we know what "100%" looks like
        originalEmissionRates = new float[allFireParticles.Length];
        
        for (int i = 0; i < allFireParticles.Length; i++)
        {
            originalEmissionRates[i] = allFireParticles[i].emission.rateOverTime.constant;
        }
    }

    public void TakeDamage(float amount)
    {
        fireHealth -= amount * Time.deltaTime;
        fireHealth = Mathf.Clamp01(fireHealth);

        // 1. Shrink the Physical Cube/Collider
        transform.localScale = originalScale * fireHealth;

        // 2. Loop through all 8 particle systems and scale their emission
        for (int i = 0; i < allFireParticles.Length; i++)
        {
            var emission = allFireParticles[i].emission;
            // Scale the rate based on current health (e.g., 50% health = 50% emission)
            emission.rateOverTime = originalEmissionRates[i] * fireHealth;
        }

        if (fireHealth <= 0)
        {
            Extinguish();
        }
    }

    void Extinguish()
    {
        // De-parent the particles before destroying the cube if you want 
        // the last few embers to fade out naturally instead of vanishing instantly
        foreach (var ps in allFireParticles)
        {
            ps.transform.parent = null; 
            ps.Stop();
            Destroy(ps.gameObject, 2f); // Clean up the particles after 2 seconds
        }

        Destroy(gameObject);
    }
}