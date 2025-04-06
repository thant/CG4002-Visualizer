using UnityEngine;
using Vuforia;
using System.Collections;

public class ImageTrackingHandler : DefaultObserverEventHandler
{
    public GameObject targetObject;  // The object to update based on the tracked image position
    public GameObject targetFlag;    // The flag to indicate the tracking status

    private ImageTargetBehaviour imageTargetBehaviour;
    public float scaleFactor = 10f; // Scaling factor for world space

    // Parameters for position stabilization
    public float positionStabilizationTime = 1f; // Time in seconds to check if the position has stabilized
    public float positionChangeThreshold = 0.01f; // Minimum change in position to consider it "stabilized"

    private CrosshairManager arrowprefab;

    protected override void Start()
    {
        base.Start();

        imageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

        if (imageTargetBehaviour == null)
        {
            Debug.LogError("❌ ImageTargetBehaviour not found. Make sure the script is attached to the Image Target.");
            return;
        }

        imageTargetBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour observerBehaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && status.StatusInfo == StatusInfo.NORMAL)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        Debug.Log("🟢 Tracking Found!");


        if (targetObject != null)
            targetObject.SetActive(true);

        // Start coroutine to wait for valid and stable image target position
        StartCoroutine(WaitForValidImageTargetPosition());
    }

    private IEnumerator WaitForValidImageTargetPosition()
    {
        Vector3 previousPosition = Vector3.zero;
        Vector3 currentPosition = imageTargetBehaviour.transform.position;
        float timeElapsed = 0f;

        // Wait until the position stabilizes (the change is below the threshold for a given amount of time)
        while (timeElapsed < positionStabilizationTime)
        {
            currentPosition = imageTargetBehaviour.transform.position;

            // Check if the position has changed significantly
            if (Vector3.Distance(currentPosition, previousPosition) < positionChangeThreshold)
            {
                timeElapsed += Time.deltaTime; // Accumulate time if the position has stabilized
            }
            else
            {
                timeElapsed = 0f; // Reset the timer if the position changes significantly
            }

            previousPosition = currentPosition;

            yield return null; // Wait one frame
        }

        // After stabilization, apply the position to the targetObject
        Vector3 targetPosition = currentPosition * scaleFactor;

        if (targetObject != null)
        {
            targetObject.transform.position = targetPosition;
            Debug.Log($"✅ Target Object Position Updated: {targetObject.transform.position}");
        }

        if (targetFlag != null)
            targetFlag.SetActive(true);
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        Debug.Log("🔴 Tracking Lost!");

        if (targetFlag != null)
            targetFlag.SetActive(false);

        if (targetObject != null)
        {
            targetObject.transform.position = Vector3.zero;
            targetObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        base.OnDestroy();

        if (imageTargetBehaviour != null)
        {
            imageTargetBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}
