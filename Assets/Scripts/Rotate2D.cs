using UnityEngine;

public class Rotate2D : MonoBehaviour
{
    public float rotationSpeed;

    void FixedUpdate()
    {
        float angle = rotationSpeed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
