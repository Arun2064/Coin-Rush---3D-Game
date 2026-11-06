using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DesertRunnerController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 12f;
    public float jumpForce = 8f;
    public float gravity = 22f;
    public float slideDuration = 0.75f;

    private CharacterController character;
    private int lane = 1;
    private float verticalVelocity;
    private float slideTimer;
    private float normalHeight;
    private Vector3 normalCenter;
    private Camera followCamera;
    private int coins;
    private int distance;
    private bool gameOver;

    void Awake()
    {
        character = GetComponent<CharacterController>();
        normalHeight = character.height;
        normalCenter = character.center;
        followCamera = Camera.main;
    }

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) RestartRun();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            lane = Mathf.Max(0, lane - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            lane = Mathf.Min(2, lane + 1);

        if (character.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
                verticalVelocity = jumpForce;
        }
        else verticalVelocity -= gravity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            slideTimer = slideDuration;
            character.height = normalHeight * 0.55f;
            character.center = new Vector3(normalCenter.x, (character.height - normalHeight) * 0.5f, normalCenter.z);
        }

        if (slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                character.height = normalHeight;
                character.center = normalCenter;
            }
        }

        float targetX = (lane - 1) * laneDistance;
        Vector3 motion = new Vector3((targetX - transform.position.x) * laneChangeSpeed,
                                     verticalVelocity,
                                     forwardSpeed);
        character.Move(motion * Time.deltaTime);
        distance = Mathf.Max(distance, Mathf.FloorToInt(transform.position.z));

        if (followCamera != null)
        {
            Vector3 target = transform.position + new Vector3(0, 6.5f, -10f);
            followCamera.transform.position = Vector3.Lerp(followCamera.transform.position, target, 5f * Time.deltaTime);
            followCamera.transform.LookAt(transform.position + new Vector3(0, 1.2f, 9f));
        }

        if (transform.position.y < -5f) EndRun();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Finish"))
        {
            EndRun();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Hazard")) EndRun();
    }

    void EndRun()
    {
        gameOver = true;
        forwardSpeed = 0;
    }

    void RestartRun()
    {
        transform.position = new Vector3(0, 1, 4);
        lane = 1;
        verticalVelocity = 0;
        coins = 0;
        distance = 0;
        gameOver = false;
        forwardSpeed = 10f;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(18, 18, 220, 92), "");
        GUI.Label(new Rect(32, 30, 190, 22), "DESERT RUN  |  COINS: " + coins);
        GUI.Label(new Rect(32, 54, 190, 22), "DISTANCE: " + distance + " m");
        GUI.Label(new Rect(32, 78, 200, 22), "A/D or arrows  |  Space jump  |  S slide");

        if (gameOver)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 55, 300, 110), "");
            GUI.Label(new Rect(Screen.width / 2 - 110, Screen.height / 2 - 30, 240, 25), "RUN OVER  -  Distance: " + distance + " m");
            GUI.Label(new Rect(Screen.width / 2 - 110, Screen.height / 2 + 2, 240, 25), "Press R to restart");
        }
    }
}

public class DesertCoinSpin : MonoBehaviour
{
    public float spinSpeed = 120f;
    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        transform.position += Vector3.up * Mathf.Sin(Time.time * 3f) * 0.0015f;
    }
}

public class DesertCameraMarker : MonoBehaviour { }
