using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Load1v1()
    {
        Debug.Log("Button pressed");
        SceneManager.LoadScene("LoginScreen");
    }

    public void LoadTournament()
    {
        Debug.Log("Button pressed");
        SceneManager.LoadScene("TournamentScene");
    }

    public void LoadStats()
    {
        Debug.Log("Button pressed");
        SceneManager.LoadScene("StatsScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}