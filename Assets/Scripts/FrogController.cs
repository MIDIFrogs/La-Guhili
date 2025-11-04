using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float laneSwitchSpeed = 8f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float waterHeight = 0f;

    //[Header("Звук")]
    //[SerializeField] private AudioManager sound;

    [Header("Анимация")]
    [SerializeField] private Animator animator;

    public GameController gc;

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right

    [Header("Прыжок")]
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpForwardBoost = 3f; // дополнительное смещение вперед при прыжке
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
        HandleJump();
    }

    private void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane = Mathf.Clamp(currentLane - 1, 0, 2);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane = Mathf.Clamp(currentLane + 1, 0, 2);
        }

        // Если прыжок не активен, просто плавное движение по X
        if (!isJumping)
        {
            float targetX = (currentLane - 1) * laneDistance;
            float newX = Mathf.Lerp(transform.position.x, targetX, laneSwitchSpeed * Time.deltaTime);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        // Прыжок по Space
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartJump();
        }
    }

    private void MoveForward()
    {
        // Всегда движемся по Z независимо от прыжка
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);
    }

    private void StartJump()
    {
        isJumping = true;
        jumpTime = 0f;
        jumpStartPos = transform.position;

        // Целевая позиция по Z с дополнительным "рывком" вперед
        jumpTargetPos = new Vector3(transform.position.x,
                                    transform.position.y,
                                    transform.position.z + forwardSpeed * jumpDuration + jumpForwardBoost);

        animator.Play("Jump");
        //sound.PlayJump();
    }

    private void HandleJump()
    {
        if (!isJumping)
        {
            // Держим на уровне воды
            Vector3 pos = transform.position;
            pos.y = waterHeight;
            transform.position = pos;
            return;
        }

        jumpTime += Time.deltaTime;
        float t = jumpTime / jumpDuration;

        // Y по параболе
        float newY = jumpStartPos.y + jumpForce * 4f * t * (1 - t);

        // X остаётся текущим (смена ряда по A/D)
        float newX = transform.position.x;

        // Z плавно вперед с учётом рывка
        float newZ = Mathf.Lerp(jumpStartPos.z, jumpTargetPos.z, t);

        transform.position = new Vector3(newX, newY, newZ);

        if (jumpTime >= jumpDuration)
        {
            isJumping = false;
            Vector3 finalPos = transform.position;
            finalPos.y = waterHeight;
            transform.position = finalPos;
            animator.Play("Idle");
            //sound.PlayLand();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Letter"))
        {
            Debug.Log("🐸 Лягушка подобрала букву: " + other.GetComponent<Letter>().letter);
            gc.OnLetterCollected(other.GetComponent<Letter>());
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
