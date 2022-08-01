using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private readonly System.Random random = new System.Random();
    [Header("Movenment")]
    public float movementSpeed;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 mousePos;
    
    
    public Camera cam;
    public float cameraFollowLerpDelay;
    
    public bool isInMenu;
    
    //Gun
    [Header("Gun Settings")] 
    public GunPositionController gunController;   
    //
    // public float gunSphereRadius;
    // private Vector2 _gunPos;
    // public Transform gun;
    // public SpriteRenderer gunSprite;
    // private float _timeTillFire;
    

    public Npc npc;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(x,y).normalized;

        if (!isInMenu)
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            gunController.UpdateGunPos(CameraAngle());
        
            if (Input.GetMouseButton(0))
            {
                gunController.Shoot();
            }
        }

        
    }

    void FixedUpdate()
    {
        if (!isInMenu)
            Move();

        MoveCamera();
    }
    

    
    void Move()
    {
        rb.AddForce(moveDirection * movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
    }

    float CameraAngle()
    {
        Vector2 lookDir = mousePos - rb.position;
        return Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
    }
    
    
    
    
    /// <summary>
    /// Lerps the camera position to follow the player smoothly
    /// </summary>
    void MoveCamera()
    {
        var position = rb.transform.position;
        var camPos = cam.transform.position;

        var camX = Mathf.Lerp(camPos.x, position.x, cameraFollowLerpDelay);
        var camY = Mathf.Lerp(camPos.y, position.y, cameraFollowLerpDelay);
        
        cam.transform.position = new Vector3(camX, camY, -10f);
    }
    
}


