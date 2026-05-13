using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundLayerMask;

    private Rigidbody _rb;
    private IWeapon _weapon;
    private Camera _mainCamera;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _weapon = GetComponentInChildren<IWeapon>();
        _mainCamera = Camera.main;

        // Rigidbody fizik ayarları: rotation'ı dondur, gravity'yi koru
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        AimAtMouse();

        if (Input.GetMouseButton(0))
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 velocity  = direction * moveSpeed;

        // Y hızını koru (yerçekimi çalışsın)
        _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
    }

    private void AimAtMouse()
    {
        if (_mainCamera == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

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
        if (_weapon == null) return;

        _weapon.Fire(transform.forward);
    }
}
