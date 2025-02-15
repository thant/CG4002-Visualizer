using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingHandler : MonoBehaviour
{
    public GameObject targetObject; // The GameObject to update with tracked image position
    public GameObject targetFlag;   // The empty GameObject that acts as a flag (true/false)
    private ARTrackedImageManager imageManager;
    private ARTrackedImage currentTrackedImage;
    private Vector3 targetPosition;

    void Awake()
    {
        imageManager = Object.FindFirstObjectByType<ARTrackedImageManager>();
        if (imageManager == null)
        {
            Debug.LogError("❌ ARTrackedImageManager not found in the scene.");
        }
    }

    void OnEnable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Iterate through all newly added or updated images
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                SetTarget(trackedImage); // Set target when a new image is tracked
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                SetTarget(trackedImage); // Update target if the image is still tracked
            }
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                // If image tracking is lost, reset and deactivate
                if (trackedImage == currentTrackedImage)
                {
                    ResetTarget(); // Reset target position when tracking is lost
                }
            }
            // This part checks if the image was in Limited state and comes back to Tracking state
            else if (trackedImage.trackingState == TrackingState.Tracking && trackedImage != currentTrackedImage)
            {
                // Re-trigger SetTarget if the image comes back into view and is now tracked
                SetTarget(trackedImage);
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            // Handle image removal
            if (trackedImage == currentTrackedImage)
            {
                ResetTarget(); // Reset target if the image is removed
            }
        }
    }

    private void SetTarget(ARTrackedImage trackedImage)
    {
        // Set the current tracked image and position
        currentTrackedImage = trackedImage;
        targetPosition = trackedImage.transform.position;

        // Activate the flag to indicate a valid target
        targetFlag.SetActive(true);

        // Update the target object position and activate it if not active
        targetObject.transform.position = targetPosition;
        if (!targetObject.activeSelf)
        {
            targetObject.SetActive(true);
        }

        //Debug.Log($"✅ Image Tracked! Target Position: {targetObject.transform.position}");
    }

    private void ResetTarget()
    {
        // Deactivate the flag to indicate no valid target
        targetFlag.SetActive(false);

        // Reset the target when tracking is lost or image is removed
        targetObject.transform.position = Vector3.zero; // Reset position
        Debug.Log("❌ Image lost tracking or removed. Target reset.");
    }
}
