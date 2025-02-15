using UnityEngine;

public class FencingAttack : MonoBehaviour
{
    public GameObject swordPrefab;            // The fencing sword prefab
    public GameObject swordAtTargetPrefab;    // The new prefab to spawn at the target position when target is visible
    public float spawnDistance = 1.5f;        // Distance in front of the camera where the sword will spawn
    public float despawnTime = 2f;            // Time after which the sword and spawned prefab will despawn

    public GameObject targetPositionObject;   // External reference to the target position GameObject
    public GameObject isTargetVisible;        // Flag to check if the target is visible (tracked)

    public void LaunchFencingAttack()
    {
        // Default spawn position in front of the camera
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;

        // Check if the target is visible, and spawn a different prefab at the target position if visible
        GameObject spawnedAtTarget = null;
        if (isTargetVisible.activeSelf && targetPositionObject != null)
        {
            // Spawn the separate prefab at the target position
            spawnedAtTarget = Instantiate(swordAtTargetPrefab, targetPositionObject.transform.position, Quaternion.identity);
            Debug.Log("Target visible, spawned new prefab at target position.");
        }
        else
        {
            Debug.Log("Target not visible, spawning sword in front of the camera.");
        }

        // Instantiate the fencing sword in front of the camera (or at target position as needed)
        GameObject sword = Instantiate(swordPrefab, spawnPosition, Camera.main.transform.rotation * Quaternion.Euler(45, 45, 45));

        // Force it to stay active and correctly positioned
        sword.SetActive(true);
        sword.transform.position = spawnPosition;

        // Get Animator and Play Animation
        Animator swordAnimator = sword.GetComponent<Animator>();
        if (swordAnimator != null)
        {
            swordAnimator.Rebind(); // Reset animation state
            swordAnimator.Update(0); // Ensure it's in the correct pose
            swordAnimator.SetTrigger("PlayFencingAnimation");
            Debug.Log("Fencing animation triggered.");
        }
        else
        {
            Debug.LogError("No Animator found on fencing sword prefab!");
        }

        // Destroy the sword after the specified despawn time
        Destroy(sword, despawnTime);

        // If we spawned a prefab at the target position, destroy it after the same despawn time
        if (spawnedAtTarget != null)
        {
            Destroy(spawnedAtTarget, despawnTime);
        }
    }
}
