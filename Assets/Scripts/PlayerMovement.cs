using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //values for movement and jump
    public float speed = 18f;
    public float acceleration = 6f;
    public float deceleration = 10f;
    public float jumpPower = 10f;
    public float jumpHeight = 10f;
    public float fallSpeed = 10f;
    public float jumpDelay = 0.5f;
    public LayerMask mask;

    //private reference values for everything else
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveInput;
    private bool jumpInput;
    private float airTime = 0f;
    private bool inAir = false;
    private ContactFilter2D filter;
    private float delayCounter = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        filter.SetNormalAngle(70, 110); //filters for the bottom angle of the player
        filter.SetLayerMask(mask);  //filters for if the player is touching ground

    }

    // Update is called once per frame
    void Update()
    {
        //gets input
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButton("Jump");
        //checks if the delay is active or not
        if (delayCounter > 0f)
        {
            delayCounter -= Time.deltaTime;
        }
        //checks if the player is on the ground
        if (rb.IsTouching(filter))
        {
            if (inAir) //if the player has just landed starts the delay timer
            {
                delayCounter = jumpDelay;
            }
            inAir = false; //resents air time vals
            airTime = 0f;
        }
        else
        {
            inAir = true; //player is in the air
            airTime += Time.deltaTime; //starts counting for max jump height
        }

    }

    void FixedUpdate() 
    {
        //runs movement calculations on fixedUpdate instead of update (good overall practice)
        Run();
        Jump();
    }
    private void Run()
    {
        float targetSpeed = speed * moveInput; //calculates the target speed
        float speedDif = targetSpeed - rb.velocity.x; //calculates the difference between the current and target speed
        //accelerate or decelerate player depending on current speed
        float accelRate;
        if (Mathf.Abs(targetSpeed) > 0.01f) //checks if the target speed is above 0 (with room for error)
        {
            accelRate = acceleration;
        }
        else
        {
            accelRate = deceleration; //uses deceleration value if player is at target speed
        }

        rb.AddForce((speedDif * accelRate) * Vector2.right, ForceMode2D.Force); //multiplies the difference and the accelRate into a vector (right) and uses force calculations
    }

    private void Jump()
    {
        if (jumpInput && airTime < jumpHeight && delayCounter <= 0f) //checks if space is pressed and if the max height has been reached, and checks if the jump delay is active
        {
            rb.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse); //pushes player up while holding space (needs to be tweaked space button is too sensitive)
        }
        else if ((inAir && !(jumpInput)) || airTime >= jumpHeight) //checks if the player is in the air without holding space, or if the player is in the air and at the max jump height
        {
            rb.AddForce(fallSpeed * Vector2.up, ForceMode2D.Force); //faster drop time after jump
        }

    }
}
