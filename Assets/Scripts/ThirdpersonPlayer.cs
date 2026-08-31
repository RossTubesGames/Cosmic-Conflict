using UnityEngine;

public class ThirdpersonPlayer : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform;
    public Transform cameraTarget;
    public Transform groundCheck;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 12f;

    [Header("Jumping")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;
    public float minCameraAngle = -40f;
    public float maxCameraAngle = 70f;

    private float verticalVelocity;

    private float cameraYaw;
    private float cameraPitch;

    private void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraYaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        HandleCameraRotation();
        HandleMovement();
        HandleCursor();
    }

    private void HandleMovement()
    {
        bool grounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDirection = Vector3.zero;

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection =
                cameraForward * inputDirection.z +
                cameraRight * inputDirection.x;

            moveDirection.Normalize();

            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : walkSpeed;

        controller.Move(
            moveDirection * currentSpeed * Time.deltaTime
        );

        if (Input.GetButtonDown("Jump") && grounded)
        {
            verticalVelocity =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }

    private void HandleCameraRotation()
    {
        float mouseX =
            Input.GetAxis("Mouse X") * mouseSensitivity;

        float mouseY =
            Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraYaw += mouseX;
        cameraPitch -= mouseY;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minCameraAngle,
            maxCameraAngle
        );

        cameraTarget.rotation =
            Quaternion.Euler(
                cameraPitch,
                cameraYaw,
                0f
            );
    }

    private void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}