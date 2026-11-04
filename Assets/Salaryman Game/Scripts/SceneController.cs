using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerHealthUI;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthUI.text = "Player Health: " + player.GetComponent<SalarymanController>().health;
    }
}
