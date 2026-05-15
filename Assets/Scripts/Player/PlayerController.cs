using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float maxHealth = 100f;

    public float Health => _currentHealth;
    public float MaxHealth => maxHealth;

    private float _currentHealth;
    private bool _isDead;

    private Rigidbody _rb;
    private IWeapon _weapon;
    private Camera _mainCamera;
    private PlayerAnimationController _animController;

    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _weapon = GetComponentInChildren<IWeapon>();
        _mainCamera = Camera.main;
        _animController = GetComponent<PlayerAnimationController>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _currentHealth = maxHealth;
        _inputActions = new InputSystem_Actions();
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;
        _currentHealth = Mathf.Max(0f, _currentHealth - amount);
        Debug.Log($"[Player] TakeDamage {amount} → HP: {_currentHealth}/{maxHealth}");
        if (_currentHealth <= 0f)
        {
            _isDead = true;
            _animController?.SetDead();
            GameEventBus.RaiseGameOver(false);
        }
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        AimAtMouse();

        bool pressing = _inputActions.Player.Attack.IsPressed();
        _animController?.SetShooting(pressing);

        if (pressing)
        {
            FireWeapon();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = _inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
        Vector3 velocity  = direction * moveSpeed;

        // Y hızını koru (yerçekimi çalışsın)
        _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
    }

    private void AimAtMouse()
    {
        if (_mainCamera == null) return;
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 lookTarget = hit.point;
            lookTarget.y = transform.position.y; // XZ düzleminde dön

            Vector3 lookDirection = (lookTarget - transform.position);
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    private void FireWeapon()
    {
        if (_weapon == null)
        {
            Debug.LogWarning("[PlayerController] FireWeapon çağrıldı ama _weapon null!");
            return;
        }
        _weapon.Fire(transform.forward);
    }
}
