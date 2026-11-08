using UnityEngine;

public class EnemyRadiusChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Collider2D enemyRadius;
    [SerializeField] private GameObject player;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(trigger.CompareTag("Enemy"))
            trigger.gameObject.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (trigger.CompareTag("Enemy"))
            trigger.gameObject.SetActive(false);
    }
}
