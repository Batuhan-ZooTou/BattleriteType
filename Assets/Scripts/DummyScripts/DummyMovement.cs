using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DummyMovement : MonoBehaviour
{
    public DummyTarget dummyTarget;
    public bool canMove=true;
    public float defaultSpeed;
    public float moveSpeed;
    public float moveSpeedWhileOnAction;
    public float changedMoveSpeedWhileOnAction {  get; private set; }
    public float _changedMoveSpeed;
    public float speedChangeRate;
    public float deAcceleration;
    public bool moveSpeedChanged;
    public float lookRange;
    public float turnRate;
    public bool gettingForced;
    private Vector3 spawnPoint;
    private Vector3 nextPos;
    [HideInInspector]public Vector3 dir;
    public float moveRange;
    public float knockbackRes;

    public float maxMagnitude;

    public bool reachedMaxMagnitude;
    public LayerMask WhatIsGround;
    private bool _grounded;
    public bool grounded
    {
        get { return _grounded; }
        set
        {
            //Check if the bloolen variable changes from false to true
            if (_grounded == false && value == true)
            {
                // Do something
                canMove = true;
            }
            //Update the boolean variable
            _grounded = value;
        }
    }

    private void Awake()
    {
        NextPosition();
        changedMoveSpeedWhileOnAction = moveSpeedWhileOnAction;
    }

    void Update()
    {
        CheckSurroundings();
    }
    private void FixedUpdate()
    {
        if (!dummyTarget.forced)
        {
            if (canMove)
            {
                Move();
                TurnToPlayer();
            }
            reachedMaxMagnitude = false;
        }
        else
        {
            ManageForce();
        }
    }
    public void ManageForce()
    {
        if (dummyTarget.rb.velocity.magnitude> maxMagnitude)
        {
            moveSpeed = 0;
            reachedMaxMagnitude = true;
        }
        else if (dummyTarget.rb.velocity.magnitude < 1 && reachedMaxMagnitude)
        {
            dummyTarget.forced = false;
        }
        if (!reachedMaxMagnitude)
        {
            dummyTarget.rb.AddForce(dir * knockbackRes, ForceMode.Impulse);
        }
        else
        {
            dummyTarget.rb.AddForce(-dir * deAcceleration, ForceMode.VelocityChange);
        }
    }
   
    public void Move()
    {
        dir = (nextPos - transform.position).normalized;
        if (Vector3.Distance(transform.position, nextPos) > 1)
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = moveSpeedChanged ? _changedMoveSpeed : defaultSpeed;
            if (moveSpeedChanged)
            {
                targetSpeed = dummyTarget.onAction ? moveSpeedWhileOnAction : _changedMoveSpeed;
            }

            // if there is no input, set the target speed to 0
            if (!canMove) targetSpeed = 0.0f;

            float speedOffset = 0.1f;

            // accelerate or decelerate to target speed
            if (moveSpeed < targetSpeed - speedOffset)
            {
                if (moveSpeed < 0.2f)
                {
                    moveSpeed = 0.2f;
                }
                moveSpeed += speedChangeRate * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, 0, targetSpeed);
            }
            else if (moveSpeed > targetSpeed + speedOffset)
            {
                moveSpeed -= speedChangeRate * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, targetSpeed, moveSpeed);
            }
            else
            {
                moveSpeed = targetSpeed;
            }
            dummyTarget.rb.velocity = new Vector3(dir.x * moveSpeed, 0, dir.z * moveSpeed);
        }
        else
        {
            NextPosition();
        }
        
    }
    public void NextPosition()
    {
        float xRange = Random.Range(-moveRange, moveRange);
        float zRange = Random.Range(-moveRange, moveRange);
        nextPos = new Vector3(spawnPoint.x + xRange, spawnPoint.y, spawnPoint.z + zRange);
    }
    
    private void CheckSurroundings()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, 0.1f, WhatIsGround, QueryTriggerInteraction.Ignore);
        if (!grounded)
        {
            canMove = false;
        }
    }
    public void TurnToPlayer()
    {
        if (Vector3.Distance(transform.position,dummyTarget.player.transform.position)<lookRange)
        {
            Vector3 target = dummyTarget.player.transform.position - transform.position;
            target.y = 0;
            if (dummyTarget.onAction)
            {
                Vector3 targetDir = Vector3.Lerp(Vector3.zero,dummyTarget.player.dir * dummyTarget.player.DefaultSpeed, turnRate) ;

                float reachTime = Vector3.Distance(transform.position, dummyTarget.player.transform.position) / (dir * dummyTarget.dummyCombat.bulletSpd).magnitude;
                Quaternion newRotation = Quaternion.LookRotation(target + (targetDir / reachTime) , Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, 0.1f);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target, Vector3.up), turnRate);
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z), Vector3.up), turnRate);
        }
    }
    //Change Movement values
    #region
    public void ReduceMovementSpeedByPercentage(float percentage)
    {
         moveSpeedChanged = true;
        _changedMoveSpeed = defaultSpeed-(defaultSpeed * (percentage / 100));
    }
    public void ReduceMovementSpeedWhileOnAction()
    {
        moveSpeedChanged = true;
        float reducedSpeed = _changedMoveSpeed - moveSpeedWhileOnAction;
        if (reducedSpeed < 0)
        {
            moveSpeedWhileOnAction = _changedMoveSpeed;
        }
    }
    public void ReduceMovementSpeedToZero(bool _canMove)
    {
        dummyTarget.rb.velocity = Vector3.zero;
        if (!_canMove)
        {
            //stunned
            canMove = false;
        }
        else
        {
            //fading snare
            moveSpeedChanged = true;
            _changedMoveSpeed = 0;
        }

    }
    public void ClearMovementSlow()
    {
        if (!dummyTarget.onAction)
        {
            moveSpeedChanged = false;
            _changedMoveSpeed = defaultSpeed;
        }
    }
    public void IncreaseMovementSpeedByPercentage(float percentage)
    {
        moveSpeedChanged = true;
        _changedMoveSpeed +=(defaultSpeed * (percentage / 100));
    }
    public void IncreaseMovementSpeedPerFrame(float time)
    {
        //fading snare
        _changedMoveSpeed += Time.deltaTime * (defaultSpeed / time);
        _changedMoveSpeed = Mathf.Clamp(_changedMoveSpeed, 0, defaultSpeed);
    }
    #endregion
    private void OnCollisionEnter(Collision collision)
    {
        if (dummyTarget.forced && collision.gameObject.CompareTag("Wall"))
        {
            dummyTarget.dummyCombat.TakeDamage(0, DamageTypes.Stun, 2);
            dummyTarget.forced = false;
            dummyTarget.rb.velocity = Vector3.zero;
            dummyTarget.rb.angularVelocity = Vector3.zero;
        }
    }
}
