using UnityEngine;

public class FrogController : MonoBehaviour
{
    public float laneDistance = 2f;
    public float moveSpeed = 10f;
    public SimpleWaterFlow riverFlow;

    private int currentLane = 1;
    private Vector3 targetPosition;
    private Transform currentLilyPad;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // Смена рядов
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveLane(1);

        Vector3 rowPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
        targetPosition = new Vector3(rowPosition.x, rowPosition.y, targetPosition.z);


        targetPosition += new Vector3(0, 0, riverFlow.currentFlowSpeedZ * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void MoveLane(int direction)
    {
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, 0, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LilyPad"))
        {
            currentLilyPad = other.transform;
            targetPosition.y = currentLilyPad.position.y + 0.5f; 
        }
    }
}
