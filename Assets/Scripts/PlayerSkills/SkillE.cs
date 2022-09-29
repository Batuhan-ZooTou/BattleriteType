using System.Collections;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class SkillE : MonoBehaviour
{
    public ThirdPersonController player;
    public bool onAction;
    public float damage;
    public float range;
    private float changedRange;
    public float rangeAfterHit;
    public float castTime;
    private float castCounter;
    public float slowDownOncCasting; //percentage
    public float coolDown;
    public float speed;
    public bool forced;
    public bool targer;
    private Vector3 _dir;
    private Vector3 startPos;
    public LayerMask targetLayer;
    public float colliderAreaHalf;
    [HideInInspector] public float coolDownCounter;
    public bool onCooldown;
    public GameObject Geometry;
    private void Start()
    {
        castCounter = castTime;
        coolDownCounter = coolDown;
        changedRange = range;
    }
    private void FixedUpdate()
    {
        CheckForTargetFront();
        ApplyingForce();
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
            player.ChangedSpeed = player.DefaultSpeed - (player.DefaultSpeed / 100 * slowDownOncCasting);
        }
        //cancel
        if (player._input.actionCancel && castCounter != castTime && onAction)
        {
            StopCoroutine(player.ClearDamageEffects(castTime));
            player.playerControllStates = PlayerDebuffs.None;
            player._input.onAction = false;
            onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            player._input.delayAfterCancel = true;
            player._input.actionCancel = false;
        }
        //execute
        else if (castCounter == 0)
        {
            player.playerGeometry.SetActive(false);
            Geometry.SetActive(true);
            onAction = false;
            player.cam.canRotate = false;
            castCounter = castTime;
            //player._input.onAction = false;
            //player.rb.AddForce( player.cam.dir* speed, ForceMode.Impulse);
            _dir = player.cam.dir;
            startPos = player.transform.position;
            forced = true;
            player.UpdateActionProgress(0);
            onCooldown = true;
        }
    }
    public void  ApplyingForce()
    {
        if (forced)
        {
            player._input.canMove = false;
            player.rb.AddForce(_dir * speed, ForceMode.Force);
            if (Vector3.Distance(startPos, player.transform.position) > changedRange)
            {
                player.cam.canRotate = true;
                Geometry.SetActive(false);
                player.playerGeometry.SetActive(true);
                forced = false;
                player.rb.velocity = Vector3.zero;
                player._input.canMove = true;
                player._input.onAction = false;
                player._input.actionCancel = false;
                changedRange = range;
            }
        }
    }
    public void CheckForTargetFront()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(colliderAreaHalf, colliderAreaHalf, 0.25f), transform.forward, transform.rotation, colliderAreaHalf, targetLayer, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit item in hits)
        {
            if (item.transform.GetComponent<IDamageable>()!=null && forced)
            {
                item.transform.GetComponent<IDamageable>().Knockback(_dir,damage);
                targer = true;
                if (targer && forced)
                {
                    startPos = transform.position;
                    _dir *= -1;
                    changedRange = rangeAfterHit;
                }
            }
        }
        if (hits.Length == 0)
        {
            targer = false;
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
    private void OnCollisionEnter(Collision collision)
    {
        if (forced && collision.gameObject.CompareTag("Wall"))
        {
            Geometry.SetActive(false);
            player.playerGeometry.SetActive(true);
            forced = false;
                player.rb.velocity = Vector3.zero;
                player._input.canMove = true;
                player._input.onAction = false;
                player._input.actionCancel = false;
                changedRange = range;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (targer) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawCube(transform.position, new Vector3(colliderAreaHalf*2, colliderAreaHalf*2, 0.5f));
    }
}
