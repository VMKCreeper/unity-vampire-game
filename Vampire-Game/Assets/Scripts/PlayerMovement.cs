using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float gravityModifier;
    public float fallGravityMulti;

    private Rigidbody2D myBody;
    private BoxCollider2D boxCollider;
    private BoxCollider2D wallCollider;

    [SerializeField] private LayerMask GROUND_LAYER;

    [SerializeField] private float moveForce;
    [SerializeField] private float jumpForce;
    private float movementX;
    //private float movementY;
    private float speed;

    private bool onGround;
    private bool jumpRequest = false;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        boxCollider = gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();
        myBody.gravityScale = gravityModifier;

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
            //movementY = 0;
        }
        //applyGravity();
        MoveForward();
        appGravity();
        Jump();
    }

    void MoveForward()
    {
        movementX = Input.GetAxisRaw("Horizontal");

        float acceleration = 3.5f;
        float decceleration = 3.5f;

        float topSpeed = 6;
        float targSpeed = movementX * topSpeed;

        float speedDiff = targSpeed - myBody.velocity.x;
        float accelRate = (Mathf.Abs(targSpeed) > 0.01f) ? acceleration : decceleration;

        float velPower = 2;

        float move = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        if (hitWall != true)
        {
            myBody.AddForce(move * Vector2.right);
        }
    }

    void Jump()
    {
        if (isGrounded())
        {
            if (jumpRequest)
            {
                //movementY = jumpForce;
                myBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequest = false;
            }
        }
    }

/*    void applyGravity()
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
        myBody.velocity = new Vector2(0f, movementY);
    }*/

    void appGravity()
    {
        if (myBody.velocity.y < 0)
        {
            myBody.gravityScale = gravityModifier * fallGravityMulti;
        }
        else
        {
            myBody.gravityScale = gravityModifier;
        }
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

    private bool hitWall()
    {
        RaycastHit2D raycastLeft = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.left, 0.1f, GROUND_LAYER);
        RaycastHit2D raycastRight = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.right, 0.1f, GROUND_LAYER);

        if (raycastLeft.collider != null || raycastRight.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}