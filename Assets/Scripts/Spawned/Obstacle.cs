using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var gc = other.GetComponent<GameController>();
        if (gc != null)
        {
            gc.OnObstacleHit();
            Destroy(gameObject);
        }
    }
}
