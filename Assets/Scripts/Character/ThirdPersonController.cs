using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    #region Movement Settings

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float gravity = -9.81f;

    #endregion

    #region References

    [Header("References")]
    public Transform cameraTransform;

    #endregion

    #region Player State

    [Header("Player State")]
    public bool canMove = true;

    #endregion

    #region Private Variables

    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity;
    private bool wasMoving;

    #endregion

    #region Unity Events

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        if (!canMove)
        {
            HandleDisabledMovement();
            return;
        }

        HandleMovement();
        HandleGravity();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        controller =
            GetComponent<CharacterController>();

        animator =
            GetComponent<Animator>();

        if (
            cameraTransform == null &&
            Camera.main != null
        )
        {
            cameraTransform =
                Camera.main.transform;
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        AudioManager.instance?.PlayIdle();
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector3 inputDirection =
            new Vector3(
                horizontal,
                0f,
                vertical
            );

        float inputMagnitude =
            Mathf.Clamp01(
                inputDirection.magnitude
            );

        UpdateAnimation(inputMagnitude);

        bool isMoving =
            inputMagnitude > 0.1f;

        HandleMovementAudio(isMoving);

        if (!isMoving)
            return;

        RotatePlayer(inputDirection);
        MovePlayer(inputDirection);
    }

    private void RotatePlayer(
        Vector3 inputDirection)
    {
        float targetAngle =
            Mathf.Atan2(
                inputDirection.x,
                inputDirection.z
            ) *
            Mathf.Rad2Deg +
            cameraTransform.eulerAngles.y;

        Quaternion targetRotation =
            Quaternion.Euler(
                0f,
                targetAngle,
                0f
            );

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }

    private void MovePlayer(
        Vector3 inputDirection)
    {
        float targetAngle =
            Mathf.Atan2(
                inputDirection.x,
                inputDirection.z
            ) *
            Mathf.Rad2Deg +
            cameraTransform.eulerAngles.y;

        Vector3 moveDirection =
            Quaternion.Euler(
                0f,
                targetAngle,
                0f
            ) *
            Vector3.forward;

        controller.Move(
            moveDirection *
            moveSpeed *
            Time.deltaTime
        );
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity +=
                gravity *
                Time.deltaTime;
        }

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }

    #endregion

    #region Animation

    private void UpdateAnimation(
        float speed)
    {
        if (animator == null)
            return;

        animator.SetFloat(
            "Speed",
            speed,
            0.1f,
            Time.deltaTime
        );
    }

    #endregion

    #region Audio

    private void HandleMovementAudio(
        bool isMoving)
    {
        if (isMoving && !wasMoving)
        {
            AudioManager.instance?.PlayWalk();
        }
        else if (!isMoving && wasMoving)
        {
            AudioManager.instance?.PlayIdle();
        }

        wasMoving = isMoving;
    }

    #endregion

    #region State

    private void HandleDisabledMovement()
    {
        UpdateAnimation(0f);

        if (wasMoving)
        {
            AudioManager.instance?.StopGameplayAudio();
            wasMoving = false;
        }
    }

    #endregion
}