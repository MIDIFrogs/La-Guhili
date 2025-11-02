using UnityEngine;

public class EndlessRiver : MonoBehaviour
{
    [Header("References")]
    public Transform player;         
    public GameObject waterPrefab;

    public GameObject grassPrefab;

    [Header("Settings")]
    public int segmentCount = 3;     

    //water segments
    private GameObject[] segments;
    private float segmentLength;
    private float segmentWidth;

    void Start()
    {

        getWaterSegmentLength();
        getWaterSegmentWidth();


        // 2️⃣ Создаём сегменты подряд по оси z (вдаль)
        segments = new GameObject[segmentCount];


        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = new Vector3(0, 0, i * segmentLength);
            segments[i] = Instantiate(waterPrefab, pos, Quaternion.identity, transform);

            PlaceGrassAtEdges(pos);
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


    private void getWaterSegmentLength()
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

    }

    private void getWaterSegmentWidth()
    {
        MeshRenderer renderer = waterPrefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            segmentWidth = renderer.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("Water prefab has no MeshRenderer — using default segment length 10");
            segmentWidth = 10f;
        }

    }


    private void PlaceGrassAtEdges(Vector3 segmentPosition)
    {
        float grassOffsetX = segmentWidth + 0.01f; // ширина + небольшой зазор
        float grassOffsetY = -0.05f; // чуть ниже, чтобы избежать мерцания

        Vector3 leftGrassPos = segmentPosition + new Vector3(-grassOffsetX, grassOffsetY, 0);

        GameObject leftGrass = Instantiate(grassPrefab, leftGrassPos, Quaternion.identity, transform);

        Vector3 rightGrassPos = segmentPosition + new Vector3(grassOffsetX, grassOffsetY, 0);

        GameObject кшпреGrass = Instantiate(grassPrefab, rightGrassPos, Quaternion.identity, transform);

    }

}
