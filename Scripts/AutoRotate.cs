using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Rotate 50 degrees per second on Y


    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}