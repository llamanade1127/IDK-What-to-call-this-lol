using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Patrol : MonoBehaviour
{
    public enum DrawMode
    {
        NONE,
        GIZMOS_ALWAYS,
        GIZMOS_ON_SELECT
    }

    [Header("Debug Data")]
    public DrawMode mode;

    
    [Header("Data")]
    public GameObject[] Waypoints;


    public int GetNextIndex(int i)
    {
        if (i + 1 >= Waypoints.Length)
        {
            return 0;
        }
        
        return i + 1;
    }

    public void OnDrawGizmosSelected()
    {
        if (mode == DrawMode.GIZMOS_ON_SELECT)
        {
            DrawGizmos();
        }
    }

    private void DrawGizmos()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {

            Gizmos.DrawSphere(Waypoints[i].transform.position, 0.2f);
        }
        
        //Only Draw lines to the other positions if there is more than one
        if (Waypoints.Length > 1)
        {
            // Gizmos.color = lineColor;
            for (int i = 0; i < Waypoints.Length; i++)
            {
                var starting = Waypoints[i].transform.position;
                var to = Waypoints[GetNextIndex(i)].transform.position;
                Gizmos.DrawLine(starting, to);
            }
        }
    }
    public void OnDrawGizmos()
    {
        if (mode == DrawMode.GIZMOS_ALWAYS)
        {
           DrawGizmos();
        }
      
    }

    private bool WaypointsAreValid()
    {
        bool isValid = true;
        for (int i = 0; i < Waypoints.Length; i++)
        {
            
            var starting = Waypoints[i];
            var to = Waypoints[GetNextIndex(i)];
            var test = Physics2D.Raycast(starting.transform.position, transform.TransformDirection(to.transform.position));


            //Vector hit something along the way
            if (test.transform.position != to.transform.position)
                isValid = false;
        }

        return isValid;
    }

    private void OnValidate()
    {
        // if (Waypoints.Length > 1)
        // {
        //     if(!WaypointsAreValid())
        //         Debug.LogError("Waypoints are not valid!");
        // }
    }
}
