using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TournamentSetupManager : MonoBehaviour
{
    public TMP_Dropdown playerCountDropdown;

    public void OnNext()
    {
        int count = GetSelectedCount();

        GameData.tournamentPlayerCount = count;

        SceneManager.LoadScene("PlayerSelectionScene");
    }

    int GetSelectedCount()
    {
        switch (playerCountDropdown.value)
        {
            case 0: return 4;
            case 1: return 8;
            case 2: return 16;
        }
        return 4;
    }

    public void bak()
    {
        SceneManager.LoadScene("MainMenu");
    }
}