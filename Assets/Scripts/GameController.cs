using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public ObjectSpawner spawner;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            int randomRow = Random.Range(0, 3);
            int randomType = Random.Range(0, 2); // 0 = letter, 1 = obstacle

            if (randomType == 0)
            {
                if (randomRow == 0) spawner.SpawnOnLeftRow(spawner.letterPrefab);
                if (randomRow == 1) spawner.SpawnOnCenterRow(spawner.letterPrefab);
                if (randomRow == 2) spawner.SpawnOnRightRow(spawner.letterPrefab);
            }
            else
            {
                if (randomRow == 0) spawner.SpawnOnLeftRow(spawner.obstaclePrefab);
                if (randomRow == 1) spawner.SpawnOnCenterRow(spawner.obstaclePrefab);
                if (randomRow == 2) spawner.SpawnOnRightRow(spawner.obstaclePrefab);
            }
        }
    }
}
