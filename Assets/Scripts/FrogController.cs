using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float laneDistance = 2f;   // расстояние между рядами
    [SerializeField] private float laneSwitchSpeed = 8f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float waterHeight = 0f;    // высота уровня воды

    [Header("Звук")]
    [SerializeField] private AudioManager sound;

    [Header("Анимация")]
    [SerializeField] private Animator animator;

    public GameController gc;

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right



    private void Start()
    {
        animator.Play("Idle");
    }


    void Update()
    {
        HandleInput();
        MoveForward();
        KeepOnWaterLevel();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane = Mathf.Clamp(currentLane - 1, 0, 2);
            sound.PlaySwoosh();
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane = Mathf.Clamp(currentLane + 1, 0, 2);
            sound.PlaySwoosh();
        }

        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, laneSwitchSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);
    }

    private void KeepOnWaterLevel()
    {
        // удерживаем игрока на уровне воды
        Vector3 pos = transform.position;
        pos.y = waterHeight;
        transform.position = pos;
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
