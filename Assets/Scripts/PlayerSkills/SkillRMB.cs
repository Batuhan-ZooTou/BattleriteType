using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SkillRMB : MonoBehaviour
{
    public ThirdPersonController player;
    public Transform bulletSp;
    public DamageTypes bulletType;
    public bool onAction = false;   //still on animation or not
    public float speed; //speed of projectile
    public float damage; //damage of projectile
    public float range; //range of projectile
    public float radius; //radius of projectile
    public float castTime; //time to projectile spawn after input
    public float slowDownOncCasting; //slow down while casting
    public int extraShots; //extra projectile after first one
    private float loopCount;
    public float delayAfterShots; //time between shots
    private float delayAfterShotsCounter;
    public float energyGain;
    private float castCounter; //timer
    [HideInInspector] public float coolDownCounter; //timer
    public GameObject skillIndicator;
    public float coolDown;
    public bool onCooldown;
    ObjectPooler objectPooler;
    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
        castCounter = castTime;
        coolDownCounter = coolDown;
        delayAfterShotsCounter = delayAfterShots;
        loopCount = extraShots;
    }
    private void Update()
    {
        Cooldown();
        //casting
        if (onAction && castCounter != 0 && !onCooldown)
        {
            skillIndicator.SetActive(true);
            if (castCounter == castTime)
            {
                player.playerControllStates = PlayerControllStates.Slowed;
                StartCoroutine(player.ClearDamageEffects(castTime+(extraShots*delayAfterShots)));
            }
            castCounter -= Time.deltaTime;
            castCounter = Mathf.Clamp(castCounter, 0, castTime);
            player.UpdateActionProgress(castTime);
            player.ChangedSpeed = player.DefaultSpeed - (player.DefaultSpeed / 100 * slowDownOncCasting);
        }
        //cancel
        if (player._input.actionCancel && onAction && castCounter != castTime)
        {
            skillIndicator.SetActive(false);
            StopCoroutine(player.ClearDamageEffects(castTime));
            player.playerControllStates = PlayerControllStates.None;
            StartCoroutine(player.ClearDamageEffects(0));

            player._input.onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            onAction = false;
            player._input.delayAfterCancel = true;
            player._input.actionCancel = false;
            if (delayAfterShotsCounter != delayAfterShots)
            {
                delayAfterShotsCounter = delayAfterShots;
                loopCount = extraShots;
            }
        }
        //execute
        else if (castCounter == 0)
        {
            onCooldown = true;
            player.UpdateActionProgress(delayAfterShots * extraShots);
            if (delayAfterShotsCounter == delayAfterShots && loopCount>=0)
            {
                SpawnProjectile();
            }
            delayAfterShotsCounter -= Time.deltaTime;
            delayAfterShotsCounter = Mathf.Clamp(delayAfterShotsCounter, 0, delayAfterShots);
            if (delayAfterShotsCounter==0 &&loopCount!=0)
            {
                delayAfterShotsCounter = delayAfterShots;
                loopCount--;
            }
            else if(loopCount==0)
            {
                delayAfterShotsCounter = delayAfterShots;
                loopCount = extraShots;
                castCounter = castTime;
                skillIndicator.SetActive(false);
                StopCoroutine(player.ClearDamageEffects(delayAfterShots * loopCount));
                player.playerControllStates = PlayerControllStates.None;
                StartCoroutine(player.ClearDamageEffects(0));
                onAction = false;
                player._input.onAction = false;
                player._input.actionCancel = false;
                player.UpdateActionProgress(0);
            }
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
    private void SpawnProjectile()
    {
        GameObject projectile = objectPooler.SpawnFromPool("AshkaRMB", bulletSp.position, player.transform.rotation);
        projectile.GetComponent<ProjectileBehavior>()._damage = damage;
        projectile.GetComponent<ProjectileBehavior>().Type = bulletType;
        projectile.GetComponent<ProjectileBehavior>().dir = player.transform.forward;
        projectile.GetComponent<ProjectileBehavior>()._speed = speed;
        projectile.GetComponent<ProjectileBehavior>()._maxRange = range;
        projectile.GetComponent<ProjectileBehavior>().playerCollider = player.playerCollider;
        projectile.GetComponent<ProjectileBehavior>().projectileOwner = transform.gameObject;
        projectile.GetComponent<ProjectileBehavior>()._energyGain = energyGain;

        Physics.IgnoreCollision(player.playerCollider, projectile.GetComponent<Collider>(), true);
    }
}
