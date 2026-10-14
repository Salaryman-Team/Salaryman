using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int tomatos = 0;
    bool grounded;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (grounded)
            {
                rb.linearVelocity = new Vector2(7, rb.linearVelocityY);
            }
            else
            {
                rb.linearVelocity = new Vector2(6, rb.linearVelocityY);
            }
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (grounded)
            {
                rb.linearVelocity = new Vector2(-7, rb.linearVelocityY);
            }
            else
            {
                rb.linearVelocity = new Vector2(-6, rb.linearVelocityY);
            }
        }
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 7);
        }
        if (tomatos >= 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bone"))
        {
            grounded = true;
        }
        if (collision.gameObject.CompareTag("death"))
        {
            die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bone"))
        {
            grounded = false;
        }
    }

    public void addTomato()
    {
        tomatos++;
    }

    void die()
    {
        transform.position = new Vector3(-447, 10, 0);
        GameObject.Find("Main Camera").GetComponent<CameraController>().reposition();
    }
}
