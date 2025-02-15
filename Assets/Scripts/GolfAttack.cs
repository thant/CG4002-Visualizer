using UnityEngine;
using System.Collections;

public class GolfAttack : MonoBehaviour
{
    public GameObject golfBallPrefab;         // The regular golf ball prefab to instantiate
    public GameObject smallGolfBallPrefab;    // The smaller golf ball prefab for close targets
    public GameObject golfClubPrefab;         // The golf club prefab to instantiate
    public float attackSpeed = 15f;           // Speed at which the golf ball flies
    public float forwardMultiplier = 1.5f;   // Multiplier to make the golf ball travel further
    public float despawnTime = 2f;            // Time after which the golf club will despawn

    public GameObject targetPositionObject;  // External reference to the target position GameObject
    public GameObject isTargetVisible;       // Flag to check if the target is visible (tracked)
    public GameObject spawnPrefabOnHit;      // Prefab to spawn at the target position when the golf ball reaches it

    void Start()
    {
        // Any initialization code if needed
    }

    // This method will be triggered when the button is clicked to launch the golf attack
    public void LaunchGolfAttack()
{
    // Get the camera's forward direction (where the camera is facing)
    Vector3 cameraForward = Camera.main.transform.forward;
    Vector3 cameraRight = Camera.main.transform.right; // Get the camera's right direction

    // Calculate spawn position for the golf ball at the middle right of the camera's view (unchanged)
    Vector3 spawnPosition = Camera.main.transform.position + cameraRight * 0.2f; // Offset to the right

    // Log spawn position for debugging (for the golf ball)
    Debug.Log("Golf ball spawn position: " + spawnPosition);

    // Check if the target is close or far
    bool isTargetClose = false;
    Vector3 targetPosition = GetTargetPosition(out isTargetClose);

    // Choose the appropriate golf ball prefab based on the target proximity
    GameObject golfBallPrefabToUse = isTargetClose ? smallGolfBallPrefab : golfBallPrefab;

    // Calculate spawn position for the golf club in front of the camera
    Vector3 golfClubSpawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 1f; // 1f can be adjusted for distance

    // Instantiate the golf club prefab at the spawn position in front of the camera
    GameObject golfClub = Instantiate(golfClubPrefab, golfClubSpawnPosition, Camera.main.transform.rotation);
    golfClub.transform.localScale *= 0.08f; // Scale down the golf club to fit the screen
    golfClub.SetActive(true);

    // Play the golf club animation (assuming the prefab has an Animator component)
    Animator golfClubAnimator = golfClub.GetComponent<Animator>();
    if (golfClubAnimator != null)
    {
        golfClubAnimator.Rebind(); // Reset animation state
        golfClubAnimator.Update(0); // Ensure it's in the correct pose
        golfClubAnimator.SetTrigger("PlayGolfSwingAnimation");  // Trigger the golf swing animation
        Debug.Log("Golf club animation triggered.");
    }
    else
    {
        Debug.LogError("No Animator found on golf club prefab!");
    }

    // Destroy the golf club after the animation time (adjust according to animation length)
    Destroy(golfClub, despawnTime);

    // Calculate the direction for the golf ball to fly
    Vector3 direction;

    if (isTargetVisible.activeSelf && targetPositionObject != null)
    {
        // If the target is visible, calculate the direction to the target
        direction = targetPosition - spawnPosition;  // Direction to the target
    }
    else
    {
        // If no target is visible, shoot straight ahead
        direction = cameraForward;  // Fly straight ahead
    }

    // Apply the forward multiplier to the direction vector (scale the direction to match the desired distance)
    direction *= forwardMultiplier;

    // Instantiate the golf ball
    GameObject golfBall = Instantiate(golfBallPrefabToUse, spawnPosition, Quaternion.identity);

    // Start a coroutine to move the golf ball towards the target with rotation
    StartCoroutine(MoveGolfBallToTarget(golfBall, spawnPosition, direction));
}

    private IEnumerator MoveGolfBallToTarget(GameObject golfBall, Vector3 spawnPosition, Vector3 direction)
    {
        float duration = 1f;  // Duration of the movement (adjust as needed)
        float elapsedTime = 0f;
        Vector3 startPosition = golfBall.transform.position;

        // Move the golf ball over time using Lerp
        while (elapsedTime < duration)
        {
            golfBall.transform.position = Vector3.Lerp(startPosition, startPosition + direction, elapsedTime / duration);

            // Rotate the golf ball around both the Z and Y axes during movement
            golfBall.transform.Rotate(Vector3.forward * 360 * Time.deltaTime / duration, Space.Self); // Rotate around Z-axis
            golfBall.transform.Rotate(Vector3.up * 360 * Time.deltaTime / duration, Space.Self);    // Rotate around Y-axis

            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the golf ball reaches the target position at the end of the movement
        golfBall.transform.position = startPosition + direction;

        // Optionally, spawn some effect or do something else here
        SpawnPrefabAtTarget(startPosition + direction);

        // Destroy the golf ball after it travels for a while
        Destroy(golfBall, 0.5f); // Adjust time as needed
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
                ps.Play(); // Start particle system effect
                Destroy(spawnedObject, ps.main.duration + ps.main.startLifetime.constantMax); // Clean up after effect
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

    // Helper function to determine the target position and proximity
    private Vector3 GetTargetPosition(out bool isTargetClose)
    {
        isTargetClose = false;
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 10f; // Default far position

        if (isTargetVisible.activeSelf && targetPositionObject != null)
        {
            // If the target is visible, use its position
            targetPosition = targetPositionObject.transform.position;

            // Check distance to decide if it's close
            float distance = Vector3.Distance(Camera.main.transform.position, targetPosition);
            if (distance < 2f)  // Threshold distance for "close"
            {
                isTargetClose = true;
            }
        }

        return targetPosition;
    }
}
