using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("sans");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > -447)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
    }

    public void reposition()
    {
        transform.position = new Vector3(-447, transform.position.y, transform.position.z);
    }

    public void returnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
