using UnityEngine;

public class Tomato : MonoBehaviour
{ 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().addTomato();
            Destroy(gameObject);
        }
    }
}
