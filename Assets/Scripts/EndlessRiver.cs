using UnityEngine;
using System.Collections.Generic;

public class EndlessRiverFixed : MonoBehaviour
{
    public Transform player;
    public GameObject riverSegmentPrefab;
    public int initialSegments = 3;
    public float yOffset = -0.5f;   // сдвиг сегмента вниз
    public float playerHeight = 0.5f; // высота игрока над водой

    private List<GameObject> segments = new List<GameObject>();
    private float segmentLength;

    void Start()
    {
        // Длина сегмента по Z (только вода)
        MeshRenderer renderer = riverSegmentPrefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            segmentLength = renderer.bounds.size.z;
        }
        else
        {
            segmentLength = 10f;
        }

        // Создаём начальные сегменты
        for (int i = 0; i < initialSegments; i++)
        {
            Vector3 pos = new Vector3(0, yOffset, i * segmentLength);
            GameObject segment = Instantiate(riverSegmentPrefab, pos, Quaternion.identity, transform);
            AddCollider(segment);
            segments.Add(segment);
        }

        // Спавн игрока в центре первого сегмента
        player.position = new Vector3(0, yOffset + playerHeight, 0);
    }

    void Update()
    {
        GameObject lastSegment = segments[segments.Count - 1];

        // Когда игрок приближается к концу последнего сегмента, создаём новый
        if (player.position.z + segmentLength / 2f > lastSegment.transform.position.z)
        {
            // Смещаем новый сегмент на длину сегмента (учитывая Pivot в центре)
            Vector3 newPos = lastSegment.transform.position + Vector3.forward * segmentLength;
            GameObject newSegment = Instantiate(riverSegmentPrefab, newPos, Quaternion.identity, transform);
            AddCollider(newSegment);
            segments.Add(newSegment);

            // Удаляем старый сегмент, если он далеко позади
            if (segments.Count > initialSegments + 2)
            {
                Destroy(segments[0]);
                segments.RemoveAt(0);
            }
        }
    }

    private void AddCollider(GameObject segment)
    {
        if (segment.GetComponent<Collider>() == null)
        {
            MeshRenderer r = segment.GetComponent<MeshRenderer>();
            float width = r ? r.bounds.size.x : 10f;
            BoxCollider bc = segment.AddComponent<BoxCollider>();
            bc.size = new Vector3(width, 0.1f, segmentLength);
            bc.center = Vector3.zero; // центр коллайдера совпадает с Pivot
        }
    }
}
