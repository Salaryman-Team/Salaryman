using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    [SerializeField] private int deathTime;
    [SerializeField] public int damage;
    [SerializeField] private float speed;
    private int timer = 0;
    private Rigidbody2D rb;
    private Vector2 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        rb.linearVelocity = transform.right * speed * Time.deltaTime;
        if (timer >= deathTime && speed == 0)
            Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (speed > 0 && collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
