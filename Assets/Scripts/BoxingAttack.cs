using UnityEngine;
using System.Collections;

public class BoxingAttack : MonoBehaviour
{
    public GameObject glovesPrefab;        // Regular gloves prefab
    public GameObject smallGlovesPrefab;   // Smaller gloves prefab for close targets
    public float despawnTime = 2f;  

    public Transform arCameraTransform; // Reference to AR Camera
    public GameObject targetPositionObject;   // External reference to the target position GameObject
    public GameObject isTargetVisible;        // Flag to check if the target is visible (tracked)

    public GameObject spawnPrefabOnHit; // Prefab to spawn at the target location when bullet reaches the target

    // This method launches the boxing attack
    public void LaunchBoxingAttack()
{
    // Check the target position and get the proximity information
    Vector3 targetPosition = GetTargetPosition(out bool isTargetClose);

    // Determine which gloves prefab to use based on the proximity
    GameObject glovesPrefabToUse = isTargetClose ? smallGlovesPrefab : glovesPrefab;

    // Set gloves' position directly in front of the camera
    Vector3 glovesPosition = arCameraTransform.position;

    // Instantiate the gloves at the calculated position (relative to camera)
    GameObject gloves = Instantiate(glovesPrefabToUse, glovesPosition, arCameraTransform.rotation * Quaternion.Euler(0, 90, 0));

    // Force it to stay active and correctly positioned
    gloves.SetActive(true);
    gloves.transform.position = glovesPosition;

    // Get Animator and Play Animation
    Animator glovesAnimator = gloves.GetComponent<Animator>();
    if (glovesAnimator != null)
    {
        glovesAnimator.Rebind(); // Reset animation state
        glovesAnimator.Update(0); // Ensure it's in the correct pose
        glovesAnimator.SetTrigger("PlayBoxingAnimation");
        Debug.Log("Boxing animation triggered.");
    }
    else
    {
        Debug.LogError("No Animator found on boxing gloves prefab!");
    }

    // Start the gloves' movement behavior towards the target
    StartCoroutine(MoveGlovesToTarget(gloves, targetPosition, isTargetClose));

    // Destroy the gloves after a set time
    Destroy(gloves, despawnTime);
}

// This function decides where the gloves should move to and checks for proximity
private Vector3 GetTargetPosition(out bool isTargetClose)
{
    // Default to a far distance if no target is visible
    isTargetClose = false;
    Vector3 targetPosition = arCameraTransform.position + arCameraTransform.forward * 10f;  // Default far position

    if (isTargetVisible.activeSelf && targetPositionObject != null)
    {
        // If the target is visible, use its position
        targetPosition = targetPositionObject.transform.position;

        // Check distance to decide if it's close
        float distance = Vector3.Distance(arCameraTransform.position, targetPosition);
        if (distance < 2f)  // Threshold distance for "close" (can adjust)
        {
            isTargetClose = true;
        }
    }

    return targetPosition;
}

// Coroutine to move the gloves towards the target position, just like shooting logic
private IEnumerator MoveGlovesToTarget(GameObject gloves, Vector3 target, bool isTargetClose)
{
    float duration = 1f;  // Duration of the movement (adjust as needed)
    float elapsedTime = 0f;
    Vector3 startPosition = gloves.transform.position;

    // If the target is close, we might want to speed up the movement
    if (isTargetClose)
    {
        duration = 0.5f;  // Faster move if target is close
    }

    // Move the gloves over time using Lerp
    while (elapsedTime < duration)
    {
        gloves.transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
        elapsedTime += Time.deltaTime;

        // Wait until the next frame
        yield return null;
    }

    SpawnPrefabAtTarget(target);

    // Destroy the gloves after the movement is complete
    Destroy(gloves, despawnTime);
}


    private void SpawnPrefabAtTarget(Vector3 position)
    {
        // Check if the prefab to spawn is assigned
        if (spawnPrefabOnHit != null && isTargetVisible.activeSelf)
        {
            // Spawn the prefab at the target position
            GameObject spawnedObject = Instantiate(spawnPrefabOnHit, position, Quaternion.identity);
            
            // Check if the spawned object has a ParticleSystem to play
            ParticleSystem ps = spawnedObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play(); // Manually start the particle effect
                Destroy(spawnedObject, ps.main.duration + ps.main.startLifetime.constantMax); // Destroy after effect ends
            }
            else
            {
                Destroy(spawnedObject, 5f); // Fallback destroy after 5 seconds if no particle system
            }

            Debug.Log("Prefab spawned at target: " + position);
        }
        else
        {
            Debug.LogWarning("❌ No prefab assigned to spawn on hit.");
        }
    }
}
