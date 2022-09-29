using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class SkillR : MonoBehaviour
{
    public ThirdPersonController player;
    public bool onAction;
    public float damage;
    public float range;
    public float radius;
    public float castTime;
    private float castCounter;
    public float energyRequirment;
    public float liveTime;
    public float slowDownOncCasting; //percentage
    public float coolDown;
    [HideInInspector] public float coolDownCounter;
    public bool onCooldown;
    public GameObject Geometry;
    private void Start()
    {
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
            player.cam.currentSkillRange = range;
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
            Geometry.SetActive(true);
            Geometry.transform.position = player.cam.skillPoint;
            Geometry.transform.rotation = player.transform.rotation;
            onAction = false;
            castCounter = castTime;
            player.UpdateActionProgress(0);
            onCooldown = true;
            player._input.actionCancel = false;
            player._input.onAction = false;
            StartCoroutine(LiveTime());
            player.playerSO.UpdateCurrentEnergy(-energyRequirment);
        }
    }
    public IEnumerator LiveTime()
    {
        yield return new WaitForSeconds(liveTime);
        Geometry.SetActive(false);
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
