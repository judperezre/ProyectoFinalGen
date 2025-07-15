using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // COMPONENTES
    private CharacterController controller;
    private TrailRenderer dashTrail;
    private Animator animator;
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

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        dashTrail = GetComponent<TrailRenderer>();
        if (dashTrail != null) dashTrail.emitting = false;

        originalHeight = controller.height;
        originalCenter = controller.center;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        yaw = transform.eulerAngles.y;
        pitch = 0f;

        // Inicializar vida
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // Si está muerto, solo aplica gravedad para caer al suelo
        if (isDead)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
            controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
            return;
        }

        // Entrada de movimiento
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0f, inputZ);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        // Calcula moveDir según cámara
        Vector3 moveDir = Vector3.zero;
        if (inputDir.magnitude > 0.01f)
        {
            Vector3 camF = cameraTransform.forward; camF.y = 0f; camF.Normalize();
            Vector3 camR = cameraTransform.right; camR.y = 0f; camR.Normalize();
            moveDir = (camF * inputDir.z + camR * inputDir.x).normalized * inputDir.magnitude;
            transform.forward = moveDir;
            // Run animation
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && moveDir.magnitude > 0.01f;
            animator.SetBool("Run", isRunning);
        }

        // Idle/Walk
        animator.SetFloat("Speed", moveDir.magnitude);

        // Salto
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -0.5f;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
            animator.SetTrigger("Jump");
        }

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
            if (currentHealth > dashCost)
            {
                isDashing = true;
                dashTimeLeft = dashDuration;
                currentEnergy -= dashCost;
                dashDirection = (moveDir.magnitude > 0f ? moveDir : transform.forward) * dashDistanceFactor;
                if (dashTrail != null) dashTrail.emitting = true;
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

        // Ataque
        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger("Attack");

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

        // Cámara
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
        animator.SetTrigger("Die");
        isDead = true;
        // Al morir, ajusta posición para quedar en el suelo
        SnapToGround();
        {
            animator.SetTrigger("Die");
            isDead = true;
        }
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
        // Raycast desde un poco arriba para evitar colisión inmediata
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
}