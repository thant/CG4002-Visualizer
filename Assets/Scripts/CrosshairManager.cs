using M2MqttUnity.Examples;
using UnityEngine;
using Vuforia; // Import Vuforia namespace for Image Target tracking

public class CrosshairManager : MonoBehaviour
{
    public GameObject crosshairPrefab; // The crosshair prefab to spawn
    public GameObject targetPosition; // The external GameObject that stores the transform of the image target
    private GameObject activeCrosshair; // Reference to the spawned crosshair object

    private ImageTargetBehaviour imageTargetBehaviour; // Reference to the Image Target's behaviour component

    void OnEnable()
    {
        GameObject.Find("MQTTHandler").GetComponent<M2MqttUnityTest>().Visibility(1);

        // Check if the targetPosition is assigned before spawning the crosshair
        if (targetPosition != null && crosshairPrefab != null)
        {
            SpawnCrosshairAtTarget();
        }
    }

    void OnDisable()
    {
        GameObject.Find("collisionchecker").GetComponent<Flipflop>().SetActiveState(0);
        GameObject.Find("MQTTHandler").GetComponent<M2MqttUnityTest>().Visibility(0);

        // Unsubscribe from the OnTargetStatusChanged event
        if (imageTargetBehaviour != null)
        {
            imageTargetBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }

        DestroyCrosshair();
    }

    // This method will be called when the target's status changes
    private void OnTargetStatusChanged(ObserverBehaviour observerBehaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && status.StatusInfo == StatusInfo.NORMAL)
        {
            // Image target is now being tracked, spawn the crosshair
            SpawnCrosshairAtTarget();
        }
        else
        {
            // Image target is no longer tracked, destroy the crosshair
            DestroyCrosshair();
        }
    }

    // Method to instantiate the crosshair at the target position
    private void SpawnCrosshairAtTarget()
    {
        if (crosshairPrefab != null && targetPosition != null)
        {
            // Define the desired rotation (in this case, -90 on X, 90 on Y, 0 on Z)
            Quaternion rotation = Quaternion.Euler(-90, 0f, 0f);

            // Spawn the crosshair with the specified rotation at the targetPosition
            activeCrosshair = Instantiate(crosshairPrefab, targetPosition.transform.position, rotation);
        }
    }

    // Method to destroy the crosshair if it exists
    public void DestroyCrosshair()
    {
        if (activeCrosshair != null)
        {
            Destroy(activeCrosshair);
            activeCrosshair = null;
        }
    }
}
