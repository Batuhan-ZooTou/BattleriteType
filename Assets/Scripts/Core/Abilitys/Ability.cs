using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public enum AbilityState
{
    Ready,
    Casting,
    Canceled,
    Casted,
    OnCooldown,
}
public enum AbilityKey
{
    RMB,
    LMB,
    Q,
    Space,
    E,
    R,
    F,
}
public class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public string description;
    public float cooldownTime;
    public float cooldownTimeAfterCancel;
    public float castTime;
    public float useTime; 
    public float energyCost;
    public bool cooldownWhileActive;
    public AbilityKey abilityKey;
    public AbilityState abilityState;


    public virtual void Trigger(ThirdPersonController hero ,AbilityHolder holder) { }
    public virtual void Cast(AbilityHolder holder) { }
    public virtual void Cancel() { }
    public virtual void TriggerEndAnim() { }



}
