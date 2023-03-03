using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float gravityModifier;

    private Rigidbody2D myBody;
    private BoxCollider2D boxCollider;
    private BoxCollider2D wallCollider;

    [SerializeField] private LayerMask GROUND_LAYER;

    [SerializeField] private float moveForce;
    [SerializeField] private float jumpForce;
    private float movementX;
    private float movementY;
    private float speed;
    private float move;

    private bool onGround;
    private bool jumpRequest = false;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        boxCollider = gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // click jump before touching ground (need timer)
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        if (hitCeiling())
        {
            movementY = 0;
        }
        applyGravity();
        MoveForward();
        Jump();
    }

    void MoveForward()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        // acceleration
        // if (movementX == 0)
        // {
        //     speed = moveForce - 1;
        // }
        // else
        // {
        //     if (speed < moveForce && isGrounded())
        //     {
        //         speed += 0.25f;
        //     }
        // }

        // constant speed
        transform.position += new Vector3(speed, 0f, 0f) * Time.deltaTime * movementX;
        // float acceleration = 3.5f;
        // float decceleration = 3.5f;

        // float topSpeed = 7;
        // float targSpeed = movementX * topSpeed;

        // float speedDiff = targSpeed - myBody.velocity.x;
        // float accelRate = (Mathf.Abs(targSpeed) > 0.01f) ? acceleration : decceleration;

        // float velPower = 2;

        // move = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
        // Debug.Log(move);
        move = moveForce * movementX;
    }

    void Jump()
    {
        if (isGrounded())
        {
            if (jumpRequest)
            {
                movementY = jumpForce;
                jumpRequest = false;
            }
        }
    }

    void applyGravity()
    {
        if (!isGrounded() && movementY > -20f)
        {
            float gravity = gravityModifier; // rise speed (default)
            if (movementY < 5 && movementY > -5)
            {
                // hangtime
                gravity = gravityModifier - 10;
            }
            else if (movementY < -5)
            {
                // fall speed (faster than rise)
                gravity = gravityModifier + 50;
            }
            movementY -= gravity * Time.deltaTime;
        }
        else if (isGrounded())
        {
            if (movementY < 0)
            {
                movementY = 0; // reset force when touching ground
            }
        }
        myBody.velocity = new Vector2(move, movementY); // changed
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, GROUND_LAYER);
        return raycastHit.collider != null;
    }

    private bool hitCeiling()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.up, 0.1f, GROUND_LAYER);
        return raycastHit.collider != null;
    }
}