using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;

public class SnowBombAttack : MonoBehaviour
{
    public GameObject snowbombPrefab;  // The snowbomb projectile
    public GameObject snowfallPrefab;  // The snowfall effect
    public float launchSpeed = 2f;     // Speed of snowbomb movement

    private Vector3 targetPosition;    // Position to send snowbomb
    private bool hasTarget = false;    // Check if we have a valid target
    private ARTrackedImageManager imageManager; // AR Tracked Image Manager
    private ARTrackedImage currentTrackedImage; // Current tracked image being used

    void Start()
    {
        // Use FindFirstObjectByType to get ARTrackedImageManager from the scene
        imageManager = Object.FindFirstObjectByType<ARTrackedImageManager>();
        if (imageManager == null)
        {
            Debug.LogError("ARTrackedImageManager not found in the scene.");
        }
    }

    void Update()
    {
        if (imageManager != null)
        {
            CheckTrackedImages();
        }
    }

    private void CheckTrackedImages()
    {
        // Iterate through all tracked images
        foreach (ARTrackedImage trackedImage in imageManager.trackables)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // Set the target position to the tracked image position if found
                targetPosition = trackedImage.transform.position;
                currentTrackedImage = trackedImage;
                hasTarget = true;
                break; // Only use the first image found
            }
            else
            {
                // If the image is no longer tracked, remove its effect
                if (trackedImage == currentTrackedImage)
                {
                    hasTarget = false;
                }
            }
        }
    }

    public void LaunchSnowbomb()
    {
        if (!hasTarget)
        {
            Debug.Log("No tracked image detected, cannot launch snowbomb!");
            return;
        }

        GameObject snowbomb = Instantiate(snowbombPrefab, Camera.main.transform.position, Quaternion.identity);
        StartCoroutine(MoveSnowbomb(snowbomb, targetPosition));
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
        SpawnSnowfall(target);
    }

    private void SpawnSnowfall(Vector3 position)
    {
        GameObject snowfall = Instantiate(snowfallPrefab, position, Quaternion.identity);
        snowfall.transform.SetParent(null); // Keeps it anchored in world space

    // Optionally scale the snowfall effect to fit the AR scene better
        float scaleFactor = 0.5f;  // Adjust this value to scale the snowfall effect
        snowfall.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

    // Optionally add Rigidbody and Collider for physical anchoring
        Rigidbody snowfallRb = snowfall.AddComponent<Rigidbody>();
        snowfallRb.isKinematic = true;  // Ensures it stays in place after falling

        Debug.Log("Snowfall started at: " + position);
    }
}
