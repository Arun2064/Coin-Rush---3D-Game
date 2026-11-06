using UnityEngine;

public class CoinVisual : MonoBehaviour
{
    #region Animation Settings

    [Header("Rotation")]
    public float rotationSpeed = 180f;

    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.25f;

    #endregion

    #region Private Variables

    private Vector3 startPosition;

    #endregion

    #region Unity Events

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        RotateCoin();
        FloatCoin();
    }

    #endregion

    #region Initialization

    private void Initialize()
    {
        startPosition = transform.position;
    }

    #endregion

    #region Animation

    private void RotateCoin()
    {
        transform.Rotate(
            0f,
            rotationSpeed * Time.deltaTime,
            0f,
            Space.World
        );
    }

    private void FloatCoin()
    {
        float yOffset =
            Mathf.Sin(
                Time.time * floatSpeed
            ) * floatHeight;

        transform.position =
            startPosition +
            Vector3.up * yOffset;
    }

    #endregion
}