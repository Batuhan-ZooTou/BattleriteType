using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class SkillF : MonoBehaviour
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
    public float energyRequirment;
    public bool jumped;
    private Vector3 startPos;
    private Vector3 _dir;
    public Vector3 hitBox;
    [HideInInspector] public float coolDownCounter;
    public bool onCooldown;
    public GameObject Geometry;
    public GameObject indicator;
    public LayerMask targetLayer;
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
            player.rb.MovePosition(transform.position + (_dir  * speed*Time.deltaTime));
            if (Vector3.Distance(player.transform.position, startPos)> range)
            {
                player.cam.canRotate = true;
                Geometry.transform.GetComponent<Collider>().isTrigger = false;
                transform.GetComponent<Collider>().isTrigger = false;
                jumped = false;
                player._input.canMove = true;
                Geometry.SetActive(false);
                player.playerGeometry.SetActive(true);
            }
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, hitBox, transform.forward, transform.rotation, radius, targetLayer, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit item in hits)
            {
                if (item.transform.GetComponent<DummyTarget>() != null)
                {
                    if (!item.transform.GetComponent<DummyTarget>().forced)
                    {
                        Vector3 _dir = Vector3.up;
                        item.transform.GetComponent<IDamageable>().TakeDamage(damage, DamageTypes.Normal, 0);
                        item.transform.GetComponent<IDamageable>().Knockback(_dir.normalized, knockbackPower);
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
            indicator.SetActive(true);
            castCounter -= Time.deltaTime;
            castCounter = Mathf.Clamp(castCounter, 0, castTime);
            player.UpdateActionProgress(castTime);
            player.ChangedSpeed = player.DefaultSpeed - (player.DefaultSpeed / 100 * slowDownOncCasting);
        }
        //execute
        else if (castCounter == 0)
        {
            indicator.SetActive(false);
            player.playerGeometry.SetActive(false);
            transform.GetComponent<Collider>().isTrigger = true;
            Geometry.transform.GetComponent<Collider>().isTrigger = true;
            player.cam.canRotate = false;
            Geometry.SetActive(true);
            onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            onCooldown = true;
            _dir = player.cam.dir;
            startPos = player.transform.position;
            jumped = true;
            player._input.actionCancel = false;
            player.playerSO.UpdateCurrentEnergy(-energyRequirment);
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
