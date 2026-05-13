using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int MoveX      = Animator.StringToHash("MoveX");
    private static readonly int MoveZ      = Animator.StringToHash("MoveZ");
    private static readonly int Speed      = Animator.StringToHash("Speed");
    private static readonly int IsDead     = Animator.StringToHash("IsDead");
    private static readonly int IsShooting = Animator.StringToHash("IsShooting");

    private Animator  _animator;
    private Rigidbody _rb;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb       = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 worldVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        Vector3 localVel = transform.InverseTransformDirection(worldVel);
        float   speed    = localVel.magnitude;

        float normalX = speed > 0.001f ? localVel.x / speed : 0f;
        float normalZ = speed > 0.001f ? localVel.z / speed : 0f;

        _animator.SetFloat(MoveX, normalX);
        _animator.SetFloat(MoveZ, normalZ);
        _animator.SetFloat(Speed, speed);
    }

    public void SetShooting(bool value)
    {
        _animator.SetBool(IsShooting, value);
    }

    public void SetDead()
    {
        _animator.SetBool(IsDead, true);
    }
}
