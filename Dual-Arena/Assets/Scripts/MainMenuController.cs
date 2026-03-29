using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Load1v1()
    {
        Debug.Log("Button pressed");
        SceneManager.LoadScene("LoginScreen");
        GameData.isTournamentMode = false;
    }

    public void LoadTournament()
    {
        Debug.Log("Button pressed");
        GameData.lastWinner = "";
        GameData.tournamentMatchIndex = 0;
        GameData.tournamentFinished = false;
        GameData.tournamentMatches.Clear();
        GameData.tournamentPlayers.Clear();
        SceneManager.LoadScene("TournamentScene");
        GameData.isTournamentMode = true;
    }

    public void LoadStats()
    {
        Debug.Log("Button pressed");
        SceneManager.LoadScene("PreStat");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}