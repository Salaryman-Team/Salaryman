using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int attackCooldown;
    [SerializeField] public int touchingDamage;
    [SerializeField] public int bulletsPerAttack = 3;
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject player;
    public bool canAttack = false;
    private int attackTimer = 10000;
    private int attackCount = 0;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack)
        {
            if (attackTimer >= attackCooldown)
            {
                if (attackCount == 0)
                {
                    attackCooldown = attackCooldown / 6;
                }
                if (isFacingRight)
                    Instantiate(attack, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);
                else
                    Instantiate(attack, new Vector2(transform.position.x - 1, transform.position.y), new Quaternion(0, 0, 180, 1));
                attackTimer = 0;
                attackCount++;
                if (attackCount == bulletsPerAttack)
                {
                    attackCooldown *= 6;
                    attackCount = 0;
                }
            }
        }
        if (!AmIAboutToWalkOffACliff())
        {
            rb.linearVelocity = new Vector2(player.transform.position.x - transform.position.x, 0).normalized;
        }
        else if (rb.linearVelocityX > 0)
        {
            rb.linearVelocityX -= 0.5f;
        }
        bool dontFlip = Mathf.Abs(player.transform.position.x - transform.position.x) < 1;
        if (player.transform.position.x > transform.position.x && !isFacingRight && !dontFlip)
        {
            flip();
        }
        else if (player.transform.position.x < transform.position.x && isFacingRight && !dontFlip)
        {
            flip();
        }
    }

    private void FixedUpdate()
    {
        attackTimer++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            health -= (collision.gameObject.GetComponent<AttackBehaviour>().damage * (player.GetComponent<SalarymanController>().primaryWeaponUpgrade + 1));
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                Destroy(gameObject);
                if(gameObject.CompareTag("Boss"))
                {
                    player.GetComponent<SalarymanController>().bossesBeat++;
                    player.GetComponent<SalarymanController>().die();
                }
            }
        }
    }

    private bool AmIAboutToWalkOffACliff()
    { 
        RaycastHit2D hit;
        if (player.transform.position.x > transform.position.x)
        {
            hit = Physics2D.Raycast(new Vector2(transform.position.x + 2, transform.position.y - 2), new Vector2(2, -2), float.PositiveInfinity);
            if (hit.collider.gameObject.CompareTag("Ground") && hit.distance < 0.5f)
            {
                return false;
            }
        }
        else
        {
            hit = Physics2D.Raycast(new Vector2(transform.position.x - 2, transform.position.y - 2), new Vector2(-2, -2), float.PositiveInfinity);
            if (hit.collider.gameObject.CompareTag("Ground") && hit.distance < 0.5f)
            {
                return false;
            }
        }
        return true;
    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
