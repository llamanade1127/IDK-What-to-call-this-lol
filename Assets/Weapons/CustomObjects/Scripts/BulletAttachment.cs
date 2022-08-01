using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttachment : MonoBehaviour
{
    
    private Transform pTransform;
    private Rigidbody2D pRigidbody;
    private Collider2D pCollider;
    private Bullet bullet;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        pTransform = GetComponentInParent<Transform>();
        pRigidbody = GetComponentInParent<Rigidbody2D>();
        pCollider = GetComponentInParent<Collider2D>();
        bullet = GetComponentInParent<Bullet>();

        bullet.onColliderEnter += OnColliderEnter;
    }

    public virtual void OnColliderEnter(Collision2D col)
    {
        
    }

    
}
