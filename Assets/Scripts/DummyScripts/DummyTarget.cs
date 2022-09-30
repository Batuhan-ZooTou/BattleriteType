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

    public float currentShield;
    public float currentHp;
    public float currentEffect;
    public string currentEffectName;
    public bool forced;
    private bool _onAction;
    public bool onAction
    {
        get { return _onAction; }
        set
        {
            //Check if the bloolen variable changes from false to true
            if (_onAction == false && value == true)
            {
                // Do something
                dummyMovement.ReduceMovementSpeedWhileOnAction();
            }
            //Update the boolean variable
            _onAction = value;
        }
    }
    [System.NonSerialized]
    public UnityEvent<float> takeDamageEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateCurrentMaxHpEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateEffectEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateEffectMaxEvent;
    [System.NonSerialized]
    public UnityEvent<string> updateEffectNameEvent;
    [System.NonSerialized]
    public UnityEvent<float> updateCurrentShieldEvent;
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
        if (updateEffectNameEvent == null)
        {
            updateEffectNameEvent = new UnityEvent<string>();
        }
        if (updateCurrentShieldEvent == null)
        {
            updateCurrentShieldEvent = new UnityEvent<float>();
        }
    }
    private void Update()
    {
        if (dummyCombat.playerHardCC == PlayerHardCC.Stunned)
        {
            UpdateEffectProgress();
        }
    }
    public void UpdateCurrentShield(float value)
    {
        if (currentShield<=0)
        {
            currentShield = value;
        }
        else
        {
            currentShield += value;
        }
        updateCurrentMaxHpEvent.Invoke(currentShield + currentHp);
        updateCurrentShieldEvent.Invoke(currentShield + currentHp);
    }
    public void DecreaseHp(float value)
    {
        //if has shield
        if (currentShield>0)
        {
            //if shield hp less than dmg
            if (currentShield<value)
            {
                currentShield = 0;
                currentHp -= Mathf.Abs(currentShield - value);
                currentHp = Mathf.Clamp(currentHp, 0, currentMaxHp);
                takeDamageEvent.Invoke(currentHp);
                updateCurrentShieldEvent.Invoke(0);
            }
            //if shield hp more than dmg
            else
            {
                currentShield -= value;
                updateCurrentMaxHpEvent.Invoke(currentMaxHp + currentShield);
                updateCurrentShieldEvent.Invoke(currentShield + currentHp);
            }
        }
        else
        {
            currentHp -= value;
            currentHp = Mathf.Clamp(currentHp, 0, currentMaxHp);
            takeDamageEvent.Invoke(currentHp);
        }
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
    public void UpdateEffectName(string value)
    {
        updateEffectNameEvent.Invoke(value);
    }
    public void ClampHpValues()
    {
        if (currentMaxHp - currentHp >= 40)
        {
            UpdateCurrentMaxHp(currentHp + 40);
        }
    }
   
}
