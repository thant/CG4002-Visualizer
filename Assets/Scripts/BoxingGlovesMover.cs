using UnityEngine;
using System.Collections;

public class BoxingGlovesMover : MonoBehaviour
{
    public float moveSpeed = 10.0f; // Adjust for realistic movement
    private Vector3 targetPosition; // Position to move towards
    private bool isMovingToTarget = false;

    public void StartMoving(Vector3 targetPos = default)
    {
        if (targetPos != default)
        {
            // If a target position is provided, move towards it
            targetPosition = targetPos;
            isMovingToTarget = true;
            StartCoroutine(MoveToTarget());
        }
        else
        {
            // Otherwise, just move forward relative to the camera
            StartCoroutine(MoveForward());
        }
    }

    private IEnumerator MoveForward()
    {
        float duration = 1.5f;  // How long to move forward
        float elapsedTime = 0f;

        Vector3 moveDirection = Camera.main.transform.forward; // Move forward relative to the camera

        while (elapsedTime < duration)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveToTarget()
    {
        while (transform.position != targetPosition)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
