using System.Collections;
using UnityEngine;
using Vuforia;

public class SnowBombAttack : MonoBehaviour
{
    public GameObject snowbombPrefab;
    public GameObject snowfallPrefab;
    public GameObject additionalPrefab; // The new prefab to spawn after snowbomb despawns
    public float launchSpeed = 2f;

    public GameObject trackingTarget; // The GameObject that contains the flag and target position
    public GameObject targetFlag;     // Reference to the target flag (set in the inspector)

    public void LaunchSnowbomb()
    {
        if (!targetFlag.activeSelf)
        {
            Debug.Log("No valid target for Snowbomb, launching forward without target.");

            Vector3 defaultTargetPosition = Camera.main.transform.position + Camera.main.transform.forward * 10f;
            GameObject snowbomb = Instantiate(snowbombPrefab, Camera.main.transform.position, Quaternion.identity);
            StartCoroutine(MoveSnowbomb(snowbomb, defaultTargetPosition));
            return;
        }

        Vector3 targetPosition = trackingTarget.transform.position;
        GameObject snowbombWithTarget = Instantiate(snowbombPrefab, Camera.main.transform.position, Quaternion.identity);
        StartCoroutine(MoveSnowbomb(snowbombWithTarget, targetPosition));
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

        Destroy(snowbomb);

        if (targetFlag != null && targetFlag.activeSelf)
        {
            SpawnSnowfallWithAnchor(target);
            SpawnAdditionalPrefab(target);
        }
        else
        {
            Debug.Log("No target to spawn snowfall.");
        }
    }

    private void SpawnSnowfallWithAnchor(Vector3 position)
    {
        // Create a parent GameObject to act as an anchor
        GameObject anchorObject = new GameObject("SnowfallAnchor");
        anchorObject.transform.position = position;

        // Add Vuforia's AnchorBehaviour to lock it in space
        AnchorBehaviour anchorBehaviour = anchorObject.AddComponent<AnchorBehaviour>();
        anchorBehaviour.enabled = true;

        // Spawn the snowfall and parent it to the anchored object
        GameObject snowfall = Instantiate(snowfallPrefab, position, Quaternion.identity, anchorObject.transform);

        Rigidbody snowfallRb = snowfall.AddComponent<Rigidbody>();
        snowfallRb.isKinematic = true;

        Debug.Log("📌 Snowfall spawned and anchored using Vuforia AnchorBehaviour at: " + position);
    }

    private void SpawnAdditionalPrefab(Vector3 position)
    {
        if (additionalPrefab != null)
        {
            GameObject spawnedObject = Instantiate(additionalPrefab, position, Quaternion.identity);
            ParticleSystem ps = spawnedObject.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                ps.Play();
                Destroy(spawnedObject, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(spawnedObject, 5f);
            }

            Debug.Log("✅ Additional prefab spawned and manually started at: " + position);
        }
        else
        {
            Debug.LogWarning("❌ No additional prefab assigned.");
        }
    }
}
