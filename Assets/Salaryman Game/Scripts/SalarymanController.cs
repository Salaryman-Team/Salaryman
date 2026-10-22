using UnityEngine;

public class SalarymanController : MonoBehaviour
{
    [SerializeField] private Collider2D standingCollider;
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private GameObject attackMelee;
    [SerializeField] private GameObject attackRanged;
    [SerializeField] private float jumpStrength;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int currentJumps;
    [SerializeField] private int weaponTypePrimary;
    [SerializeField] private int weaponTypeSecondary;

    private bool grounded = true;
    private bool dashing = false;
    private bool canDash = true;
    private bool rolling = false;
    private bool canRoll = true;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumps = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps > 0 && !dashing)
        {
            rb.linearVelocity = new Vector2 (rb.linearVelocityX, jumpStrength);
            currentJumps--;
            grounded = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !grounded)
        {
            rb.linearVelocity = new Vector2((rb.linearVelocity.x / Mathf.Abs(rb.linearVelocity.x)) * dashSpeed, 0);
            canDash = false;
            dashing = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift) && canDash && grounded)
        {
            rb.linearVelocity = new Vector2((rb.linearVelocity.x / Mathf.Abs(rb.linearVelocity.x)) * rollSpeed, 0);
            canRoll = false;
            rolling = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (weaponTypePrimary == 1)
                Instantiate(attackMelee, gameObject.transform.position, gameObject.transform.rotation);
            else if (weaponTypePrimary == 2)
                Instantiate(attackRanged, gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    void FixedUpdate()
    {
        if(!dashing && !rolling)
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            canDash = true;
            dashing = false;
            canRoll = true;
            rolling = false;
            currentJumps = maxJumps;
        }
    }
}