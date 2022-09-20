using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyTarget : MonoBehaviour, IDamageable
{
  [HideInInspector]public float maxHp;
  [HideInInspector] public float currentMaxHp;
  [HideInInspector] public float currentEffectMax;
  public float currentHp;
  public float currentEffect;
    public float atkSpd;
    public float dmg;
    public float spd;
    public float range;
    public Transform player;
    public float turnRate;
    public bool forced;
    [System.NonSerialized]
    public UnityEvent<float> takeDamageEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateCurrentMaxHpEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateEffectEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateEffectMaxEvent;
    ObjectPooler objectPooler;
    private Collider dummyCollider;
    private Rigidbody rb;
    public PlayerControllStates ControllStates;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(Shot());
        dummyCollider = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        currentMaxHp = maxHp;
        currentHp = currentMaxHp;
        if (takeDamageEvent == null)
        {
            takeDamageEvent = new UnityEvent<float>();
        }
        if (updateCurrentMaxHpEvent == null)
        {
            updateCurrentMaxHpEvent = new UnityEvent<float>();
        }
        if (updateEffectEvent == null)
        {
            updateEffectEvent = new UnityEvent<float>();
        }
        if (updateEffectMaxEvent == null)
        {
            updateEffectMaxEvent = new UnityEvent<float>();
        }
    }
    private void Update()
    {
        if (ControllStates==PlayerControllStates.Stunned)
        {
            UpdateEffectProgress();
        }
        else
        {
            transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));

        }
    }
    public IEnumerator Slowdown()
    {
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = true;
        forced = false;

    }
    public void DecreaseHp(float value)
    {
        currentHp -= value;
        currentHp = Mathf.Clamp(currentHp, 0, currentMaxHp);
        takeDamageEvent.Invoke(currentHp);
    }
    public void UpdateCurrentMaxHp(float value)
    {
        currentMaxHp = value;
        currentMaxHp = Mathf.Clamp(currentMaxHp, 40, maxHp);
        updateCurrentMaxHpEvent.Invoke(currentMaxHp);
    }
    public void UpdateEffectProgress()
    {
        currentEffect -= Time.deltaTime;
        updateEffectEvent.Invoke(currentEffect);
    }
    public void UpdateEffectProgressMax(float value)
    {
        updateEffectMaxEvent.Invoke(value);
    }
    public void TakeDamage(float amount, DamageTypes damageTypes, float debuffTime)
    {
        switch (damageTypes)
        {
            case DamageTypes.None:
                break;
            case DamageTypes.Normal:
                ControllStates = PlayerControllStates.Normal;
                DecreaseHp(amount);
                StartCoroutine(ClearDamageEffects(debuffTime));
                ClampHpValues();
                break;
            case DamageTypes.Stun:
                ControllStates = PlayerControllStates.Stunned;
                StartCoroutine(ClearDamageEffects(debuffTime));
                UpdateEffectProgressMax(debuffTime);
                currentEffect = debuffTime;
                DecreaseHp(amount);
                ClampHpValues();
                break;
            case DamageTypes.Silence:
                break;
            case DamageTypes.Slow:
                break;
            case DamageTypes.Overtime:
                break;
            default:
                break;
        }
    }
    public IEnumerator ClearDamageEffects(float time)
    {
        switch (ControllStates)
        {
            case PlayerControllStates.None:
                break;
            case PlayerControllStates.Normal:
                yield return new WaitForSeconds(time);
                ControllStates = PlayerControllStates.None;
                break;
            case PlayerControllStates.Stunned:
                yield return new WaitForSeconds(time);
                currentEffect = 0;
                UpdateEffectProgressMax(currentEffect);
                ControllStates = PlayerControllStates.None;
                break;
            case PlayerControllStates.Silenced:
                yield return new WaitForSeconds(time);
                ControllStates = PlayerControllStates.None;
                break;
            case PlayerControllStates.Slowed:
                yield return new WaitForSeconds(time);
                ControllStates = PlayerControllStates.None;
                break;
            case PlayerControllStates.DamagedOvertime:
                yield return new WaitForSeconds(time);
                ControllStates = PlayerControllStates.None;
                break;
        }
    }
    private void ClampHpValues()
    {
        if (currentMaxHp - currentHp >= 40)
        {
            UpdateCurrentMaxHp(currentHp + 40);
        }
    }
    public void GetKnockBack(Vector3 _dir,float _power)
    {
        rb.isKinematic = false;
        forced = true;
        rb.AddForce(_dir*_power, ForceMode.VelocityChange);
        StartCoroutine(Slowdown());
    }
    public IEnumerator Shot()
    {
        yield return new WaitForSeconds(atkSpd); 
        GameObject projectile = objectPooler.SpawnFromPool("DummyProjectile", transform.position, transform.rotation);
        projectile.GetComponent<ProjectileBehavior>()._damage = dmg;
        projectile.GetComponent<ProjectileBehavior>().Type = DamageTypes.Normal;
        projectile.GetComponent<ProjectileBehavior>().dir = transform.forward;
        projectile.GetComponent<ProjectileBehavior>()._speed = spd;
        projectile.GetComponent<ProjectileBehavior>()._maxRange = range;
        projectile.GetComponent<ProjectileBehavior>().projectileOwner = transform.gameObject;
        Physics.IgnoreCollision(dummyCollider, projectile.GetComponent<Collider>(), true);
        StartCoroutine(Shot());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (forced && collision.gameObject.CompareTag("Wall"))
        {
            TakeDamage(0, DamageTypes.Stun, 2);
            rb.isKinematic = true;
            forced = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
