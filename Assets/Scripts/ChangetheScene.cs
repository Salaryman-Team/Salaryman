using UnityEngine;

public class ChangetheScene : MonoBehaviour
{
    public void goToGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
