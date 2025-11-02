using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SimpleWaterFlow : MonoBehaviour
{
    public float scrollSpeedX = 0.1f; 
    public float scrollSpeedY = 0.005f; 
    public float acceleration = 0.01f;
    [HideInInspector]
    public float currentFlowSpeedZ = 0.1f; 
    public string textureName = "_BaseMap";

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // плавное ускорение
        scrollSpeedX += acceleration * Time.deltaTime;
        scrollSpeedY += acceleration * Time.deltaTime;

        // обновляем смещение текстуры
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;
        rend.material.SetTextureOffset(textureName, offset);

        currentFlowSpeedZ += acceleration * Time.deltaTime;
    }
}