using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HumanDetection : MonoBehaviour
{
    public ARHumanBodyManager humanBodyManager;  // Assign in Inspector
    public Transform detectedHumanTransform;  // This stores the position of the detected human

    void Update()
    {
        foreach (var human in humanBodyManager.trackables)
        {
            if (human != null)
            {
                detectedHumanTransform.position = human.transform.position;
                Debug.Log("Human detected at: " + human.transform.position);
                return;
            }
        }
    }
}
