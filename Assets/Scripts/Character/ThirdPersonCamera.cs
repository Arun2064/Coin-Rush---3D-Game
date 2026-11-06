using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    #region Target

    [Header("Target")]
    public Transform target;

    #endregion

    #region Camera Settings

    [Header("Camera Settings")]
    public float distance = 5f;
    public float height = 2f;
    public float sensitivity = 3f;
    public float smoothSpeed = 10f;

    #endregion

    #region Rotation Limits

    [Header("Pitch Limits")]
    public Vector2 pitchLimits =
        new Vector2(-30f, 60f);

    #endregion

    #region Private Variables

    private float yaw;
    private float pitch;

    #endregion

    #region Unity Events

    private void Start()
    {
        InitializeCamera();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleCameraInput();
        UpdateCameraPosition();
    }

    #endregion

    #region Initialization

    private void InitializeCamera()
    {
        if (target == null)
        {
            Debug.LogError(
                "ThirdPersonCamera: Target not assigned."
            );

            enabled = false;
            return;
        }

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    #endregion

    #region Camera Input

    private void HandleCameraInput()
    {
        if (Time.timeScale <= 0f)
            return;

        float currentSensitivity =
            PlayerPrefs.GetFloat(
                "MouseSensitivity",
                sensitivity
            );

        yaw +=
            Input.GetAxis("Mouse X") *
            currentSensitivity;

        pitch -=
            Input.GetAxis("Mouse Y") *
            currentSensitivity;

        pitch = Mathf.Clamp(
            pitch,
            pitchLimits.x,
            pitchLimits.y
        );
    }

    #endregion

    #region Camera Movement

    private void UpdateCameraPosition()
    {
        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 desiredPosition =
            target.position
            - rotation * Vector3.forward * distance
            + Vector3.up * height;

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

        transform.rotation = rotation;
    }

    #endregion
}