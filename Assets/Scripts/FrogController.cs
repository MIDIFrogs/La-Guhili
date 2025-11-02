using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Движение")]
    public float laneDistance = 2f;   // расстояние между рядами
    public float laneSwitchSpeed = 8f;
    public float forwardSpeed = 5f;
    public float waterHeight = 0f;    // высота уровня воды

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right

    void Update()
    {
        HandleInput();
        MoveForward();
        KeepOnWaterLevel();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Clamp(currentLane - 1, 0, 2);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Clamp(currentLane + 1, 0, 2);

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
            Debug.Log("🐸 Лягушка подобрала букву: " + other.name);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Debug.Log("💥 Лягушка столкнулась с препятствием!");
            Destroy(other.gameObject);
        }
    }
}
