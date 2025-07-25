using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 6;
    private Collider player;
    public bool canDamage;
    public GameObject projectileVFX;
    void Start()
    {
       player = GetComponent<Collider>();
        Destroy(gameObject, 10f);

        if (projectileVFX != null)
        {
            GameObject vfx = Instantiate(projectileVFX, transform.position, transform.rotation);
            vfx.transform.SetParent(transform);
            vfx.transform.Rotate(-90f, 0f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartDamage();
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
