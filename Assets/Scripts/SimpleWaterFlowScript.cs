using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SimpleWaterFlow : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.05f;
    public float accelerationZ = 0.1f; // ускорение течения
    public string textureName = "_BaseMap";

    [HideInInspector]
    public float currentFlowSpeedZ = 0.5f; // начальная скорость течения

    void Update()
    {
        // двигаем текстуру
        Vector2 offset = new Vector2(scrollSpeedX * Time.time, -scrollSpeedY * Time.time);
        GetComponent<Renderer>().material.SetTextureOffset(textureName, offset);

        // ускорение течения постепенно
        currentFlowSpeedZ += accelerationZ * Time.deltaTime;
    }
}
