using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillQ : MonoBehaviour
{
    public ThirdPersonController player;
    private List<IDamageable> detectedDamageable = new List<IDamageable>();
    public bool onAction;
    private bool switched;
    public float damage;
    public float damageOverTime;
    public float DotTickTimer;
    public float range;
    public float radius;
    public float castTime;
    private float castCounter;
    public float slowDownOncCasting; //percentage
    public float coolDown;
    public float stunTime;
    public float energyGain;
    [HideInInspector]public float coolDownCounter;
    public bool onCooldown;

    public PlayerHardCC HardCCType;
    public PlayerDebuffs deBuffType;

    private CapsuleCollider capsuleCollider;
    private Animator animator;
    private void OnEnable()
    {
        if (capsuleCollider==null)
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            animator = GetComponent<Animator>();
        }
        castCounter = castTime;
        coolDownCounter = coolDown;
        capsuleCollider.enabled = false;
        animator.enabled = false;
    }
    private void Update()
    {
        //casting
        if (onAction && castCounter!=0)
        {
            if (castCounter==castTime)
            {
                player.playerControllStates = PlayerDebuffs.Snared;
                StartCoroutine(player.ClearDamageEffects(castTime));
            }
            castCounter -= Time.deltaTime;
            castCounter = Mathf.Clamp(castCounter, 0, castTime);
            player.UpdateActionProgress(castTime);
            player.cam.currentSkillRange = range;
            transform.localScale /= transform.localScale.x * radius;
            transform.position = player.cam.skillPoint;
            player.ChangedSpeed = player.DefaultSpeed - (player.DefaultSpeed / 100 * slowDownOncCasting);
        }
        //cancel
        if (player._input.actionCancel && castCounter != castTime && onAction)
        {
            StopCoroutine(player.ClearDamageEffects(castTime));
            player.playerControllStates = PlayerDebuffs.None;
            StartCoroutine(player.ClearDamageEffects(0));
            player._input.onAction = false;
            onAction = false;
            castCounter = castTime;
            gameObject.SetActive(false);
            player.UpdateActionProgress(0);
            player._input.delayAfterCancel = true;
            player._input.actionCancel = false;
        }
        //execute
        else if (castCounter == 0)
        {
            switched = true;
            onAction = false;
            castCounter = castTime;
            player._input.onAction = false;
            player.UpdateActionProgress(0);
            animator.enabled = true;
            onCooldown = true;
            player.skillShiftQ.onCooldown = true;
            player._input.actionCancel = false;
        }
    }
    public IEnumerator TriggerEndAnimation()
    {
        if (switched)
        {
            switched = false;
            capsuleCollider.enabled = true;
            yield return new WaitForFixedUpdate();
            foreach (IDamageable item in detectedDamageable.ToList())
            {
                if (item != player.GetComponent<IDamageable>())
                {
                    item.TakeDamage(damage, DamageTypes.HardCC, stunTime);
                    item.GetHardCC(HardCCType, stunTime,0);
                    //item.GetDeBuffed(PlayerDebuffs.FadingSnared,4,0);
                    item.DamageOverTime(damageOverTime, 5, 0.5f);
                }
                detectedDamageable.Remove(item);
                player.playerSO.UpdateCurrentEnergy(energyGain);

            }
            gameObject.SetActive(false);
        }
    }
    public void AddToDetected(Collider collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            detectedDamageable.Add(damageable);
        }
    }
    public void RemoveFromDetected(Collider collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            detectedDamageable.Remove(damageable);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        AddToDetected(collision);
    }
    private void OnTriggerExit(Collider collision)
    {
        RemoveFromDetected(collision);
    }
}
