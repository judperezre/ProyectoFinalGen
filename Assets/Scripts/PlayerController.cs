using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // COMPONENTES
    private CharacterController controller;
    private TrailRenderer dashTrail;
    private Animator animator;
    private AudioSource audioSource;
    public Transform cameraTransform;
    public GameObject powerPrefab;

    // MOVIMIENTO
    [Header("Movimiento")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;

    // ROLL COLLIDER
    [Header("Roll Collider")]
    public float rollHeight = 1f;
    public Vector3 rollCenterOffset = new Vector3(0f, 0.5f, 0f);
    private float originalHeight;
    private Vector3 originalCenter;

    // DASH
    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashTapTime = 0.2f;
    public float dashDistanceFactor = 0.5f;
    private float dashTapTimer = 0f;
    private bool dashInputReady = false;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private Vector3 dashDirection;

    // VIDA
    [Header("Vida")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;
    public float projectileDamage = 10f;
    public float meleeDamage = 20f;
    public DeathScreen deathScreen;

    // ENERGÍA
    [Header("Energía")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float dashCost = 20f;
    public float specialCost = 50f;
    public float energyGainOnKill = 20f;

    // HABILIDAD ESPECIAL
    [Header("Habilidad Especial")]
    public float powerForce = 15f;
    public float specialCooldown = 3f;
    private float specialCooldownTimer = 0f;
    private bool canDamage;

    // AUDIO
    [Header("Audio")]
    public AudioClip attackClip;
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip dashClip;
    private bool isWalkingAudio = false;
    private bool isRunningAudio = false;

    // TRABAJO (animación Working)
    [Header("Trabajo")]
    [Tooltip("Distancia para activar Working al acercarse a objetos con tags específicos")]
    public float workRange = 1f;

    // CÁMARA
    [Header("Cámara")]
    public float mouseSensitivity = 100f;
    public float cameraDistance = 4f;
    public float cameraHeight = 1.5f;

    // OTROS
    private float verticalVelocity = 0f;
    private float yaw = 0f;
    private float pitch = 0f;
    private bool isDead = false;
    private bool wasGrounded = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        dashTrail = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        if (dashTrail != null) dashTrail.emitting = false;

        originalHeight = controller.height;
        originalCenter = controller.center;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        yaw = transform.eulerAngles.y;
        pitch = 0f;

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Detectar actividad de trabajo
        bool working = false;
        Collider[] workCols = Physics.OverlapSphere(transform.position, workRange);
        foreach (var col in workCols)
        {
            if (col.CompareTag("Tomatoes") || col.CompareTag("Grass") || col.CompareTag("Gallina"))
            {
                working = true;
                break;
            }
        }
        animator.SetBool("Working", working);

        // Entrada de movimiento
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0f, inputZ);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        // Gestión del salto
        bool isGrounded = controller.isGrounded;

        // 1) Aterrizaje: reset del bool Jump
        if (isGrounded && !wasGrounded)
        {
            animator.SetBool("Jump", false);
            verticalVelocity = -0.5f;
        }

        // 2) Inicio de salto al presionar Espacio
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
            animator.SetBool("Jump", true);
        }
        wasGrounded = isGrounded;

        // Calcula moveDir según cámara
        Vector3 moveDir = Vector3.zero;
        if (inputDir.magnitude > 0.01f)
        {
            Vector3 camF = cameraTransform.forward; camF.y = 0f; camF.Normalize();
            Vector3 camR = cameraTransform.right; camR.y = 0f; camR.Normalize();
            moveDir = (camF * inputDir.z + camR * inputDir.x).normalized * inputDir.magnitude;
            transform.forward = moveDir;

            // Animaciones y sonidos de caminar/correr
            bool running = Input.GetKey(KeyCode.LeftShift);
            bool walking = !running;
            animator.SetBool("Run", running);

            if (walking && moveDir.magnitude > 0.01f && !isWalkingAudio)
            {
                if (walkClip != null) audioSource.PlayOneShot(walkClip);
                isWalkingAudio = true;
            }
            else if (!walking)
                isWalkingAudio = false;

            if (running && moveDir.magnitude > 0.01f && !isRunningAudio)
            {
                if (runClip != null) audioSource.PlayOneShot(runClip);
                isRunningAudio = true;
            }
            else if (!running)
                isRunningAudio = false;
        }

        // Parámetro Speed para Idle/Walk
        animator.SetFloat("Speed", moveDir.magnitude);

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashTapTimer = 0f;
            dashInputReady = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) && !isDashing && !dashInputReady)
            moveDir *= runSpeed / walkSpeed;
        if (Input.GetKeyUp(KeyCode.LeftShift) && dashInputReady && dashTapTimer < dashTapTime)
        {
            if (currentEnergy >= dashCost)
            {
                isDashing = true;
                dashTimeLeft = dashDuration;
                currentEnergy -= dashCost;
                dashDirection = (moveDir.magnitude > 0f ? moveDir : transform.forward) * dashDistanceFactor;
                if (dashTrail != null) dashTrail.emitting = true;
                if (dashClip != null) audioSource.PlayOneShot(dashClip);
                StartRoll();
                animator.SetTrigger("Roll");
            }
            dashInputReady = false;
        }
        if (dashInputReady)
        {
            dashTapTimer += Time.deltaTime;
            if (dashTapTimer >= dashTapTime) dashInputReady = false;
        }

        // Gravedad y movimiento
        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        Vector3 velocity = moveDir * walkSpeed;
        velocity.y = verticalVelocity;
        if (isDashing)
        {
            velocity = dashDirection * dashSpeed;
            velocity.y = verticalVelocity;
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
                if (dashTrail != null) dashTrail.emitting = false;
                EndRoll();
            }
        }
        controller.Move(velocity * Time.deltaTime);

        // Ataque básico
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            if (attackClip != null) audioSource.PlayOneShot(attackClip);
        }

        // Cooldown especial
        if (specialCooldownTimer > 0f)
            specialCooldownTimer -= Time.deltaTime;

        // Habilidad especial (Spell)
        if (Input.GetKeyDown(KeyCode.C) && currentEnergy >= specialCost && specialCooldownTimer <= 0f)
        {
            currentEnergy -= specialCost;
            specialCooldownTimer = specialCooldown;
            animator.SetTrigger("Spell");
            if (powerPrefab != null)
            {



                Vector3 sp = transform.position + Vector3.up * 1.2f + transform.forward * 1f;
                Quaternion sr = Quaternion.LookRotation(transform.forward);
                GameObject proj = Instantiate(powerPrefab, sp, sr);
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = transform.forward * powerForce;
            }
        }

        // Cámara seguimiento
        float mX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        yaw += mX; pitch = Mathf.Clamp(pitch - mY, -30f, 60f);
        if (cameraTransform != null)
        {
            Vector3 tgt = transform.position + Vector3.up * cameraHeight;
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            cameraTransform.position = tgt + rot * Vector3.back * cameraDistance;
            cameraTransform.LookAt(tgt);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            TakeDamage(projectileDamage);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("MeleeEnemy"))
        {
            TakeDamage(meleeDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        deathScreen.ShowDeathScreen();
        animator.SetTrigger("Die");
        isDead = true;
    }

    private void StartRoll()
    {
        controller.height = rollHeight;
        controller.center = rollCenterOffset;
    }

    private void EndRoll()
    {
        controller.height = originalHeight;
        controller.center = originalCenter;
    }

    // Ajusta personaje al suelo tras morir
    private void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 10f))
        {
            Vector3 p = transform.position;
            p.y = hit.point.y;
            transform.position = p;
        }
    }

    public void RecoverEnergy(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    public void EnemyHit()
    {
        float attackRange = 2f;
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(25);
            }
        }
    }






}