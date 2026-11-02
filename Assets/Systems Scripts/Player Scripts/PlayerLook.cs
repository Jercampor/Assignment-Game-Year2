using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    private PlayerInputReader input;
    private float pitch;

    private void Awake()
    {
        input = GetComponent<PlayerInputReader>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 look = input.Look * sensitivity;

        // Left/right: rotate the whole player
        transform.Rotate(Vector3.up * look.x);

        // Up/down: tilt only the camera, clamped so you can't flip over
        pitch = Mathf.Clamp(pitch - look.y, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}