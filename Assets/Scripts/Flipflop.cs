using UnityEngine;

public class Flipflop : MonoBehaviour
{
    public int isActive = 0; // 0 = inactive, 1 = active

    // Method to set the value of isActive
    public void SetActiveState(int state)
    {
        isActive = state;
        Debug.Log("collisionchecker state changed to: " + isActive);
    }
}
