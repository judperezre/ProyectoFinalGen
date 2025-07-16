using UnityEngine;

public class PlayerPruebaPortal : MonoBehaviour
{
    public float velocidad = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(horizontal, 0, vertical);
        transform.Translate(movimiento * velocidad * Time.deltaTime);
    }
}
