using UnityEngine;

public class EnemyShieldHandler : MonoBehaviour
{
    public GameObject shieldPrefab;        // The prefab for the enemy shield
    public GameObject targetPosition;      // The GameObject containing the enemy's position (Transform)
    public GameObject isVisible;           // The GameObject that determines if the enemy is visible (active state)

    private GameObject spawnedShield;      // The instance of the shield prefab
    public GameObject sparksPrefab;  // Reference to the sparks prefab
    private bool isEnemyVisible = false;   // Tracks the current visibility state of the enemy

    // This method will be called when the script is enabled
    private void OnEnable()
    {
        // If the enemy is visible when the script is enabled, spawn the shield
        if (isVisible.activeSelf)
        {
            SpawnShield();
            isEnemyVisible = true;  // Update visibility state
        }
    }

    void Update()
    {
        // If isVisible becomes active (enemy visible), spawn the shield
        if (isVisible.activeSelf && !isEnemyVisible)
        {
            SpawnShield();
            isEnemyVisible = true;  // Update visibility state
        }

        // If isVisible becomes inactive (enemy invisible), destroy the shield
        if (!isVisible.activeSelf && isEnemyVisible)
        {
            DestroyShield();
            isEnemyVisible = false;  // Update visibility state
        }
    }

    void SpawnShield()
    {
        // Check if the prefab and target position are assigned
        if (shieldPrefab != null && targetPosition != null)
        {
            // Instantiate the shield at the target position
            spawnedShield = Instantiate(shieldPrefab, targetPosition.transform.position, Quaternion.identity);
            spawnedShield.transform.SetParent(targetPosition.transform);  // Optional: Set parent to the enemy object
        }
        else
        {
            Debug.LogWarning("Shield prefab or target position is not assigned!");
        }
    }

    void DestroyShield()
    {
        // Check if the shield exists and destroy it
        if (spawnedShield != null)
        {
            Destroy(spawnedShield);
        }
    }

    // Ensure shield is destroyed when the script is disabled
    private void OnDisable()
    {
        DestroyShield();
    }

    


public void ShieldSparks()
{
    // Check if the sparks prefab is assigned and the enemy is visible
    if (sparksPrefab != null && isVisible.activeSelf && spawnedShield != null)
    {
        // Spawn the sparks prefab at the shield's position
        GameObject spawnedSparks = Instantiate(sparksPrefab, spawnedShield.transform.position, Quaternion.identity);

        // Check if the spawned sparks object has a ParticleSystem to play
        ParticleSystem ps = spawnedSparks.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play(); // Manually start the particle effect
            Destroy(spawnedSparks, ps.main.duration + ps.main.startLifetime.constantMax); // Destroy after effect ends
        }
        else
        {
            Destroy(spawnedSparks, 5f); // Fallback destroy after 5 seconds if no particle system
        }

        Debug.Log("Sparks spawned at shield position: " + spawnedShield.transform.position);
    }
    else
    {
        Debug.LogWarning("❌ No sparks prefab assigned or enemy shield not visible.");
    }
}

}
