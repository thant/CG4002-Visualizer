using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject collisionChecker; // Reference to the GameObject to be toggled

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snowfall"))
        {
            // Find the "collisionchecker" within the snowfall object or in the scene
            collisionChecker = GameObject.Find("collisionchecker");

            if (collisionChecker != null)
            {
                collisionChecker.GetComponent<Flipflop>().SetActiveState(1);
                Debug.Log("Crosshair entered the snowfall area.");
            }Debug.Log(collisionChecker.GetComponent<Flipflop>().isActive);
            }
            //Debug.Log("Crosshair entered the snowfall area.");
        }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Snowfall"))
        {
            if (collisionChecker != null)
            {
                collisionChecker.GetComponent<Flipflop>().SetActiveState(0);
            }Debug.Log(collisionChecker.GetComponent<Flipflop>().isActive);
        }
    }
}