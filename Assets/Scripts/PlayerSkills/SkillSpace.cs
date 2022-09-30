using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class SkillSpace : MonoBehaviour
{
    public ThirdPersonController player;
    public bool onAction;
    public float damage;
    public float range;
    public float radius;
    public float castTime;
    private float castCounter;
    public float slowDownOncCasting; //percentage
    public float coolDown;
    public float speed;
    public float knockbackPower;
    public bool jumped;
    public float energyGain;
    private Vector3 jumpedPos;
    [HideInInspector] public float coolDownCounter;
    public bool onCooldown;
    public GameObject Geometry;
    public GameObject indicator;
    public LayerMask targetLayer;
    public Animator animator;
    private void Start()
    {
        castCounter = castTime;
        coolDownCounter = coolDown;
    }
    private void Update()
    {
        if (jumped)
        {
            player._input.canMove = false;
            player.rb.MovePosition(transform.position + (jumpedPos - transform.position) * Time.deltaTime * speed);
            if (Vector3.Distance(player.transform.position, jumpedPos) < 0.1f)
            {
                jumped = false;
                player._input.canMove = true;
                Geometry.SetActive(false);
                player.playerGeometry.SetActive(true);
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, radius, targetLayer, QueryTriggerInteraction.Ignore);
                foreach (RaycastHit item in hits)
                {
                    if (item.transform.GetComponent<IDamageable>() != null)
                    {
                        Vector3 _dir = item.transform.position - transform.position;
                        _dir.y = 0.01f;
                        item.transform.GetComponent<IDamageable>().TakeDamage(damage,DamageTypes.Normal,0);
                        //item.transform.GetComponent<IDamageable>().GetDeBuffed(PlayerDebuffs.Snared,6,damage);
                        item.transform.GetComponent<IDamageable>().Knockback(_dir.normalized, knockbackPower);
                        player.playerSO.UpdateCurrentEnergy(energyGain);
                    }
                }
            }
        }
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
            player.cam.currentSkillRange = range;
            jumpedPos = player.cam.skillPoint;
            indicator.transform.position = player.cam.skillPoint;
            player.UpdateActionProgress(castTime);
            indicator.SetActive(true);
            player.ChangedSpeed = player.DefaultSpeed - (player.DefaultSpeed / 100 * slowDownOncCasting);
        }
        //cancel
        if (player._input.actionCancel && castCounter != castTime && onAction)
        {
            StopCoroutine(player.ClearDamageEffects(castTime));
            player.playerControllStates = PlayerDebuffs.None;
            player._input.onAction = false;
            onAction = false;
            indicator.SetActive(false);
            castCounter = castTime;
            player.UpdateActionProgress(0);
            player._input.delayAfterCancel = true;
            player._input.actionCancel = false;
        }
        //execute
        else if (castCounter == 0)
        {
            indicator.SetActive(false);
            player.playerGeometry.SetActive(false);
            Geometry.SetActive(true);
            onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            onCooldown = true;
            player.skillShiftSpace.onCooldown = true;
            jumped = true;
            player._input.actionCancel = false;
            player._input.onAction = false;

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
