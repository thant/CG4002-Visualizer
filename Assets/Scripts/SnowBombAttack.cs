using UnityEngine;
using System.Collections;

public class SnowBombAttack : MonoBehaviour
{
    public GameObject snowbombPrefab;
    public GameObject snowfallPrefab;
    public GameObject additionalPrefab; // The new prefab to spawn after snowbomb despawns
    public float launchSpeed = 2f;

    public GameObject trackingTarget; // The GameObject that contains the flag and target position

    public GameObject targetFlag; // Reference to the target flag (set in the inspector)

    void Awake()
    {
    }

    public void LaunchSnowbomb()
    {
        // Check if the target flag is active (i.e., a valid target exists)
        if (!targetFlag.activeSelf)
        {
            Debug.Log("No valid target for Snowbomb, launching forward without target.");

            // Use a default target when there's no valid target (launch forward from the camera)
            Vector3 defaultTargetPosition = Camera.main.transform.position + Camera.main.transform.forward * 10f;
            GameObject snowbomb = Instantiate(snowbombPrefab, Camera.main.transform.position, Quaternion.identity);
            StartCoroutine(MoveSnowbomb(snowbomb, defaultTargetPosition)); // Move to the default target
            return;
        }

        // Access the target position from the trackingTarget (which should be set by ImageTrackingHandler)
        Vector3 targetPosition = trackingTarget.transform.position;

        GameObject snowbombWithTarget = Instantiate(snowbombPrefab, Camera.main.transform.position, Quaternion.identity);
        StartCoroutine(MoveSnowbomb(snowbombWithTarget, targetPosition)); // Move towards the actual target
    }

    private IEnumerator MoveSnowbomb(GameObject snowbomb, Vector3 target)
    {
        float duration = 2f;
        float elapsedTime = 0f;
        Vector3 startPosition = snowbomb.transform.position;

        while (elapsedTime < duration)
        {
            snowbomb.transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(snowbomb); // Destroy the snowbomb after it reaches the target

        // Only spawn snowfall and additional prefab if there's a valid target
        if (targetFlag != null && targetFlag.activeSelf)
        {
            SpawnSnowfall(target); // Spawn snowfall at the target position
            SpawnAdditionalPrefab(target); // Spawn the additional prefab after snowbomb despawns
        }
        else
        {
            Debug.Log("No target to spawn snowfall.");
        }
    }

    private void SpawnSnowfall(Vector3 position)
    {
        GameObject snowfall = Instantiate(snowfallPrefab, position, Quaternion.identity);
        snowfall.transform.SetParent(null);

        Rigidbody snowfallRb = snowfall.AddComponent<Rigidbody>();
        snowfallRb.isKinematic = true;

        Debug.Log("Snowfall started at: " + position);
    }

    private void SpawnAdditionalPrefab(Vector3 position)
    {
        if (additionalPrefab != null)
        {
            GameObject spawnedObject = Instantiate(additionalPrefab, position, Quaternion.identity);
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

            Debug.Log("✅ Additional prefab spawned and manually started at: " + position);
        }
        else
        {
            Debug.LogWarning("❌ No additional prefab assigned.");
        }
    }
}
