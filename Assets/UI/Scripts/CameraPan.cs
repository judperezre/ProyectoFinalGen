using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public Transform target;      // El punto alrededor del cual girar�
    public float radius = 10f;    // Radio del c�rculo
    public float speed = 10f;     // Velocidad de la rotaci�n
    public Vector3 offset = new Vector3(0, 5f, 0); // Altura o ajuste vertical

    private float angle = 90f;

    void Update()
    {
        angle += speed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;

        transform.position = new Vector3(x, offset.y, z) + target.position;
        transform.LookAt(target); // Siempre mira al centro
    }
}
