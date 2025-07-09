using UnityEngine;

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

    // DASH
    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashTapTime = 0.2f;
    private float dashTapTimer = 0f;
    private bool dashInputReady = false;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private Vector3 dashDirection;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        dashTrail = GetComponent<TrailRenderer>();
        if (dashTrail != null) dashTrail.emitting = false;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        yaw = transform.eulerAngles.y;
        pitch = 0f;
    }

    void Update()
    {
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
            if (currentEnergy >= dashCost)
            {
                isDashing = true;
                dashTimeLeft = dashDuration;
                currentEnergy -= dashCost;
                dashDirection = moveDir.magnitude > 0f ? moveDir : transform.forward;
                if (dashTrail != null) dashTrail.emitting = true;
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

    // Recuperar energía al matar enemigos
    public void RecoverEnergy(float amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
    }
}
