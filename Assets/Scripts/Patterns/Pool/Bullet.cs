using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float speed    = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage   = 10f;

    private Rigidbody _rb;
    public string ShooterName { get; set; } = "Unknown";
    public int    TypeIndex   { get; private set; }
    public void   SetTypeIndex(int i) => TypeIndex = i;
    private float _timer;
    private bool _returned;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation |
                          RigidbodyConstraints.FreezePositionY;
    }

    public void OnSpawn()
    {
        _returned = false;
        _timer    = lifetime;
        gameObject.SetActive(true);
        _rb.linearVelocity = transform.forward * speed;
    }

    public void OnReturn()
    {
        _rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f) Return();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target != null) { target.TakeDamage(damage, ShooterName); Return(); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable target = collision.gameObject.GetComponentInParent<IDamageable>();
        if (target != null) { target.TakeDamage(damage, ShooterName); Return(); }
    }

    private void Return()
    {
        if (_returned) return;
        _returned = true;
        BulletPool.Instance?.ReturnBullet(this);
    }
}
