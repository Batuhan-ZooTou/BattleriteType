using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillLMB : MonoBehaviour
{
    public ThirdPersonController player;
    public DamageTypes bulletType;
    public Transform bulletSp;
    public bool onAction=false;   //still on animation or not
    public float speed; //speed of projectile
    public float damage; //damage of projectile
    public float range; //range of projectile
    public float radius; //radius of projectile
    public float castTime; //time to projectile spawn after input
    public float slowDownOncCasting; //slow down while casting
    public float energyGain;
    private float castCounter; //timer
    [HideInInspector] public float coolDownCounter; //timer
    public float coolDown;
    public bool onCooldown;
    ObjectPooler objectPooler;
    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
        castCounter = castTime;
        coolDownCounter = coolDown;
    }
    private void Update()
    {
        Cooldown();
        //casting
        if (onAction && castCounter != 0)
        {
            if (castCounter == castTime)
            {
                player.playerControllStates = PlayerDebuffs.Snared;
                StartCoroutine(player.ClearDamageEffects(castTime));
            }
            castCounter -= Time.deltaTime;
            castCounter = Mathf.Clamp(castCounter, 0, castTime);
            player.UpdateActionProgress(castTime);
            player.ChangedSpeed = player.DefaultSpeed-(player.DefaultSpeed / 100 * slowDownOncCasting);
        }
        //cancel
        if (player._input.actionCancel && onAction && castCounter != castTime)
        {
            StopCoroutine(player.ClearDamageEffects(castTime));
            player.playerControllStates = PlayerDebuffs.None;
            StartCoroutine(player.ClearDamageEffects(0));
            player._input.onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            onAction = false;
            player._input.delayAfterCancel = true;
            player._input.actionCancel = false;
        }
        //execute
        else if (castCounter == 0)
        {
            player._input.onAction = false;
            onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            onCooldown = true;
            player._input.actionCancel = false;
            GameObject projectile = objectPooler.SpawnFromPool("AshkaLMB", bulletSp.position, player.transform.rotation);
            projectile.GetComponent<ProjectileBehavior>()._damage = damage;
            projectile.GetComponent<ProjectileBehavior>().Type = bulletType;
            projectile.GetComponent<ProjectileBehavior>().dir = player.transform.forward;
            projectile.GetComponent<ProjectileBehavior>()._speed = speed;
            projectile.GetComponent<ProjectileBehavior>()._maxRange = range;
            projectile.GetComponent<ProjectileBehavior>().playerCollider = player.playerCollider;
        projectile.GetComponent<ProjectileBehavior>()._energyGain = energyGain;
        projectile.GetComponent<ProjectileBehavior>().projectileOwner = transform.gameObject;
            Physics.IgnoreCollision(player.playerCollider, projectile.GetComponent<Collider>(), true);
        }
    }
    public void Cooldown()
    {
        if (onCooldown && coolDownCounter != 0)
        {
            coolDownCounter -= Time.deltaTime;
            coolDownCounter = Mathf.Clamp(coolDownCounter, 0, coolDown);
        }
        else
        {
            coolDownCounter = coolDown;
            onCooldown = false;
        }
    }
}
