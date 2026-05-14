using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody _rb;
    private float _timer;
    private BulletPool _pool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation |
                          RigidbodyConstraints.FreezePositionY;
    }

    public void Init(BulletPool pool)
    {
        _pool = pool;
    }

    public void OnSpawn()
    {
        _returned = false;
        _timer = lifetime;
        _rb.linearVelocity = transform.forward * speed;
        gameObject.SetActive(true);
    }

    public void OnReturn()
    {
        _rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private bool _returned;

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            Return();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Return();
        }
    }

    private void Return()
    {
        if (_returned) return;
        _returned = true;
        _pool?.ReturnBullet(this);
    }
}
