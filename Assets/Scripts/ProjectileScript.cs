using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 15;
    private Collider player;
    public bool canDamage;
    void Start()
    {
       player = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        StartDamage();
        OnTriggerEnter(player);
    }

    private void StartDamage()
    {
        canDamage = true;
        GetComponent<Collider>().enabled = true;
    }
    private void StopDamage()
    {
        canDamage = false;
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage == true && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null) 
            {
                
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
                StopDamage();
            }
        }
        
    }
}
