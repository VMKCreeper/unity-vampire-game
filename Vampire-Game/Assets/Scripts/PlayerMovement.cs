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
    [SerializeField] private float dashForce;
    
    private float movementX;
    private float movementY;
    private float speed;
    private float move;

    private float wallJumpDirection = 1f;
    private float movementMultiplier = 1f;

    private bool isWallHanging = false;
    private bool isCeilingHanging = false;
    private bool isWallJumping = false;
    private bool isDashing = false;

    private bool jumpRequest = false;
    private bool dashRequest = false;
    private bool canDash = true;

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
            StartCoroutine(jumpGrace(0.1f));
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashRequest = true;
        }
    }

    void FixedUpdate()
    {
        if (hitCeiling())
        {
            movementY = 0;
        }
        if (isGrounded() || isWallHanging)
        {
            canDash = true;
        }
        if (dashRequest)
        {
            StartCoroutine(dash());
        }
        if (!isDashing && !isCeilingHanging)
        {
            if (!isWallHanging)
            {
                applyGravity();
            }
            moveForward();
        }
        wallHang();
        wallClimb();
        ceilingHang();
        ceilingClimb();
        jump();
    }

    private void moveForward()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        // need fix

        float acceleration = 3.5f;
        float decceleration = 3.5f;

        float targSpeed = movementX * moveForce;

        float speedDiff = targSpeed - myBody.velocity.x;
        float accelRate = (Mathf.Abs(targSpeed) > 0.01f) ? acceleration : decceleration;

        float velPower = 2;

        float move = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        myBody.AddForce(move * Vector2.right * movementMultiplier);
    }

    private void jump()
    {
        if (jumpRequest)
        {
            if (isGrounded()){
                movementY = jumpForce;
                jumpRequest = false;
            } else if(hitWall() && isWallHanging)
            {
                // wall jump
                isWallJumping = true;
                movementMultiplier = 0.1f;
                movementY = jumpForce;
                jumpRequest = false;
            }
        }
        if(isWallJumping){
            myBody.velocity = new Vector2((moveForce + 1) * wallJumpDirection, movementY);
        }
    }

    private void wallHang()
    {
        if (hitWall() && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && movementY <= 0 && !isWallJumping)
        {
            movementY = 0;
            myBody.velocity = Vector3.zero;
            isWallHanging = true;
            return;
        }
        isWallHanging = false;
    }

    private void ceilingHang()
    {
        if (hitCeiling() && Input.GetKey(KeyCode.W))
        {
            movementY = 0;
            myBody.velocity = Vector3.zero;
            isCeilingHanging = true;
            return;
        }
        isCeilingHanging = false;
    }

    private void applyGravity()
    {
        if (!isGrounded() && movementY > -20f)
        {
            float gravity = gravityModifier; // rise speed (default)
            if (movementY < 5 && movementY > -5)
            {
                // hangtime
                gravity = gravityModifier - 15;
                // regain control of jump
                if (movementY < 0){
                    isWallJumping = false;
                    movementMultiplier = 1;
                }
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
    }

    private void wallClimb(){
        // need stanima bar
        float vertical = Input.GetAxisRaw("Vertical");
        if(vertical != 0 && isWallHanging){
            movementY = 0;
            myBody.AddForce(Vector2.up * 200 * vertical);
        }
    }

    private void ceilingClimb()
    {
        // need stanima bar
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0 && isCeilingHanging)
        {
            movementY = 0;
            myBody.AddForce(Vector2.right * 150 * horizontal);
        }
    }

    private IEnumerator dash(){
        dashRequest = false;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
        {
            // prevent hovering in air when holding a and d or w and s
            yield break;
        }

        float dashModifier = 1;
        if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) == 2)
        {
            dashModifier = 0.75f; // diagonal speed decrease cuz maths make it faster
        }

        float xAxis = dashForce * horizontal;
        float yAxis = dashForce * vertical;

        isDashing = true;
        // reset
        isWallJumping = false;
        canDash = false;
        movementY = 0;

        // acceleration
        myBody.velocity = new Vector2(xAxis * (dashModifier - 0.2f), yAxis * (dashModifier - 0.2f));
        yield return new WaitForSeconds(0.05f);
        // constant speed
        myBody.velocity = new Vector2(xAxis * (dashModifier), yAxis * (dashModifier));
        yield return new WaitForSeconds(0.1f);
        // decceleration
        myBody.velocity = new Vector2(xAxis * (dashModifier - 0.2f), yAxis * (dashModifier - 0.2f));
        yield return new WaitForSeconds(0.05f);
        isDashing = false;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.03f, GROUND_LAYER);
        return raycastHit.collider != null;
    }

    private bool hitCeiling()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.up, 0.1f, GROUND_LAYER);
        return raycastHit.collider != null;
    }

    private bool hitWall()
    {
        RaycastHit2D raycastLeft = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.left, 0.03f, GROUND_LAYER);
        RaycastHit2D raycastRight = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.right, 0.03f, GROUND_LAYER);
        
        if (raycastLeft.collider != null || raycastRight.collider != null)
        {
            if (jumpRequest && isWallHanging)
            {
                if (raycastLeft.collider != null)
                {
                    wallJumpDirection = 1;
                }
                else if (raycastRight.collider != null)
                {
                    wallJumpDirection = -1;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator jumpGrace(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        jumpRequest = false;
    }
}