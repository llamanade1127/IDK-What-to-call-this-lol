using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Create Weapon", menuName = "Weapons/CreateWeapon", order = 1)]
public class Weapon : ScriptableObject
{
    [Header("Details")]
    public string weaponName;
    [TextArea]
    public string weaponDescription;
    
    [Header("Art")]
    public Sprite body;
    public Sprite bulletBody;
    public float bulletSizeModifier = 1f;
    [Tooltip("This object must have a animation that will be played after bullet hits something")]
    public GameObject onHitEffect;
    
    //public Bullet bulletClass;
    //public string derivedClass;
    
    [Header("Stats")]
    public RarityType rarityType;

    public FireType fireType;
    
    public float damage;
    
    [Tooltip("This is measured as shots per second")]
    public float fireRate;
    
    [Range(1f, 400f)]
    [Tooltip("Force applied on the bullet")]
    public float bulletSpeed;
    
    [Range(1, 30)]
    [Tooltip("Amount of bullets per shot")]
    public int numOfShots;
    
    [Range(0.1f, 40f)]
    [Tooltip("Angle of spread")]
    public float spread;

    [Range(0, 100f)]
    [Tooltip("How much the player should be kicked back when firing")]
    public float kickback;
    
    public float accuracy;

    public bool canRicochet;
    
    //Stat Changes
    public float moveSpeed;


    [Header("Shot Modifiers")] 
    [Tooltip("Adds a effect to the bullet depending on what is added")]
    public GameObject[] customBulletObjects;
    public GameObject[] customPlayerObjects;
    
    
    [Header("Advanced Custom Modifiers")]
    [Tooltip("Classes to add to the Bullet Object")]
    public string[] customBulletClasses;
    [Tooltip("Classes to add to the player object when weapon is selected")]
    public string[] customPlayerClasses;
    


}

public enum RarityType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Meme
}

public enum FireType
{
    Auto,
    Burst
}