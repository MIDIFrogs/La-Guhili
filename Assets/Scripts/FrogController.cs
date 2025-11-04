using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float laneSwitchSpeed = 8f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float acceleration = 0.02f;
    [SerializeField] private float maxForwardSpeed = 30f;
    [SerializeField] private float waterHeight = 0f;

    [Header("Звук")]
    [SerializeField] private AudioManager audioManager;

    [Header("Анимация")]
    [SerializeField] private Animator animator;

    public GameController gc;

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right

    [Header("Прыжок")]
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpForwardBoost = 3f;
    private bool isJumping = false;
    private float jumpTime;
    private Vector3 jumpStartPos;
    private Vector3 jumpTargetPos;

    private void Start()
    {
        animator.Play("Idle");
    }

    private void Update()
    {
        HandleLaneInput();
        MoveForward();
        AccelerateForward();
        HandleJump();
    }

    private void HandleLaneInput()
    {
        int previousLane = currentLane; // сохраняем текущий ряд до изменения

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane = Mathf.Clamp(currentLane - 1, 0, 2);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane = Mathf.Clamp(currentLane + 1, 0, 2);
        }

        // Воспроизводим звук перемещения по рядам только если ряд изменился
        if (previousLane != currentLane && audioManager != null)
        {
            if (previousLane == 0 && currentLane == 1) audioManager.PlaySmoothFromLeftToCenter();
            else if (previousLane == 1 && currentLane == 0) audioManager.PlaySmoothFromCenterToLeft();
            else if (previousLane == 1 && currentLane == 2) audioManager.PlaySmoothFromCenterToRight();
            else if (previousLane == 2 && currentLane == 1) audioManager.PlaySmoothFromRightToCenter();
        }

        // Смена ряда всегда доступна, даже при прыжке
        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, laneSwitchSpeed * Time.deltaTime);
        Vector3 pos = transform.position;
        pos.x = newX;
        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartJump();
        }
    }


    private void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);
    }

    private void AccelerateForward()
    {
        forwardSpeed += acceleration * Time.deltaTime;
        forwardSpeed = Mathf.Min(forwardSpeed, maxForwardSpeed);
    }

    private void StartJump()
    {
        isJumping = true;
        jumpTime = 0f;
        jumpStartPos = transform.position;
        jumpTargetPos = new Vector3(transform.position.x,
                                    transform.position.y,
                                    transform.position.z + forwardSpeed * jumpDuration + jumpForwardBoost);

        animator.Play("Jump");
        audioManager.PlayJump();
    }

    private void HandleJump()
    {
        if (!isJumping)
        {
            Vector3 pos = transform.position;
            pos.y = waterHeight;
            transform.position = pos;
            return;
        }

        jumpTime += Time.deltaTime;
        float t = jumpTime / jumpDuration;

        // Парабола Y
        float newY = jumpStartPos.y + jumpForce * 4f * t * (1 - t);
        float newZ = Mathf.Lerp(jumpStartPos.z, jumpTargetPos.z, t);

        transform.position = new Vector3(transform.position.x, newY, newZ);

        if (jumpTime >= jumpDuration)
        {
            isJumping = false;
            Vector3 finalPos = transform.position;
            finalPos.y = waterHeight;
            transform.position = finalPos;
            animator.Play("Idle");
            audioManager.PlayLanding();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Letter"))
        {
            Debug.Log("🐸 Лягушка подобрала букву: " + other.GetComponent<Letter>().letter);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Debug.Log("💥 Лягушка столкнулась с препятствием!");
            gc.OnObstacleHit();
            Destroy(other.gameObject);
        }
    }
}
