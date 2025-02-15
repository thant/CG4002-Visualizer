using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public GameObject crosshairPrefab; // The crosshair prefab to spawn
    public GameObject targetPosition; // The GameObject that follows the tracked image
    private GameObject activeCrosshair; // Reference to the spawned crosshair object

    void OnEnable()
    {
        // Check if the target position is assigned before spawning the crosshair
        if (targetPosition != null && crosshairPrefab != null)
        {
            SpawnCrosshairAtTarget();
        }
    }

    void OnDisable()
    {
        // When the InView GameObject is disabled, destroy the crosshair
        DestroyCrosshair();
    }

    // Method to instantiate the crosshair at the target position
    private void SpawnCrosshairAtTarget()
    {
        if (crosshairPrefab != null && targetPosition != null)
        {
            // Define the desired rotation (in this case, -90 on X, 90 on Y, 0 on Z)
            Quaternion rotation = Quaternion.Euler(-90f, 90f, 0f);

            // Spawn the crosshair with the specified rotation
            activeCrosshair = Instantiate(crosshairPrefab, targetPosition.transform.position, rotation);
        }
    }

    // Method to destroy the crosshair if it exists
    private void DestroyCrosshair()
    {
        if (activeCrosshair != null)
        {
            Destroy(activeCrosshair);
            activeCrosshair = null;
        }
    }

    void Update()
    {
        // Update the position of the crosshair to match the targetPosition
        if (activeCrosshair != null && targetPosition != null)
        {
            activeCrosshair.transform.position = targetPosition.transform.position;
        }
    }
}
