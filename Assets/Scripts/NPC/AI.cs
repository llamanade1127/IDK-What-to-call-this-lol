using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AI : MonoBehaviour
{
    public enum DebugMode
    {
        OnSelect,
        Always,
        None
    }

    public enum EnemyType
    {
        Gunner,
        StabbyBoi
    }

    public enum SpottingType
    {
        Raycast, //Will cast rays to see if player is there
        Box, //If player enters a box they will automatically detect them
        Always //Can always see the player, hunting/patrol will not apply to this
    }

    public enum PatrolType
    {
        Points,
        Area,
        None
    }
    
    [Header("Debug Data")]
    public DebugMode mode;
    

    [Header("Enemy Data")]
    public State currentState;
    public EnemyType enemyType;
    public SpottingType spottingType;
    public PatrolType patrolType;
    public GameObject Player;
    public Rigidbody2D rb;
    public float movementSpeed;
    public bool _canMove = true;



    [Header("Gun Data")]
    //TODO: Redo this data
    public float shootDistance;
    public float closestDistance;
    public float shootDelayTime;
    public int shotsPerBurst;
    public float fireRateModifier;
    private GunPositionController _gunPositionController;
    private bool canShoot = true;

    [Header("Hunting Data")] 
    public float waitingTime;
    
    [Header("Patrol Data")]
    public Patrol patrolPoints;

    public float turnSpeed;
    private float minDistance = 0.5f;
    private int pointIndex = 0;
    public Vector3 _currentTarget;
    
    [Header("FOV")]
    [Range(1, 360)]
    public float FOV;
    public float ViewDistance;
    public float strongSpotDistance;
    public float CurrentAngle;
    public float RayIndexIncrement = 1f;
    public LayerMask rayLayerMask;
    public float softLerpValue;
    public float hardLerpValue;
    private bool EnemySpotted;

    public enum State
    {
        Patrol, //NPC has no stimulus so they patrol 
        Hunting, //Something was heard and it moves to vector to investigate
        Fighting, //Spotted enemy and will fight them
        Fleeing //If low on health, run away
    }


    private void Start()
    {
        _gunPositionController = GetComponent<GunPositionController>();

        _gunPositionController.onShootEvent += OnShoot;
    }

    public void Update()
    {
        _gunPositionController.UpdateGunPos(CurrentAngle);
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Fighting:
                Fight();
                break;
            case State.Hunting:
                Hunt();
                break;
        }

        CheckState();

    }

    private void CheckState()
    {
        Spot();
        
        switch (currentState)
        {
            case State.Patrol:
                if (EnemySpotted)
                    currentState = State.Fighting;
                break;
            case State.Fighting:
                if (!EnemySpotted)
                    currentState = State.Hunting;
                break;
        }
    }

    public void FixedUpdate()
    {

        Move();
    }

    #region States

    private void Patrol()
    {
        switch (patrolType)
        {
            case PatrolType.Points:
                //Get distance to next point
                var distance = Vector3.Distance(transform.position, patrolPoints.Waypoints[pointIndex].transform.position);
        
                //If distance is less than threshold we increment the waypoint and face the angle to the next waypoint
                if (distance < minDistance)
                {
                    pointIndex = patrolPoints.GetNextIndex(pointIndex);
                }

                //Set our move target
                _currentTarget = patrolPoints.Waypoints[pointIndex].transform.position;

                break;
            
        }

    }

    private void Fight()
    {
        var distance = Vector2.Distance(transform.position, _currentTarget);
        
        //Shoot if we can otherwise we just keep moving
        if (distance <= shootDistance)
        {
            if (canShoot)
            {
                StartCoroutine(Shoot());
            }
        } 
    }

    private void Hunt()
    {

        var distance = Vector2.Distance(transform.position, _currentTarget);

        if (distance <= minDistance)
        {

            //Wait for a few seconds at that position
            StartCoroutine(WaitAtPosition());
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        var timeToWait = ((1000 / _gunPositionController.weapon.fireRate) / 1000) * fireRateModifier;
        Debug.Log(timeToWait);
        _gunPositionController.Shoot();
        yield return new WaitForSeconds(timeToWait);
        canShoot = true;
    }


    
    private IEnumerator WaitAtPosition()
    {
        _canMove = false;
        yield return new WaitForSeconds(waitingTime);
        _canMove = true;
        if (!EnemySpotted)
            currentState = State.Patrol;
    }

    #endregion
   
    
    private void Move()
    {
        var angle = AngleToVector(_currentTarget);
        CurrentAngle = Mathf.LerpAngle(CurrentAngle, angle, EnemySpotted ? hardLerpValue : turnSpeed);
        
        var x = Mathf.Cos(CurrentAngle * Mathf.Deg2Rad);
        var y = Mathf.Sin(CurrentAngle * Mathf.Deg2Rad);

        var position = transform.position;
        var distance = Vector2.Distance(_currentTarget, position);

        if (_canMove)
        {
            if (currentState == State.Fighting && distance <= shootDistance)
            {

                //If the player gets too close we back off
                if(distance < closestDistance)
                    rb.AddForce(-new Vector2(x, y) * movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(x, y) * movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
            }

        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        
      
    }

    private float AngleToVector(Vector3 point)
    {
        var facingVector = point-transform.position ;
        var angle = Mathf.Atan2(facingVector.y, facingVector.x) * Mathf.Rad2Deg ;
        return angle;
    }

    #region Debug

    private void OnDrawGizmos()
    {
        if (mode == DebugMode.Always)
        {
            DrawGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mode == DebugMode.OnSelect)
        {
           DrawGizmos();
        }

 
       
    }
    
    private void DrawGizmos()
    {
        switch (spottingType)
        {
            case SpottingType.Raycast:
                for (float i = ((FOV / 2) - FOV); i < FOV - FOV / 2; i += RayIndexIncrement)
                {

                    float CurrentRayAngle = CurrentAngle + i;
                    var RayVector = GetVectorOfAngle(CurrentRayAngle, ViewDistance);
                    var position = transform.position;
                    var test = Physics2D.Raycast(position, transform.TransformVector(RayVector), Single.MaxValue,
                        rayLayerMask);


                    if (test.collider != null && test.distance < ViewDistance)
                    {

                        var angle = AngleToVector(test.transform.position);
                        if (test.distance < strongSpotDistance)
                        {

                            //CurrentAngle = Mathf.LerpAngle(CurrentAngle, angle, hardLerpValue);
                            Gizmos.color = Color.red;
                        }
                        else
                        {
                            Gizmos.color = Color.yellow;
                            //CurrentAngle = Mathf.LerpAngle(CurrentAngle, angle, softLerpValue);
                        }


                        Gizmos.DrawRay(position,
                            transform.TransformVector(Vector3.ClampMagnitude(RayVector, test.distance)));
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawRay(position, transform.TransformVector(RayVector));
                    }
                }

                break;
            case SpottingType.Always:
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _currentTarget);
                break;
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_currentTarget, 1f);
            
    }
    #endregion
    private void OnShoot()
    {
        canShoot = true;
    }

    private void Spot()
    {
        switch (spottingType)
        {
            case SpottingType.Raycast:
                CastFOVRays();
                break;
            case SpottingType.Always:
                GetClosestPlayer();
                break;
        }
    }

    private void GetClosestPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        var closest = float.MaxValue;
        GameObject closestPlayer = null;
        for (int i = 0; i < players.Length; i++)
        {
            if (Vector2.Distance(players[i].transform.position, transform.position) < closest)
            {
                closestPlayer = players[i];
                closest = Vector2.Distance(players[i].transform.position, transform.position);
            }
        }

        if(closestPlayer != null)
        {
            _currentTarget = closestPlayer.transform.position;
            currentState = State.Fighting;
        }
        
    }
    private void CastFOVRays()
    {
        bool somethingSpotted = false;



        for (float i = ((FOV / 2) - FOV); i < FOV - FOV / 2; i += RayIndexIncrement)
        {

            float CurrentRayAngle = CurrentAngle + i;
            var RayVector = GetVectorOfAngle(CurrentRayAngle, ViewDistance);
            var position = transform.position;
            var raycast = Physics2D.Raycast(position, transform.TransformVector(RayVector), Single.MaxValue,rayLayerMask);


            if (raycast.collider != null && raycast.distance < ViewDistance)
            {
                EnemySpotted = true;
                somethingSpotted = true;
                _currentTarget = raycast.transform.position;
                currentState = State.Fighting;
            }
        }
        
        if (EnemySpotted && !somethingSpotted)
        {
            EnemySpotted = false;
        }
    }
    private Vector2 GetVectorOfAngle(float angle, float radius)
    {
        float gX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float gY = radius * Mathf.Sin(angle * Mathf.Deg2Rad) ;
        return new Vector2(gX, gY);
    }
    private void OnValidate()
    {
        if (RayIndexIncrement <= 0)
            RayIndexIncrement = 0.001f;
    }
}
