using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObject/PlayerAbility")]
public class AshkaQ : Ability
{
    public float range;
    public float radius;
    public float damage;
    public float stunTime;
    public int energyGain;
    public GameObject prefab;
    public bool animEnded;
    [SerializeField]private SphereCollider hitBox;
    [SerializeField]private Animator animator;
    private void Awake()
    {
        //hitBox = prefab.GetComponent<SphereCollider>();
        //animator = prefab.GetComponent<Animator>();
    }
    public override void Trigger(ThirdPersonController hero,AbilityHolder holder)
    {
        hero.cam.currentSkillRange = range;
        prefab.transform.localScale = Vector3.right*radius;
        prefab.transform.position = hero.cam.skillPoint;
    }
    public override void Cancel()
    {
        base.Cancel();
    }
    public override void Cast(AbilityHolder holder)
    {
        if (holder!=null && animEnded)
        {
            animEnded = false;
            Collider[] collisions = Physics.OverlapSphere(prefab.transform.position + hitBox.center, hitBox.radius);
            foreach (Collider collider in collisions)
            {
                if (collider.TryGetComponent(out IDamageable damageable))
                {
                    if (damageable != holder.hero.GetComponent<IDamageable>())
                    {
                        damageable.TakeDamage(damage, DamageTypes.HardCC, stunTime);
                        damageable.GetHardCC(PlayerHardCC.Stunned, stunTime, 0);
                    }
                    holder.hero.playerSO.UpdateCurrentEnergy(energyGain);
                }
            }
            prefab.SetActive(false);
        }
        else
        {
            Debug.Log("casted");
            prefab.SetActive(true);
            animator.enabled = true;
        }
    }
    public override void TriggerEndAnim()
    {
        animEnded = true;
    }
    

}
