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
        // If we do not currently have a target,
        // try to find the spawned Player.
        if (target == null)
        {
            FindPlayerTarget();
        }

        if (target == null)
            return;

        bool isAiming = Input.GetMouseButton(1);

        float currentDistance =
            isAiming
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

    private void FindPlayerTarget()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Transform camTarget =
            player.transform.Find("CamTarget");

        if (camTarget != null)
        {
            target = camTarget;
        }
        else
        {
            Debug.LogWarning(
                "Player found, but no child called CamTarget exists."
            );
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}