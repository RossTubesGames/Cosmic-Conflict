using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Position")]
    public float normalDistance = 4f;
    public float aimDistance = 2.5f;
    public float height = 0.5f;

    [Header("Smoothing")]
    public float positionSmoothSpeed = 15f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        bool isAiming = Input.GetMouseButton(1);

        float currentDistance = isAiming
            ? aimDistance
            : normalDistance;

        Vector3 desiredPosition =
            target.position
            - target.forward * currentDistance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position);
    }
}