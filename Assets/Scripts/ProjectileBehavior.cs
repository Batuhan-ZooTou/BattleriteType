using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class ProjectileBehavior : MonoBehaviour
{
    public  float _damage {  private get;  set; }
    public float _speed { private get; set; }
    private float defaultSpeed;
    public float _maxRange { private get; set; }
    public DamageTypes Type { private get; set; }

    public GameObject projectileOwner;
    private Rigidbody rb;
    private Vector3 sp;
    ObjectPooler objectPooler;
    public Vector3 dir { private get; set; }
    public Collider playerCollider { private get; set; }
    private bool turned=false;

    void Awake()
    {
        Type = DamageTypes.None;
        rb = GetComponent<Rigidbody>();
        objectPooler = ObjectPooler.Instance;
    }
    private void OnEnable()
    {
        turned = false;
        sp = transform.position;
        transform.Rotate(transform.rotation.x + 90, transform.rotation.y, transform.rotation.z);
    }
    void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Vector3.Distance(sp, transform.position) < _maxRange)
            {
                //Vector3 movement = transform.up * _speed * Time.deltaTime;
                //rb.MovePosition(transform.position + movement);
                rb.velocity = new Vector3(dir.x*_speed,0,dir.z*_speed);
            }
            else
            {
                objectPooler.AddToPool(this.gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (Type==DamageTypes.LifeSteal)
            {
                projectileOwner.GetComponent<ThirdPersonController>().playerSO.UpdateCurrentHp(_damage);
            }
            damageable.TakeDamage(_damage, Type, 0);
            objectPooler.AddToPool(this.gameObject);
        }
        if (other.TryGetComponent<Bouble>(out var areaField))
        {
            if (areaField.boubleType==Bouble.BoubleType.Pearl)
            {
                defaultSpeed = _speed;
                _speed *= areaField.speed / 100;
            }
           else if (areaField.boubleType == Bouble.BoubleType.Oldur && turned==false)
           {
                turned = true;
                transform.Rotate(transform.rotation.x + 180, transform.rotation.y, transform.rotation.z);
           }
            else if (areaField.boubleType == Bouble.BoubleType.Ashka )
            {
                objectPooler.AddToPool(this.gameObject);
            }
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            objectPooler.AddToPool(this.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Bouble>(out var areaField))
        {
            if (areaField.boubleType == Bouble.BoubleType.Oldur)
            {
                _speed -= Time.deltaTime*areaField.speed;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Bouble>(out var areaField))
        {
            if (areaField.boubleType == Bouble.BoubleType.Pearl)
            {
                _speed =defaultSpeed;
            }
        }
    }
}
