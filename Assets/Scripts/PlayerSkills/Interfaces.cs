using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amaount,DamageTypes type,float DebuffTime);
}
public interface IKnockable
{
    void Knockback(Vector2 angle, float strength, int direction);
}
public enum DamageTypes
{
    None,
    Normal,
    Stun,
    Silence,
    Slow,
    Overtime,
    LifeSteal
}
public enum PlayerControllStates
{
    None,
    Normal,
    Stunned,
    Silenced,
    Slowed,
    DamagedOvertime
}
public enum PlayerBuffs
{
    None,
    Normal,
    SpeedUp,
    LifeSteal,
    AtkBoost,
    DamageReduction
}
