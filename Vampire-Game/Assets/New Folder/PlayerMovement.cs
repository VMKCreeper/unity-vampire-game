using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float gravityModifier = 98f;

    private Rigidbody2D myBody;
    private BoxCollider2D boxCollider;

    [SerializeField] private LayerMask GROUND_LAYER;

    [SerializeField] private float moveForce = 5f;
    [SerializeField] private float jumpForce = 1f;
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
        Debug.Log(movementY);
        movementX = Input.GetAxisRaw("Horizontal");

        myBody.AddForce(Vector2.up * movementY, ForceMode2D.Impulse);
        if (Input.GetButtonDown("Jump"))
        {
            // click jump before touching ground (need timer)
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        MoveForward();
        if (!isGrounded())
        {
            if (movementY > -0.8f)
            movementY -= gravityModifier * Time.deltaTime;
        }
        else
        {
            if (jumpRequest)
            {
                Jump();
            }
        }
    }

    void MoveForward()
    {
        // acceleration

        // constant speed
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;

        // deceleration
    }

    void Jump()
    {
        movementY = jumpForce;
        
        // acceleration

        // hang time

        // deceleration
        jumpRequest = false;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.25f, GROUND_LAYER);
        return raycastHit.collider != null;
    }
}
