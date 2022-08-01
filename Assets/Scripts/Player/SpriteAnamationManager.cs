using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpriteAnamationManager : MonoBehaviour
{
    private enum CurrentVector
    {
        Right,
        Up,
        Left,
        Down
    }

    private CurrentVector cv;
    public GameObject gunBody;
    public SpriteRenderer renderer;
    public bool useRightForLeft = true;
    [Tooltip("Goes in Right, Up, Left, Down order")]
    public Sprite[] sprites = new Sprite[4];
    
    //Animations
    public Animator animator;
    public Rigidbody2D rb;
    public Animation anim;
    
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Direction = Animator.StringToHash("direction");

    private static readonly int[] directionTriggers = new[]
    {
        Animator.StringToHash("right"), Animator.StringToHash("up"), 
        Animator.StringToHash("left"), Animator.StringToHash("down")
    };

    private void Update()
    {
        UpdateCurrentSprite();
    }

    public void UpdateCurrentSprite()
    {
        var position = gunBody.transform.localPosition;
        
        //Get angle by gun position
        var v = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;

        var p = (int)(Mathf.Ceil(AddAngles(v, 45) / 90) - 1);



        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        

        if (x > 0 || y > 0)
        {
            animator.SetFloat(Speed, 10);
        }
        else
        {
            animator.SetFloat(Speed, 0);
        }
        
        if (p == 2 && useRightForLeft)
        {
            renderer.flipX = true;
            SetTrigger(p);
        }
        else
        {
            renderer.flipX = false;
            SetTrigger(p);
        }


    }

    private void SetTrigger(int index)
    {
        for (int i = 0; i < directionTriggers.Length; i++)
        {
            if(i == index)
                animator.SetTrigger(directionTriggers[i]);
            else 
                animator.ResetTrigger(directionTriggers[i]);
        }
    }
    private float AddAngles(float a, float b)
    {
        a += b;
        a %= 360;

        if (a < 0)
        {
            a += 360;
        }

        return a;
    }
}
