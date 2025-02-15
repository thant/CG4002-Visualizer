using UnityEngine;
using System.Collections;

public class HoldGun : MonoBehaviour
{
    public Transform arCameraTransform; // Reference to AR Camera
    public Vector3 offsetPosition = new Vector3(0.1f, -0.2f, 0.2f); // Adjust gun position relative to camera
    public GameObject targetPositionObject; // The target position GameObject from image tracking
    public GameObject isTargetVisible; // Flag to check if the target is visible (tracked)
    public Animator gunAnimator; // Reference to the gun's Animator component


    public GameObject bulletPrefab; // Bullet prefab to be instantiated
    public float shootSpeed = 20f;  // Bullet speed

    public GameObject spawnPrefabOnHit; // Prefab to spawn at the target location when bullet reaches the target

    public float aimDownSightsRotation = 15f; // Additional rotation to simulate aiming down sights
    public float leftTiltRotation = -10f; // Tilt to the left
    public float upwardTiltRotation = 10f; // Tilt upwards

    // Bobbing parameters
    public float bobSpeed = 5f;   // Speed of the bobbing effect
    public float bobAmount = 0.1f; // Amount of bobbing (how far up/down the gun moves)
    
    private float time = 0f;  // Used for bobbing time calculation

    void Update()
    {
        // Update the gun's position based on camera's position and direction
        Vector3 cameraForward = arCameraTransform.forward;  // Get the camera's forward direction
        Vector3 cameraRight = arCameraTransform.right;      // Get the camera's right direction

        // Adjust the gun's position to be closer to the camera (less forward)
        transform.position = arCameraTransform.position + cameraForward * offsetPosition.z + cameraRight * offsetPosition.x + new Vector3(0, offsetPosition.y, 0);

        // Add the bobbing effect
        time += Time.deltaTime * bobSpeed; // Increase time for bobbing
        float bobbingEffect = Mathf.Sin(time) * bobAmount; // Use sine function for smooth up and down movement
        transform.position += new Vector3(0, bobbingEffect, 0); // Apply the bobbing effect to the Y axis

        // If the target is visible, aim the gun at the target
        if (isTargetVisible.activeSelf && targetPositionObject != null)
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = targetPositionObject.transform.position - transform.position;

            // Make sure to ignore vertical orientation (Y axis)
            directionToTarget.y = 0;  // Optional: ignore Y-axis for more stable rotation

            // Invert the direction to make sure the muzzle faces forward
            directionToTarget = -directionToTarget; // Flip the direction to point the muzzle away from you

            // Calculate the desired rotation to aim the gun at the target
            Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

            // Apply the combined rotation: rotate towards the target, with additional tilt adjustments
            transform.rotation = rotationToTarget * Quaternion.Euler(aimDownSightsRotation, 90f, leftTiltRotation) * Quaternion.Euler(upwardTiltRotation, 0f, 0f);
        }
        else
        {
            // If no target or not visible, maintain the original position and rotation of the gun
            Vector3 directionToCamera = arCameraTransform.position - transform.position;
            directionToCamera.y = 0;  // Optional: ignore Y-axis for more stable rotation
            Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);

            // Apply the default combined rotation to the gun
            transform.rotation = rotationToCamera * Quaternion.Euler(aimDownSightsRotation, 90f, leftTiltRotation) * Quaternion.Euler(upwardTiltRotation, 0f, 0f);
        }
    }

    public void Shoot()
{
    // Instantiate bullet at gun's position
    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

    // If the target is visible, shoot towards the target
    Vector3 targetPosition;

    if (isTargetVisible.activeSelf && targetPositionObject != null)
    {
        // If the target is visible, use its position as the destination
        targetPosition = targetPositionObject.transform.position;
    }
    else
    {
        // If no target, shoot straight ahead (just like the camera's forward direction)
        targetPosition = arCameraTransform.position + arCameraTransform.forward * 10f;  // Example forward position
    }

    // Call a coroutine to move the bullet to the target position
    StartCoroutine(MoveBulletToTarget(bullet, targetPosition));
}

private IEnumerator MoveBulletToTarget(GameObject bullet, Vector3 target)
{
    float duration = 0.5f;  // Adjust the duration of the bullet's flight
    float elapsedTime = 0f;
    Vector3 startPosition = bullet.transform.position;

    // Move the bullet over time using Lerp
    while (elapsedTime < duration)
    {
        bullet.transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
        elapsedTime += Time.deltaTime;

        // Wait until the next frame
        yield return null;
    }

    // After reaching the target, optionally spawn a prefab (like snowfall or an effect)
    SpawnPrefabAtTarget(target);

    // Destroy the bullet after reaching the target
    Destroy(bullet);
}

private void SpawnPrefabAtTarget(Vector3 position)
{
    // Check if the prefab to spawn is assigned
    if (spawnPrefabOnHit != null  && isTargetVisible.activeSelf)
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



    public void Reload()
{
    // Check if the gunAnimator is assigned and trigger the reload animation
    if (gunAnimator != null)
    {
        gunAnimator.SetTrigger("Reload"); // Trigger the "Reload" animation
    }
    else
    {
        Debug.LogWarning("Gun Animator not assigned!");
    }
}

}