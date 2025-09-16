using UnityEngine;

public class AmbienceRotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed;

    void Update()
    {
        transform.localRotation *= Quaternion.Euler(_rotationSpeed * Time.deltaTime);
    }
}
