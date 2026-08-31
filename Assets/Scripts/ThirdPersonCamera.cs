using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Position")]
    public float distance = 4f;
    public float height = 0.5f;

    [Header("Smoothing")]
    public float positionSmoothSpeed = 15f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            target.position
            - target.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position);
    }
}