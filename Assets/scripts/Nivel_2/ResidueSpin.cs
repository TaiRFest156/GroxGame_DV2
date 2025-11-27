using UnityEngine;

public class ResidueSpin : MonoBehaviour
{
    public float rotationSpeed = 90f; // grados por segundo

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
