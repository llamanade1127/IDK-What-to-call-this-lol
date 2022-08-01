using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    public enum NpcType
    {
        Enemy,
        Player,
        Trader,
        Friendly
    }

    public Image healthBar;
    public NpcType type;
    public float maxHealth;
    public float health;
    public float armour;

    private void Update()
    {
        if (healthBar)
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }

    public virtual void Hurt(float damage)
    {
        health -= Mathf.Abs(damage - (armour * 0.1f));
        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
        }
    }
    
    
    
}

