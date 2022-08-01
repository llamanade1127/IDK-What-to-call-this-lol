using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public delegate void OnShoot();

public class GunPositionController : MonoBehaviour
{
    public float gunSphereRadius;
    private Vector2 _gunPos;
    public GameObject gunObj;
    private Transform _gunTransform;
    private SpriteRenderer _gunSprite;
    private float _timeTillFire;
    public bool canFire;
    public Weapon weapon;
    private Random random = new Random();
    private float _currentAngle;

    public event OnShoot onShootEvent;
    private bool isOverridden;
    private Rigidbody2D rb;
    
    //Burst data


    private void Start()
    {
        _gunTransform = gunObj.GetComponent<Transform>();
        _gunSprite = gunObj.GetComponent<SpriteRenderer>();
        _timeTillFire = 0;
        rb = GetComponent<Rigidbody2D>();
        UpdateWeapon(weapon);
    }

    private void Update()
    {
        _timeTillFire -= Time.deltaTime * 1000f;
        
        _gunTransform.localPosition = GetGunVectorWithAngle(_currentAngle);
        _gunTransform.rotation = Quaternion.Euler(0.0f, 0.0f, _currentAngle);
    }
    public float GetAngle()
    {
        return _currentAngle;
    }
    public void UpdateWeapon(Weapon _weapon)
    {
        
        //Remove custom player classes
        for (int i = 0; i < weapon.customPlayerClasses.Length; i++)
        {
            //If this class is on a obj then it is the player.
            if (TryGetComponent<PlayerMovement>(out PlayerMovement pm))
            {
                if(TryGetComponent(Type.GetType(weapon.customPlayerClasses[i]), out Component weaponClass))
                {
                    Destroy(weaponClass); 
                }
            }

        }

        weapon = ScriptableObject.CreateInstance<Weapon>();
        weapon = _weapon;
        _gunSprite.sprite = _weapon.body;
        //Add Classes
        for (int i = 0; i < weapon.customPlayerClasses.Length; i++)
        {
            //If this class is on a obj then it is the player.
            if (TryGetComponent(out PlayerMovement pm))
            {
                try
                {
                    gameObject.AddComponent(Type.GetType(weapon.customPlayerClasses[i]));
                }
                catch (Exception e)
                {
                    Debug.LogError($"Class {weapon.customPlayerClasses[i]} does not exist!");
                }
            }
        }
        
    }


    /// <summary>
    /// Updates the guns position using the inputted angle
    /// </summary>
    /// <param name="angle"></param>
    public void UpdateGunPos(float angle)
    {
        _currentAngle = angle;
    }

    /// <summary>
    /// Returns the time until the next shot can be taken in milliseconds
    /// </summary>
    /// <returns></returns>
    public float TimeTillNextShot()
    {
        return _timeTillFire;
    }
    
    //TODO: Turn into IEnumerator
    public void Shoot()
    {
        if (_timeTillFire > 0) return;
        
        onShootEvent?.Invoke();
        
        _timeTillFire = 1000/weapon.fireRate;
        
        for (int i = 0; i < weapon.numOfShots; i++)
        {
            var obj = MakeBullet();
            obj.transform.position = _gunTransform.position;

            var shot = GetShotVector(_currentAngle);
        
            obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, shot.Angle - 90f);

            var bulletRb = obj.GetComponent<Rigidbody2D>();
            
            bulletRb.AddForce(shot.Vector * weapon.bulletSpeed * 100f, ForceMode2D.Force);
        }

        var kickbackVector = GetKickbackVector(_currentAngle);

        rb.AddForce(kickbackVector * weapon.kickback * 100 );
    }
    
    
    
    
    
    void UpdateGunType(Weapon _weapon)
    {
        weapon = _weapon;
        _gunSprite.sprite = _weapon.body;
    }
    
    
    
    
    //Helper Functions
    GameObject MakeBullet()
    {
        var obj = new GameObject
        {
            name = "Bullet",
        };

        //Set Size
        obj.transform.localScale = new Vector3(weapon.bulletSizeModifier, weapon.bulletSizeModifier, 1);
        
        //Create base components
        var spriteRenderer = obj.AddComponent<SpriteRenderer>();

        var boxCollider2D = obj.AddComponent<BoxCollider2D>();
        var objRb = obj.AddComponent<Rigidbody2D>();
        var bullet = obj.AddComponent<Bullet>();

        foreach (var t in weapon.customBulletClasses)
        {
            obj.AddComponent(Type.GetType(t));
        }

        foreach (var t in weapon.customBulletObjects)
        {
            Instantiate(t, obj.transform);
        }
        

        //Set Components
        spriteRenderer.sprite = weapon.bulletBody;
        bullet.damage = weapon.damage;
        bullet.speed = weapon.bulletSpeed;


        boxCollider2D.isTrigger = true;
        boxCollider2D.size = new Vector2(weapon.bulletSizeModifier, weapon.bulletSizeModifier);
        objRb.gravityScale = 0;

        for (int i = 0; i < weapon.customBulletClasses.Length; i++)
        {
            obj.AddComponent(Type.GetType(weapon.customBulletClasses[i]));
        }

        return obj;
    }
    
    
    Vector2 GetGunVectorWithAngle(float angle)
    {
        float gX = gunSphereRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float gY = gunSphereRadius * Mathf.Sin(angle * Mathf.Deg2Rad) ;
        return new Vector2(gX, gY);
    }

    Vector2 GetKickbackVector(float angle)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        return -new Vector2(x, y);
    }

    ShotVector GetShotVector(float midAngle)
    {

        var t1 = midAngle - (weapon.spread / 2);
        var t2 = midAngle + (weapon.spread / 2);


        var randomAngle = random.Next((int) t1, (int) t2);


        var shotVect = GetGunVectorWithAngle(randomAngle);

        return new ShotVector(shotVect, randomAngle);
    }
    
    
    private struct ShotVector
    {
        public Vector2 Vector;
        public float Angle;


        public ShotVector(Vector2 vector, float angle)
        {
            Vector = vector;
            this.Angle = angle;
        }
    }
}
