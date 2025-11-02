using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         
    public Vector3 offset = new Vector3(0, 5, -8); 
    public float smoothSpeed = 5f;     

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            offset.x,                                     
            target.position.y + offset.y,                  
            target.position.z + offset.z                   
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        //transform.rotation = Quaternion.Euler(20f, 0f, 0f); 
    }
}
