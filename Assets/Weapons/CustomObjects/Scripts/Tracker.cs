using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private Transform parentTransform;
    private Rigidbody2D rb;
    
    public float radius;
    public float turnStrength;
    public LayerMask isEnemy;
    private Transform closest;

    
    private void Awake()
    {
        parentTransform = GetComponentInParent<Transform>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void Update()
    {
        closest = GetClosestEnemy();

        if (closest)
        {
            Debug.Log("turning");
            
            var v = (parentTransform.position - closest.position).normalized;
            var rotateAmount = Vector3.Cross(v, parentTransform.up).z;
            rb.angularVelocity = -rotateAmount * turnStrength;
        }

    }

    private float AngleToPoint(Vector2 position)
    {
        return Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
    }
    private Transform GetClosestEnemy()
    {
        var cast = Physics2D.CircleCast(parentTransform.position, radius, Vector2.zero, radius, isEnemy);

        return cast.transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
