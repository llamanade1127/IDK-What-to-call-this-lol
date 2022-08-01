using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Npc npc;
    public Transform bar;

    private void Start()
    {
    }

    private void Update()
    {
        bar.localScale = new Vector3((npc.health / npc.maxHealth) - 0.01f, 0.94f, 1);
    }
}
