using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amaount,DamageTypes type,float DebuffTime);
    void GetHardCC(PlayerHardCC type, float DebuffTime, float amount);
    void GetDeBuffed(PlayerDebuffs type, float DebuffTime,float amount);
    void GetBuffed(PlayerBuffs type, float DebuffTime,float amount);
    void DamageOverTime(float amount, int tick, float tickRate);
    void Knockback(Vector3 dir, float strength);
}

public class DeBuffSlot
{

    public PlayerDebuffs deBuffType;
    public float time;
    public float counter;
    public float amount;

    public DeBuffSlot(PlayerDebuffs _deBuffType, float _time,float _counter,float _amount)
    {
        deBuffType = _deBuffType;
        time = _time;
        counter = _counter;
        amount = _amount;
    }
}
public class BuffSlot
{

    public PlayerBuffs buffType;
    public float time;
    public float counter;
    public float amount;
    public BuffSlot(PlayerBuffs _buffType, float _time, float _counter, float _amount)
    {
        buffType = _buffType;
        time = _time;
        counter = _counter;
        amount = _amount;
    }
}
public enum DamageTypes
{
    None,
    Normal,
    Stun,
    Silence,
    Slow,
    Overtime,
    LifeSteal,
    HardCC,
    DeBuff,
    Buff,
}
public enum PlayerDebuffs
{
    None,
    Normal,
    Stunned,
    Silenced,
    Snared,
    DamagedOvertime,
    Weakened,
    Blinded,
    Rooted,
    FadingSnared,
    Disarm,
    UsingSkill
}
public enum PlayerHardCC
{
    None,
    Normal,
    Stunned,
    Incapacitated,
    Paniced,
    Petrified
}
public enum PlayerBuffs
{
    None,
    Normal,
    SpeedUp,
    LifeSteal,
    AtkBoost,
    DamageReduction,
    Healing,
    Shield
}
