using UnityEngine;

public class SprayApar : MonoBehaviour
{
    [Header("References")]
    public Transform nozzle; // Drag your nozzle object here in the Inspector
    public ParticleSystem foamParticles;
    
    //spraying mechanics
    [Header("Settings")]
    public float range = 10f;
    public LayerMask detectableLayers;

    [SerializeField] private int rayCount = 5; // Number of rays per frame
    [SerializeField] private float spreadAngle = 30.0f; // Degrees of spread

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Spray();
        }
        else
        {
            StopSpraying();
        }
    }

    void Spray()
    {
        if (!foamParticles.isPlaying) foamParticles.Play();

        for (int i = 0; i < rayCount; i++)
        {
            // Calculate a random rotation within a cone
            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );

            // Multiply the nozzle's forward direction by the random rotation
            Vector3 direction = nozzle.rotation * spreadRotation * Vector3.forward;

            RaycastHit hit;
            if (Physics.Raycast(nozzle.position, direction, out hit, range, detectableLayers))
            {
                if (hit.collider.TryGetComponent<FireObject>(out FireObject fire))
                {
                    // Divide damage by rayCount so the total damage remains consistent
                    fire.TakeDamage(0.3f / rayCount);
                }
                
                Debug.DrawRay(nozzle.position, direction * hit.distance, Color.red);
            }
        }
    }

    void StopSpraying()
    {
        if (foamParticles.isPlaying) foamParticles.Stop();
    }
}