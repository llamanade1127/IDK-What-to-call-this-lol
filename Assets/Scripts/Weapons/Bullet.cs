using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnColliderEnter(Collision2D col);

[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    public float damage;
    public float speed;

    private float _timeTillDestroy = 10f;
    public Sprite bullet;
    
    public OnColliderEnter onColliderEnter;

    private void OnCollisionEnter2D(Collision2D col)
    {
        
        onColliderEnter?.Invoke(col);
        if (col.gameObject.TryGetComponent(out Npc npc))
        {
            npc.Hurt(damage);
            Destroy(gameObject);
        } else if (col.gameObject.name != "Bullet")
        {
            Debug.Log(col.gameObject.name);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {


    }

    private void Update()
    {
        _timeTillDestroy -= Time.deltaTime;
        if (_timeTillDestroy <= 0)
        {
            Destroy(gameObject);
        }   
    }

    public virtual void Test()
    {
        Debug.Log("Im working");
    }
}
