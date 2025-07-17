using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // El jugador o el punto que sigue la cámara
    public Vector3 offset = new Vector3(0, 5, -7);  // Ajusta según el ángulo deseado
    public float smoothSpeed = 5f;  // Qué tan rápido sigue

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target); // Opcional, para que siempre mire al jugador
    }
}
