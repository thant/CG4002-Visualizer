using UnityEngine;

public class BoxingAttack : MonoBehaviour
{
    public GameObject glovesPrefab;  
    public float spawnDistance = 1.5f;  
    public float despawnTime = 2f;  

    public void LaunchBoxingAttack()
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;
        GameObject gloves = Instantiate(glovesPrefab, spawnPosition, Camera.main.transform.rotation * Quaternion.Euler(0, 90, 0));


        // Force it to stay active and correctly positioned
        gloves.SetActive(true);
        gloves.transform.position = spawnPosition;

        // Debugging: Confirm object exists
        Debug.Log("Boxing gloves spawned at: " + spawnPosition);

        // Get Animator and Play Animation
        Animator glovesAnimator = gloves.GetComponent<Animator>();
        if (glovesAnimator != null)
        {
            glovesAnimator.Rebind(); // Reset animation state
            glovesAnimator.Update(0); // Ensure it's in correct pose
            glovesAnimator.SetTrigger("PlayBoxingAnimation");
            Debug.Log("Boxing animation triggered.");
        }
        else
        {
            Debug.LogError("No Animator found on boxing gloves prefab!");
        }

        gloves.AddComponent<BoxingGlovesMover>().StartMoving();

        // Destroy gloves after animation
        Destroy(gloves, despawnTime);
    }
}
