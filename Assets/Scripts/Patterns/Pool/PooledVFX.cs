using UnityEngine;

/// <summary>
/// Pool-friendly wrapper around a ParticleSystem prefab. Plays on spawn,
/// returns itself to VFXPool when the particle system finishes (IsAlive == false).
/// Lifetime is driven entirely by the particle data — no manual duration needed.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PooledVFX : MonoBehaviour, IPoolable
{
    public int  TypeIndex { get; private set; }
    public void SetTypeIndex(int i) => TypeIndex = i;

    private ParticleSystem _ps;
    private bool           _returned;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        if (_ps == null)
            _ps = GetComponentInChildren<ParticleSystem>();
    }

    public void OnSpawn()
    {
        _returned = false;
        gameObject.SetActive(true);
        if (_ps != null)
        {
            _ps.Clear(true);
            _ps.Play(true);
        }
    }

    public void OnReturn()
    {
        if (_ps != null)
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_returned || _ps == null) return;
        if (!_ps.IsAlive(true))
        {
            _returned = true;
            VFXPool.Instance?.Return(this);
        }
    }
}
