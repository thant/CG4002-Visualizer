using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SnowBomb : MonoBehaviour
{
    public GameObject snowfallEffectPrefab;  // Prefab for snowfall effect
    public float launchForce = 5f;  // Speed of the snowbomb
    public float gravity = 9.8f; // Simulate gravity effect
    private Vector3 velocity;
    private bool hasLanded = false;

    void Start()
    {
        // Launch the bomb forward from the camera
        velocity = Camera.main.transform.forward * launchForce;
    }

    void Update()
    {
        if (!hasLanded)
        {
            // Simulate projectile motion
            velocity.y -= gravity * Time.deltaTime;  // Apply gravity
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;

            // Get the impact position
            Vector3 impactPosition = collision.contacts[0].point;

            // Instantiate snowfall effect
            GameObject snowfall = Instantiate(snowfallEffectPrefab, impactPosition, Quaternion.identity);

            // Ensure snowfall stays physically anchored
            ARAnchor anchor = snowfall.AddComponent<ARAnchor>();

            // Destroy the snowbomb object itself
            Destroy(gameObject);
        }
    }
}
