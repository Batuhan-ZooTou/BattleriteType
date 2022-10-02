using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCombat : MonoBehaviour, IDamageable
{
    public List<DeBuffSlot> deBuffSlots = new List<DeBuffSlot>();
    public List<BuffSlot> buffSlots = new List<BuffSlot>();
    public DummyTarget dummyTarget;
    // Variables for Shoting
    public float atkSpd;
    public float atkDmg;
    public float bulletSpd;
    public float bulletRange;
    public float atkActionTime;
    private float atkSpdCounter;
    ObjectPooler objectPooler;
    public bool isSilenced;
    //Enums for status
    public PlayerHardCC playerHardCC;
    public PlayerDebuffs deBuffed;
    public PlayerBuffs buffed;
    // timers
    [SerializeField] private float hardCCcounter;
    [SerializeField] private float hardCCtimer;
    //Knockback
    public bool forced;


    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHardCC==PlayerHardCC.Normal || playerHardCC == PlayerHardCC.None)
        {
            if (!isSilenced)
            {
                Shot();
            }
        }
        ClearHardCC(hardCCtimer);
        ClearDeBuff();
    }
    public void TakeDamage(float amount, DamageTypes damageTypes, float debuffTime)
    {
        if (playerHardCC==PlayerHardCC.Incapacitated)
        {
            if (damageTypes!=DamageTypes.HardCC)
            {
                playerHardCC = PlayerHardCC.Normal;
                dummyTarget.dummyMovement.moveSpeed = dummyTarget.dummyMovement.defaultSpeed;
                dummyTarget.dummyMovement.canMove = true;
            }
        }
        switch (damageTypes)
        {
            case DamageTypes.None:
                break;
            case DamageTypes.Normal:
                dummyTarget.DecreaseHp(amount);
                dummyTarget.ClampHpValues();
                break;
            case DamageTypes.HardCC:
                dummyTarget.currentEffect = debuffTime;
                hardCCtimer = debuffTime;
                dummyTarget.UpdateEffectProgressMax(debuffTime);
                dummyTarget.DecreaseHp(amount);
                dummyTarget.ClampHpValues();
                break;
            case DamageTypes.DeBuff:
                dummyTarget.DecreaseHp(amount);
                dummyTarget.ClampHpValues();
                break;
            case DamageTypes.Buff:
                dummyTarget.DecreaseHp(amount);
                dummyTarget.ClampHpValues();
                break;
            default:
                break;
        }
    }
    public void Knockback(Vector3 _dir, float strength)
    {
        dummyTarget.rb.velocity = Vector3.zero;
        dummyTarget.dummyMovement.dir = _dir;
        dummyTarget.forced = true;
    }
    public void DamageOverTime(float amount, int tick, float tickRate)
    {
        StartCoroutine(Dot(amount, tick, tickRate));
    }
    public IEnumerator Dot(float amount, int tick, float tickRate)
    {
        for (int i = 0; i < tick; i++)
        {
            yield return new WaitForSeconds(tickRate);
            dummyTarget.DecreaseHp(amount);
            dummyTarget.ClampHpValues();
        }
    }
    public void GetHardCC(PlayerHardCC type, float DebuffTime , float amount)
    {
        playerHardCC = type;
        dummyTarget.currentEffectName = type.ToString();
        dummyTarget.UpdateEffectName(type.ToString());
        switch (playerHardCC)
        {
            case PlayerHardCC.None:
                break;
            case PlayerHardCC.Normal:
                break;
            case PlayerHardCC.Stunned:
                //Stun the player
                hardCCcounter = 0;
                //Apply effect 
                dummyTarget.dummyMovement.ReduceMovementSpeedToZero(false);
                //And ClearsHardCC
                break;
            case PlayerHardCC.Incapacitated:
                dummyTarget.dummyMovement.ReduceMovementSpeedToZero(false);
                hardCCcounter = 0;
                break;
            case PlayerHardCC.Paniced:
                hardCCcounter = 0;
                atkSpdCounter = 0;
                dummyTarget.onAction = false;
                dummyTarget.dummyMovement.moveSpeedWhileOnAction = dummyTarget.dummyMovement.changedMoveSpeedWhileOnAction;
                dummyTarget.dummyMovement.moveSpeedChanged = false;
                dummyTarget.dummyMovement.dir= -(dummyTarget.player.transform.position - transform.position).normalized;
                break;
            case PlayerHardCC.Petrified:
                dummyTarget.dummyMovement.ReduceMovementSpeedToZero(false);
                dummyTarget.UpdateCurrentShield(amount);
                hardCCcounter = 0;
                break;
            default:
                break;
        }
    }
    
    public void GetDeBuffed(PlayerDebuffs type, float DebuffTime , float amount)
    {
        deBuffed = type;
        dummyTarget.currentEffectName = type.ToString();
        dummyTarget.UpdateEffectName(type.ToString());
        DeBuffSlot recentDebuff = new DeBuffSlot(type, DebuffTime, DebuffTime, amount);
        deBuffSlots.Add(recentDebuff);
        switch (deBuffed)
        {
            case PlayerDebuffs.None:
                break;
            case PlayerDebuffs.Normal:
                break;
            case PlayerDebuffs.Silenced:
                isSilenced = true;
                break;
            case PlayerDebuffs.Snared:
                for (int i = 0; i < deBuffSlots.Count; i++)
                {
                    if (deBuffSlots[i].deBuffType == PlayerDebuffs.Snared)
                    {
                        if (deBuffSlots[i] != recentDebuff)
                        {
                            if (deBuffSlots[i].amount < recentDebuff.amount)
                            {
                                if (deBuffSlots[i].counter > recentDebuff.time)
                                {
                                    recentDebuff.counter = deBuffSlots[i].counter;
                                }
                                deBuffSlots.Remove(deBuffSlots[i]);
                                dummyTarget.dummyMovement.ReduceMovementSpeedByPercentage(amount);
                            }
                            else
                            {
                                if (deBuffSlots[i].counter < recentDebuff.time)
                                {
                                    deBuffSlots[i].counter = recentDebuff.time;
                                }
                                deBuffSlots.Remove(recentDebuff);
                            }
                        }
                        else
                        {
                            dummyTarget.dummyMovement.ReduceMovementSpeedByPercentage(amount);
                        }
                    }

                }
                break;
            case PlayerDebuffs.Weakened:
                break;
            case PlayerDebuffs.Blinded:
                break;
            case PlayerDebuffs.Rooted:
                dummyTarget.dummyMovement.canMove = true;
                break;
            case PlayerDebuffs.FadingSnared:
                dummyTarget.dummyMovement.ReduceMovementSpeedToZero(true);
                break;
            case PlayerDebuffs.Disarm:
                break;
            case PlayerDebuffs.UsingSkill:
                break;
            default:
                break;
        }
    }
    public void GetBuffed(PlayerBuffs type, float DebuffTime , float amount)
    {
        buffed = type;
        dummyTarget.currentEffectName = type.ToString();
        BuffSlot recentbuff = new BuffSlot(type, DebuffTime, DebuffTime, amount);
        buffSlots.Add(recentbuff);

        switch (buffed)
        {
            case PlayerBuffs.None:
                break;
            case PlayerBuffs.Normal:
                break;
            case PlayerBuffs.SpeedUp:
                break;
            case PlayerBuffs.LifeSteal:
                break;
            case PlayerBuffs.AtkBoost:
                break;
            case PlayerBuffs.DamageReduction:
                break;
            case PlayerBuffs.Healing:
                break;
            case PlayerBuffs.Shield:
                dummyTarget.UpdateCurrentShield(amount);
                for (int i = 0; i < deBuffSlots.Count; i++)
                {
                    if (buffSlots[i].buffType == PlayerBuffs.Shield)
                    {
                        if (buffSlots[i] != recentbuff)
                        {
                            if (buffSlots[i].counter <= recentbuff.time)
                            {
                                buffSlots.Remove(buffSlots[i]);
                            }
                            else
                            {
                                recentbuff.counter = buffSlots[i].counter;
                                buffSlots.Remove(buffSlots[i]);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
    public void ClearDeBuff()
    {
        if (deBuffSlots.Count > 0)
        {
            for (int i = 0; i < deBuffSlots.Count; i++)
            {
                deBuffSlots[i].counter -= Time.deltaTime;
                if (deBuffSlots[i].deBuffType == PlayerDebuffs.FadingSnared )
                {
                    dummyTarget.dummyMovement.IncreaseMovementSpeedPerFrame(deBuffSlots[i].time);
                }
                if (deBuffSlots[i].deBuffType == PlayerDebuffs.Snared)
                {
                    dummyTarget.dummyMovement.moveSpeedChanged = true;

                }
                if (deBuffSlots[i].deBuffType == PlayerDebuffs.UsingSkill)
                {
                    if (dummyTarget.onAction)
                    {
                        TakeDamage(0, DamageTypes.HardCC, deBuffSlots[i].time);
                        GetHardCC(PlayerHardCC.Paniced, deBuffSlots[i].time, 0);
                        deBuffSlots[i].counter = 0;
                    }
                }
                if (deBuffSlots[i].counter <= 0)
                {
                    Debug.Log("Cleared debuff");
                    if (deBuffSlots[i].deBuffType==PlayerDebuffs.Snared)
                    {
                        dummyTarget.dummyMovement.ClearMovementSlow();
                    }
                    else if (deBuffSlots[i].deBuffType == PlayerDebuffs.Silenced)
                    {
                        isSilenced = false;
                    }
                    else if (deBuffSlots[i].deBuffType == PlayerDebuffs.Rooted)
                    {
                        dummyTarget.dummyMovement.canMove = true;
                    }
                    if (deBuffSlots[i].deBuffType.ToString()== dummyTarget.currentEffectName)
                    {
                        dummyTarget.UpdateEffectName("");
                    }
                    deBuffSlots.Remove(deBuffSlots[i]);
                }
            }
        }
        else
        {
            deBuffed = PlayerDebuffs.Normal;
        }
    }
    public void ClearBuff()
    {
        if (buffSlots.Count > 0)
        {
            for (int i = 0; i < buffSlots.Count; i++)
            {
                buffSlots[i].counter -= Time.deltaTime;
                if (buffSlots[i].counter <= 0)
                {
                    Debug.Log("Cleared buff");
                    if (buffSlots[i].buffType == PlayerBuffs.Shield)
                    {
                        dummyTarget.UpdateCurrentShield(0);
                    }
                    buffSlots.Remove(buffSlots[i]);
                }
            }
        }
        else
        {
            deBuffed = PlayerDebuffs.Normal;
        }
    }
    public void ClearHardCC(float time)
    {
        switch (playerHardCC)
        {
            case PlayerHardCC.None:
                break;
            case PlayerHardCC.Normal:
                hardCCcounter = 0; 
                dummyTarget.UpdateEffectName("");
                break;
            case PlayerHardCC.Stunned:
                hardCCcounter += Time.deltaTime;
                if (hardCCcounter >= time)
                {
                    hardCCcounter = 0;
                    playerHardCC = PlayerHardCC.Normal; 
                    dummyTarget.dummyMovement.moveSpeed = dummyTarget.dummyMovement.defaultSpeed;
                    dummyTarget.dummyMovement.canMove = true;
                }
                break;
            case PlayerHardCC.Incapacitated:
                hardCCcounter += Time.deltaTime;
                if (hardCCcounter >= time)
                {
                    hardCCcounter = 0;
                    playerHardCC = PlayerHardCC.Normal;
                    dummyTarget.dummyMovement.moveSpeed = dummyTarget.dummyMovement.defaultSpeed;
                    dummyTarget.dummyMovement.canMove = true;
                }
                break;
            case PlayerHardCC.Paniced:
                hardCCcounter += Time.deltaTime;
                if (hardCCcounter >= time)
                {
                    hardCCcounter = 0;
                    playerHardCC = PlayerHardCC.Normal;
                    dummyTarget.dummyMovement.moveSpeed = dummyTarget.dummyMovement.defaultSpeed;
                    dummyTarget.dummyMovement.canMove = true;
                }
                break;
            case PlayerHardCC.Petrified:
                hardCCcounter += Time.deltaTime;
                if (hardCCcounter >= time)
                {
                    hardCCcounter = 0;
                    playerHardCC = PlayerHardCC.Normal;
                    dummyTarget.dummyMovement.moveSpeed = dummyTarget.dummyMovement.defaultSpeed;
                    dummyTarget.dummyMovement.canMove = true;
                }
                break;
            default:
                break;
        }
    }
    
    public void Shot()
    {
        if (atkSpdCounter>=atkSpd)
        {
            atkSpdCounter = 0;
            dummyTarget.onAction = false;
            dummyTarget.dummyMovement.moveSpeedWhileOnAction = dummyTarget.dummyMovement.changedMoveSpeedWhileOnAction;
            dummyTarget.dummyMovement.moveSpeedChanged = false;
            GameObject projectile = objectPooler.SpawnFromPool("DummyProjectile", transform.position, transform.rotation);
            ProjectileBehavior projectileBehavior = projectile.GetComponent<ProjectileBehavior>();
            projectileBehavior._damage = atkDmg;
            projectileBehavior.Type = DamageTypes.Normal;
            projectileBehavior.dir = transform.forward;
            projectileBehavior._speed = bulletSpd;
            projectileBehavior._maxRange = bulletRange;
            projectileBehavior.projectileOwner = transform.gameObject;
            Physics.IgnoreCollision(dummyTarget.dummyCollider, projectile.GetComponent<Collider>(), true);

        }
        else
        {
            atkSpdCounter += Time.deltaTime;
            if (atkSpdCounter >= atkSpd-atkActionTime)
            {
                dummyTarget.onAction = true;
            }
        }
    }
}
