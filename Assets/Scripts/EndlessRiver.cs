using UnityEngine;

public class EndlessRiver : MonoBehaviour
{
    [Header("References")]
    public Transform player;         
    public GameObject waterPrefab;    

    [Header("Settings")]
    public int segmentCount = 3;     

    private GameObject[] segments;
    private float segmentLength;

    void Start()
    {
        MeshRenderer renderer = waterPrefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            segmentLength = renderer.bounds.size.z; 
        }
        else
        {
            Debug.LogWarning("Water prefab has no MeshRenderer — using default segment length 10");
            segmentLength = 10f;
        }

        // 2️⃣ Создаём сегменты подряд
        segments = new GameObject[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = new Vector3(0, 0, i * segmentLength);
            segments[i] = Instantiate(waterPrefab, pos, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            if (player.position.z - segments[i].transform.position.z > segmentLength)
            {
                segments[i].transform.position += Vector3.forward * segmentLength * segmentCount;
            }
        }
    }
}
