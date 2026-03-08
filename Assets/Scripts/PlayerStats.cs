using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class PlayerStats : MonoBehaviour
{
    [Header("Health:")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;

    [Header("Stamina:")]
    public float maxStamina = 100f;
    public float currentStamina;
    [Tooltip("How much stamina is consumed per second while sprinting.")]
    public float staminaDrainRate = 15f;
    [Tooltip("How much stamina is recovered per second when not sprinting.")]
    public float staminaRegenRate = 10f;
    [Tooltip("Speed multiplier applied when sprinting.")]
    public float sprintMultiplier = 1.5f;
    public Image staminaBarFill;
    [Tooltip("Stamina drain per one jump")]
    public float jumpStaminaCost = 15f;


    [Header("Input:")]
    public InputActionReference sprintAction;

    private Player _playerMovement;
    private bool _isDead = false;
    private bool _isSprinting = false;
    private bool _isExhausted = false;

    private void Awake()
    {
        _playerMovement = GetComponent<Player>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        if (sprintAction != null)
        {
            sprintAction.action.Enable();
        }
    }
    private void Update()
    {
        if (_isDead) return;

        HandleStamina();
        UpdateUI();
    }
    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        currentHealth -= damage;
        Debug.Log($"CODE_LOG: Player took {damage} damage. Current Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        if (_isDead) return;
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        Debug.Log($"CODE_LOG: Player healed {amount}. Current Health: {currentHealth}/{maxHealth}");
    }
    private void HandleStamina()
    {
        bool wantsToSprint = sprintAction != null && sprintAction.action.ReadValue<float>() > 0.1f;
        Vector2 horizontalVelocity = new(_playerMovement.velocity.x, _playerMovement.velocity.z);
        bool isMoving = horizontalVelocity.magnitude > 0.1f;

        if (currentStamina <= 0f)
        {
            _isExhausted = true;
            currentStamina = 0f;
        }
        else if (currentStamina >= maxStamina * 0.25f)
        {
            _isExhausted = false;
        }
        if (wantsToSprint && isMoving && !_isExhausted)
        {
            _isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            _isSprinting = false;

            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }
    }
    private void Die()
    {
        _isDead = true;
        currentHealth = 0;
        Debug.Log("Player died");

        if (_playerMovement != null)
            _playerMovement.enabled = false;
        if (MissionManager.Instance != null)
            MissionManager.Instance.FailMission("KIA: Killed in action.");
    }
    private void UpdateUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
        if(staminaBarFill != null)
            staminaBarFill.fillAmount = currentStamina / maxStamina;
    }
    private void ApplySprintMultiplier()
    {
        if (_isSprinting)
            _playerMovement.speedMultiplier *= sprintMultiplier;
    }
    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount;

        if(currentStamina < 0f)
        {
            currentStamina = 0f; 
            _isExhausted = true;
        }
    }
    private void OnDestroy()
    {
        if (sprintAction != null)
            sprintAction.action.Disable();
    }
    private void OnEnable()
    {
        if (_playerMovement != null)
            _playerMovement.OnBeforeMove += ApplySprintMultiplier;
    }
    private void OnDisable()
    {
        if (_playerMovement != null)
            _playerMovement.OnBeforeMove -= ApplySprintMultiplier;
    }
}