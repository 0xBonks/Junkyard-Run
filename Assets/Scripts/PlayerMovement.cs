using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 65f;

    private CharacterController controller;
    private float verticalSpeed;
    private float cameraPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
        {
            cameraPitch = cameraTransform.localEulerAngles.x;
            if (cameraPitch > 180f)
                cameraPitch -= 360f;
        }
    }

    private void Start()
    {
        SetCursorLocked(true);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetCursorLocked(false);
        }

        if (Mouse.current != null &&
            Cursor.lockState != CursorLockMode.Locked &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetCursorLocked(true);
        }

        if (Mouse.current != null &&
            cameraTransform != null &&
            Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            transform.Rotate(0f, mouseDelta.x, 0f);

            cameraPitch = Mathf.Clamp(
                cameraPitch - mouseDelta.y,
                minPitch,
                maxPitch
            );

            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.wKey.isPressed) input.y += 1;
        }

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        verticalSpeed += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalSpeed * Time.deltaTime);
    }

    private void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
