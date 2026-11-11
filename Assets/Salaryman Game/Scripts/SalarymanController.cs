using NUnit.Framework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class SalarymanController : MonoBehaviour
{
    [SerializeField] private Collider2D standingCollider;
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private GameObject attackMelee;
    [SerializeField] private GameObject attackRanged;
    [SerializeField] private GameObject attackRangedSpecial;
    [SerializeField] private GameObject attackMeleeSpecial;
    [SerializeField] private GameObject pointer;
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask youShallNotPass;
    [SerializeField] private float jumpStrength;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pointerDistance;
    [SerializeField] private float dashReloadTime;
    [SerializeField] private float rollReloadTime;
    [SerializeField] private float teleportDistance;
    [SerializeField] public float health = 10;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int currentJumps;
    public int[] salaries = { 100, 200, 300, 400, 500 };
    private bool grounded = true;
    private bool dashing = false;
    private bool canDash = true;
    private bool rolling = false;
    private bool canRoll = true;
    private bool immune = false;
    public bool inSelection = true;
    private Vector2 moveInput;
    private Vector3 mouseScreenPosition;
    private Vector3 mouseWorldPosition;
    private float mouseTriangleOpposite;
    private float mouseTriangleAdjacent;
    private float mouseTriangleRads;
    private float mouseTriangleDegrees;
    private float pointerX;
    private float pointerY;
    private int attackCooldown = 100;
    private int attackCooldownSecondary = 100;
    private int immunityCooldown = 100;
    public static int bossesBeat;
    public int weaponTypePrimary;
    public int weaponTypeSecondary;
    public int primaryWeaponUpgrade;
    public int secondaryWeaponUpgrade;
    public string currentRoom;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        currentJumps = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
        pointer.GetComponent<Rigidbody2D>().transform.position = new Vector2(rb.transform.position.x + 2, rb.transform.position.y);
        camera = Camera.main;

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps > 0 && !dashing)
        {
            rb.linearVelocity = new Vector2 (rb.linearVelocityX, jumpStrength);
            currentJumps--;
            grounded = false;
        }

//Making Pointer Rotate
        mouseScreenPosition = Input.mousePosition; // Gets raw mouse position
        mouseScreenPosition.z = 10f;  //Have to do this otherwise the next thing works funky
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);  //translates the position of the mouse on the screen into world coordinates
        mouseWorldPosition.z = 0;  // Puts it back to even with the screen

        /*This next section is explaining all the math that happens below this
         * 
         * When finding the angle between two points in the world, it looks like a triangle
         * so we can use geomotry to find that angle, starting from the player, the line
         * directly to the camera woud be the hypotenuse of the triangle, and we could find that, 
         * but its easier to find the other two values, the opposite side of the triangle is 
         * just the difference in the y coordinates between the player and the camera, and 
         * the adjacent side is the difference in x coordinates. Using these two, we can 
         * use arctan which while give us the difference in position in radians, and to get that 
         * into degrees, which is how unity reads and writes the roation of a object, we use the unity 
         * function Mathf.Rad2Deg which translates it for us. However it gives the degrees as a 
         * range from -180 to 180, so if it is negative we add 360 to get true 360 degrees. After
         * that we use euler angles (just how unity does rotation) and assign it every update.
         * That plus 20 when actually transforming the pointer is because of some rounding errors with 
         * unity that dont work with geomotry so I just add in 20 degrees and it works alot better.
         */

        mouseTriangleOpposite = mouseWorldPosition.y - transform.position.y;
        mouseTriangleAdjacent = mouseWorldPosition.x - transform.position.x;
        mouseTriangleRads = Mathf.Atan2(mouseTriangleOpposite, mouseTriangleAdjacent);
        mouseTriangleDegrees = mouseTriangleRads * Mathf.Rad2Deg;
        if (mouseTriangleDegrees < 0)
            mouseTriangleDegrees += 360;
        pointer.transform.eulerAngles = new Vector3(0f, 0f, mouseTriangleDegrees);
        //Debug.Log(mouseWorldPosition);
//Making Pointer Move Around the Player (Just finding the point of intersection on that previous triangle with a circle with a radius of pointerDistance)
        pointerX = pointerDistance * Mathf.Cos(mouseTriangleRads) + transform.position.x;
        pointerY= pointerDistance * Mathf.Sin(mouseTriangleRads) + transform.position.y;
        //This just stops it from going into the floor
        if (pointerY < transform.position.y)
            pointerY = transform.position.y;
        pointer.transform.position = new Vector2(pointerX, pointerY);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !grounded)
        {
            if (!(Mathf.Abs(rb.linearVelocity.x) == 0))
            {
                rb.linearVelocity = new Vector2((rb.linearVelocity.x / Mathf.Abs(rb.linearVelocity.x)) * dashSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2((transform.position.x - pointerX) / Mathf.Abs(transform.position.x - pointerX) * dashSpeed * -1, 0);
            }
            canDash = false;
            dashing = true;
            StartCoroutine(DashTimer());
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll && grounded)
        {
            if (rb.linearVelocity.x != 0)
            {
                rb.linearVelocity = new Vector2((rb.linearVelocity.x / Mathf.Abs(rb.linearVelocity.x)) * rollSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2((transform.position.x - pointerX) / Mathf.Abs(transform.position.x - pointerX) * rollSpeed * -1, 0);
            }
            canRoll = false;
            rolling = true;
            StartCoroutine(RollTimer());
        }

        if (Input.GetMouseButtonDown(0) && !inSelection)
        {
            if (weaponTypePrimary == 1 && attackCooldown > 50)
            { 
                Instantiate(attackMelee, gameObject.transform.position, gameObject.transform.rotation);
                attackCooldown = 0;
            }
            else if (weaponTypePrimary == 2 && attackCooldown > 25)
            {
                Instantiate(attackRanged, pointer.transform.position, pointer.transform.rotation);
                attackCooldown = 0;
            }
        }
        else if (Input.GetMouseButtonDown(1) && !inSelection && weaponTypeSecondary != 0)
        {
            if (weaponTypeSecondary == 1 && attackCooldownSecondary > 50)
            {
                Instantiate(attackMelee, gameObject.transform.position, gameObject.transform.rotation);
                attackCooldownSecondary = 0;
            }
            else if (weaponTypeSecondary == 2 && attackCooldownSecondary > 25)
            {
                Instantiate(attackRanged, pointer.transform.position, pointer.transform.rotation);
                attackCooldownSecondary = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (weaponTypePrimary == 1 && attackCooldown > 50 && primaryWeaponUpgrade >= 1)
            {
                //Will be implemented at a later date (soon)
                Vector2 teleportLocation = new Vector2(teleportDistance * Mathf.Cos(mouseTriangleRads) + transform.position.x, teleportDistance * Mathf.Sin(mouseTriangleRads) + transform.position.y);

                Collider2D hit = Physics2D.OverlapPoint(teleportLocation);
                if (hit == null || !hit.gameObject.CompareTag("Boundry"))
                {
                    transform.position = teleportLocation;
                }
                immune = true;
                Instantiate(attackMelee, gameObject.transform.position, gameObject.transform.rotation);
                attackCooldown = 0;
                immunityCooldown = 0;
            }
            else if (weaponTypePrimary == 2 && attackCooldown > 25 && primaryWeaponUpgrade >= 1)
            {
                Instantiate(attackRangedSpecial, pointer.transform.position, pointer.transform.rotation);
                attackCooldown = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if(!dashing && !rolling)
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        attackCooldown++;
        attackCooldownSecondary++;
        immunityCooldown++;
        if (immunityCooldown == 20)
        {
            immune = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            canDash = true;
            currentJumps = maxJumps;
        }
        if (collision.gameObject.CompareTag("Enemy") && !rolling && !immune)
        {
            immunityCooldown = 0;
            immune = true;
            health -= collision.gameObject.GetComponent<EnemyController>().touchingDamage;
            if (health <= 0)
                die();
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("LeftDoor"))
            transform.position += Vector3.right * 5;
        else if (trigger.gameObject.CompareTag("RightDoor"))
            transform.position += Vector3.left * 5;
        else if (trigger.gameObject.CompareTag("TopDoor"))
            transform.position += Vector3.down * 5;
        else if (trigger.gameObject.CompareTag("BottomDoor"))
            transform.position += Vector3.up * 5;
        
        if (trigger.gameObject.CompareTag("EnemyAttack") && !rolling)
        {
            if (!immune)
            {
                immunityCooldown = 0;
                immune = true;
                health -= trigger.gameObject.GetComponent<AttackBehaviour>().damage;
                if (health <= 0)
                    die();
            }
            Destroy(trigger.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("VertRoom"))
        {
            currentRoom = trigger.gameObject.tag;
            camera.transform.position = new Vector3(trigger.GameObject().transform.position.x, transform.position.y, -10);
        }
        if (trigger.gameObject.CompareTag("HorizRoom"))
        {
            currentRoom = trigger.gameObject.tag;
            camera.transform.position = new Vector3(transform.position.x, trigger.GameObject().transform.position.y, -10);
        }
        if (trigger.gameObject.CompareTag("BossRoom"))
        {
            currentRoom = trigger.gameObject.tag;
            camera.transform.position = new Vector3(trigger.GameObject().transform.position.x, trigger.GameObject().transform.position.y, -10);
            camera.orthographicSize = 16;
        }
        if (trigger.gameObject.CompareTag("StaticRoom"))
        {
            currentRoom = trigger.gameObject.tag;
            camera.transform.position = new Vector3(trigger.GameObject().transform.position.x, trigger.GameObject().transform.position.y, -10);
        }
    }

    public void die()
    {
        primaryWeaponUpgrade = 0;
        secondaryWeaponUpgrade = 0;
        weaponTypePrimary = 0;
        weaponTypeSecondary = 0;
        health = 10;
        SceneManager.LoadScene("Test Weapon Selection");
        Destroy(pointer);
        Destroy(gameObject);
    }

    IEnumerator DashTimer()
    {
        yield return new WaitForSeconds(dashReloadTime);
        dashing = false;
    }
    IEnumerator RollTimer()
    {
        yield return new WaitForSeconds(rollReloadTime);
        rolling = false;
        canRoll = true;
    }
}