using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject spawnPrefabOnHit; // Prefab to spawn at the target location when bullet reaches the target
    public float destroyDelay = 0f;  // Delay before destroying the bullet

    // You may want to pass the target object from the HoldGun or elsewhere
    public GameObject targetPositionObject;  // Reference to the target's position object

    private void OnTriggerEnter(Collider other)
    {
        // Check if the target position is valid (not at default position)
        if (other.transform.position != Vector3.zero)
        {
            // Proceed if the target is set and the bullet collides with it
            if (other.CompareTag("Target")) // Ensure the target has the "Target" tag
            {
                // Spawn the prefab at the target location
                if (spawnPrefabOnHit != null)
                {
                    GameObject spawnedObject = Instantiate(spawnPrefabOnHit, this.transform.position, Quaternion.identity);
                    ParticleSystem ps = spawnedObject.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Play(); // Manually start the particle effect
                        Destroy(spawnedObject, ps.main.duration + ps.main.startLifetime.constantMax); // Destroy after effect ends
                    }
                    else
                    {
                        Destroy(spawnedObject, 5f); // Fallback destroy after 5 seconds
                    }

                }

                // Destroy the bullet after the collision
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
