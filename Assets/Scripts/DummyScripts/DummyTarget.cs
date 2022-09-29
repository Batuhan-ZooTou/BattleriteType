using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using StarterAssets;

public class DummyTarget : MonoBehaviour 
{
    [HideInInspector]public float maxHp;
    [HideInInspector] public float currentMaxHp;
    [HideInInspector] public float currentEffectMax;

    public float currentHp;
    public float currentEffect;
    public string currentEffectName;
    public bool forced;
    public bool onAction;

    [System.NonSerialized]
    public UnityEvent<float> takeDamageEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateCurrentMaxHpEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateEffectEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateEffectMaxEvent;

    [HideInInspector]public Collider dummyCollider;
    public Rigidbody rb;

    public ThirdPersonController player;
    public DummyMovement dummyMovement;
    public DummyCombat dummyCombat;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        if (dummyCombat.playerHardCC == PlayerHardCC.Stunned)
        {
            UpdateEffectProgress();
        }
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
    public void ClampHpValues()
    {
        if (currentMaxHp - currentHp >= 40)
        {
            UpdateCurrentMaxHp(currentHp + 40);
        }
    }
   
}
