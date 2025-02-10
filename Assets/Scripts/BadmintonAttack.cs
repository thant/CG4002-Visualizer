using UnityEngine;

public class BadmintonAttack : MonoBehaviour
{
    public GameObject shuttlecockPrefab; // Shuttlecock prefab to instantiate
    public float attackSpeed = 15f;     // Speed at which the shuttlecock flies
    public float arcHeight = 2f;        // Height of the arc (how high the shuttlecock goes)
    public float forwardMultiplier = 1.5f; // Multiplier to make the shuttlecock travel further

    void Start()
    {
        // Any initialization code if needed
    }

    // This method will be triggered when the button is clicked to launch the badminton attack
    public void LaunchBadmintonAttack()
    {
        // Get the camera's forward direction (where the camera is facing)
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right; // Get the camera's right direction

        // Calculate spawn position at the middle right of the camera's view
        Vector3 spawnPosition = Camera.main.transform.position + cameraForward * 2f + cameraRight * 1f; // Offset to the right

        // Log spawn position for debugging
        Debug.Log("Shuttlecock spawn position: " + spawnPosition);

        // Instantiate the shuttlecock at the calculated spawn position
        GameObject shuttlecock = Instantiate(shuttlecockPrefab, spawnPosition, Quaternion.identity);

        // Log to confirm instantiation
        if (shuttlecock != null)
        {
            Debug.Log("Shuttlecock instantiated at position: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Failed to instantiate shuttlecock.");
        }

        // Calculate the direction for the shuttlecock to fly to the left and away
        Vector3 direction = -cameraRight + cameraForward;  // Fly to the left and away (opposite right and forward)

        // Adjust the direction to simulate an arc (add upward velocity)
        direction.y = arcHeight; // This adds the upward arc effect

        // Apply forward multiplier to increase the sense of depth (distance)
        direction *= forwardMultiplier;

        // Get the shuttlecock's Rigidbody and apply force to make it fly towards the target
        Rigidbody shuttlecockRigidbody = shuttlecock.GetComponent<Rigidbody>();
        if (shuttlecockRigidbody != null)
        {
            // Apply the calculated direction and speed
            shuttlecockRigidbody.linearVelocity = direction.normalized * attackSpeed;

            // Optionally, add some spin or rotation to the shuttlecock
            shuttlecockRigidbody.angularVelocity = new Vector3(0f, 10f, 0f); // Adjust rotation speed if needed
        }
        else
        {
            Debug.LogError("Shuttlecock does not have a Rigidbody.");
        }
    }
}
