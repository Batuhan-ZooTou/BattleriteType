using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    public ThirdPersonController hero;
    [SerializeField]float _cooldownTime;
    [SerializeField] float _castTime;
    float _useTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (ability.abilityState)
        {
            case AbilityState.Ready:
                _castTime = ability.castTime;
                _cooldownTime = ability.cooldownTime;
                break;
            case AbilityState.Casting:
                ability.Trigger(hero,this);
                _castTime -= Time.deltaTime;
                hero.UpdateActionProgress(_castTime);
                if (_castTime <= 0)
                {
                    ChangeAbilityState(AbilityState.Casted);
                }
                break;
            case AbilityState.Canceled:
                ability.Cancel();
                _cooldownTime = ability.cooldownTimeAfterCancel;
                _useTime = 0;
                ChangeAbilityState(AbilityState.OnCooldown);
                break;
            case AbilityState.Casted:
                ability.Cast(null);
                ChangeAbilityState(AbilityState.OnCooldown);
                break;
            case AbilityState.OnCooldown:
                if (_useTime>0)
                {
                    _useTime -= Time.deltaTime;
                    ability.Cast(this);
                }
                if (ability.cooldownWhileActive)
                {
                    _cooldownTime -= Time.deltaTime;
                }
                else
                {
                    //cooldown when skill ended
                    if (_useTime<=0)
                        _cooldownTime -= Time.deltaTime;
                }
                if (_cooldownTime<=0)
                {
                    ChangeAbilityState(AbilityState.Ready);
                }
                break;
            default:
                break;
        }
    }
    public void ChangeAbilityState(AbilityState state)
    {
        ability.abilityState = state;
    }
}
