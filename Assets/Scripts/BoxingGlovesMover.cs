using UnityEngine;
using System.Collections;

public class BoxingGlovesMover : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Adjust for realistic movement

    public void StartMoving()
    {
        StartCoroutine(MoveForward());
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
}
