using UnityEngine;

public class LilyPadSpawner : MonoBehaviour
{
    public GameObject lilyPadPrefab;
    public int poolSize = 10;       
    public float laneDistance = 2f;
    public float spawnZDistance = 5f;     

    private GameObject[] lilyPads;
    private int nextIndex = 0;

    void Start()
    {
        lilyPads = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            lilyPads[i] = Instantiate(lilyPadPrefab, Vector3.zero, Quaternion.identity, transform);
            lilyPads[i].SetActive(false);
        }

        for (int i = 0; i < 3; i++)
        {
            SpawnLilyPad(i, i * spawnZDistance);
        }
    }

    public void SpawnLilyPad(int lane, float zPosition)
    {
        GameObject pad = lilyPads[nextIndex];
        pad.SetActive(true);
        pad.transform.position = new Vector3((lane - 1) * laneDistance, 0, zPosition);
        nextIndex = (nextIndex + 1) % poolSize;
    }
}