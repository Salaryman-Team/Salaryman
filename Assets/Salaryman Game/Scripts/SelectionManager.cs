using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject meleePrimary;
    [SerializeField] private GameObject rangedPrimary;
    [SerializeField] private GameObject upgrade;
    [SerializeField] private TextMeshProUGUI moneyCount;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    private int moneySpent;
    private int primaryUpgrades = 0;
    private int moneytoSpend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moneytoSpend = player.GetComponent<SalarymanController>().salaries[player.GetComponent<SalarymanController>().bossesBeat];
        moneyCount.text = "You have : $" + (moneytoSpend - moneySpent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMelee()
    {
        if (player.GetComponent<SalarymanController>().weaponTypePrimary == 0)
        {
            player.GetComponent<SalarymanController>().weaponTypePrimary = 1;
            moneySpent += 50;
            moneyCount.text = "You have : $" + (moneytoSpend - moneySpent);
            Destroy(rangedPrimary);
            Destroy(meleePrimary);
        }
        else if (player.GetComponent<SalarymanController>().weaponTypePrimary != 1 && (moneytoSpend - moneySpent) >= 50)
        {
            player.GetComponent<SalarymanController>().weaponTypeSecondary = 1;
            moneySpent += 50;
            moneyCount.text = "You have : $" + (moneytoSpend - moneySpent);
        }
    }

    public void setRanged()
    {
        if (player.GetComponent<SalarymanController>().weaponTypePrimary == 0)
        {
            player.GetComponent<SalarymanController>().weaponTypePrimary = 2;
            moneySpent += 50;
            Destroy(rangedPrimary);
            Destroy(meleePrimary);
            moneyCount.text = "You have : $" + (moneytoSpend - moneySpent);
        }
        else if (player.GetComponent<SalarymanController>().weaponTypePrimary != 2 && (moneytoSpend - moneySpent) >= 50)
        {
            player.GetComponent<SalarymanController>().weaponTypeSecondary = 2;
            moneyCount.text = "You have : $" + (moneytoSpend - moneySpent);
        }
    }

    public void upgradePrimary()
    {
        if (moneytoSpend - moneySpent >= 25 + (5 * primaryUpgrades) && primaryUpgrades < 4)
        {
            moneySpent += 25 + (5 * primaryUpgrades);
            primaryUpgrades++;
            moneyCount.text = "You have : $" + (moneytoSpend - moneySpent);
            upgradeCost.text = "Upgrade your primary for $" + (25 + (5 * primaryUpgrades));
        }
    }

    public void loadScene()
    {
        player.GetComponent<SalarymanController>().primaryWeaponUpgrade = primaryUpgrades;
        SceneManager.LoadScene("Test Level");
    }
}
