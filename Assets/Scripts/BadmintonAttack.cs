using UnityEngine;
using System.Collections;

public class BadmintonAttack : MonoBehaviour
{
    public GameObject shuttlecockPrefab;        //shuttlecock prefab
    public GameObject smallShuttlecockPrefab;   //Smaller shuttlecock prefab for close targets
    public float attackSpeed = 15f;             //Speed at which the shuttlecock flies
    public float arcHeight = 2f;                //Base height of the arc
    public float forwardMultiplier = 1.5f;     //Multiplier to make the shuttlecock travel further

    public GameObject targetPositionObject;    // External reference to target position GameObject
    public GameObject isTargetVisible;         // Flag to check if the target is visible (tracked)

    public GameObject spawnPrefabOnHit;        // hit vfx

    void Start()
    {
    }
    public void LaunchBadmintonAttack()
    {
        // Get the camera's forward direction
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right; // Get the camera's right direction

        // spawn position at the middle right of the camera's view
        Vector3 spawnPosition = Camera.main.transform.position + cameraRight * 0.2f;
        spawnPosition.y -= 0.2f;

        Debug.Log("Shuttlecock spawn position: " + spawnPosition);

        // Determine if the target is close
        bool isTargetClose = false;
        Vector3 targetPosition = GetTargetPosition(out isTargetClose);  // Get target position and proximity check

        // Choose the shuttlecock prefab based on proximity
        GameObject shuttlecockPrefabToUse = isTargetClose ? smallShuttlecockPrefab : shuttlecockPrefab;

        // Instantiate the shuttlecock at the calculated spawn position
        GameObject shuttlecock = Instantiate(shuttlecockPrefabToUse, spawnPosition, Quaternion.identity);

        // Log to confirm instantiation
        if (shuttlecock != null)
        {
            Debug.Log("Shuttlecock instantiated at position: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Failed to instantiate shuttlecock.");
        }

        // Calculate the direction for the shuttlecock to fly
        Vector3 direction;

        if (isTargetVisible.activeSelf && targetPositionObject != null)
        {
            // If the target is visible, calculate the direction to the target
            direction = targetPosition - spawnPosition;  // Direction to the target
        }
        else
        {
            // If no target is visible, shoot straight ahead and apply an arc
            direction = cameraForward;  // Fly straight forward relative to the camera
            direction.y = arcHeight;    // Add the upward arc effect
        }
        StartCoroutine(MoveShuttlecockToTarget(shuttlecock, spawnPosition, direction, isTargetClose));
    }

    private Vector3 GetTargetPosition(out bool isTargetClose)
    {
        isTargetClose = false; // Default to not close
        Vector3 targetPosition = Vector3.zero;

        if (isTargetVisible.activeSelf && targetPositionObject != null)
        {
            targetPosition = targetPositionObject.transform.position;
            float distance = Vector3.Distance(Camera.main.transform.position, targetPosition);
            if (distance < 2f)  // Threshold distance for "close" (can adjust)
            {
                isTargetClose = true;
            }
        }

        return targetPosition;
    }

    private IEnumerator MoveShuttlecockToTarget(GameObject shuttlecock, Vector3 spawnPosition, Vector3 direction, bool isTargetClose)
    {
        float duration = 2f; // Default duration of the movement
        if (isTargetClose)
        {
            duration = 1f; // Faster move if target is close
        }

        float elapsedTime = 0f;
        Vector3 startPosition = spawnPosition;

        // Move the shuttlecock over time using Lerp
        while (elapsedTime < duration)
        {
            shuttlecock.transform.position = Vector3.Lerp(startPosition, startPosition + direction, elapsedTime / duration);

            // Shuttlecock spin on its axis
            shuttlecock.transform.Rotate(Vector3.forward * 360 * Time.deltaTime / duration, Space.Self); // Rotation around Z-axis
            shuttlecock.transform.Rotate(Vector3.up * 360 * Time.deltaTime / duration, Space.Self); // Rotation around Y-axis

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After the shuttlecock reaches the target, spawn vfx
        Vector3 targetPosition = startPosition + direction;
        SpawnPrefabAtTarget(targetPosition);
        Destroy(shuttlecock, 0.5f);
    }

    private void SpawnPrefabAtTarget(Vector3 position)
    {
        // Check if the prefab to spawn is assigned
        if (spawnPrefabOnHit != null && isTargetVisible.activeSelf)
        {
            // Spawn the prefab at the target position
            GameObject spawnedObject = Instantiate(spawnPrefabOnHit, position, Quaternion.identity);
            
            // Check if the spawned object has a ParticleSystem to play (vfx)
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
