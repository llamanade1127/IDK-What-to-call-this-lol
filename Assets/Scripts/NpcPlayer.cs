using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPlayer : Npc
{
    private void Update()
    {
    }

    public override void Hurt(float damage)
    {
        health -= damage;
    }
}
