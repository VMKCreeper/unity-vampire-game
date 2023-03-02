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
        movementX = Input.GetAxisRaw("Horizontal");
        
        // gravity
        if (!isGrounded() && movementY > -20f){
            movementY -= gravityModifier * Time.deltaTime;
        } else if (isGrounded()){
            myBody.velocity = Vector2.zero;
        }
        myBody.velocity = new Vector2(0f, movementY);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // click jump before touching ground (need timer)
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        MoveForward();

        if (isGrounded())
        {
            if (jumpRequest)
            {
                Jump();
                jumpRequest = false;
            }
        }
        else
        {
            
        }
    }

    void MoveForward()
    {
        // acceleration

        // constant speed
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }

    void Jump()
    {
        movementY = jumpForce;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, GROUND_LAYER);
        return raycastHit.collider != null;
    }
}
