using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MapSelectionManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject[] highlights;
    string[] mapsname = { "Castle Arena", "Colosseum Arena", "Prison Arena", "Cave Arena" };

    int selectedMap = -1;

    public void SelectMap(int index)
    {
        GameData.selectedMap = index;
        selectedMap = index;

        // 1. TURN OFF ALL
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].SetActive(false);
        }

        // 2. TURN ON SELECTED
        highlights[index].SetActive(true);

        statusText.text = "Selected Map: " + mapsname[index];
    }

    public void StartGame()
    {
        if (selectedMap == -1)
        {
            statusText.text = "Select a map!";
            return;
        }

        SceneManager.LoadScene("DualScene");
    }

    public void GoBack()
    {
        if (GameData.isTournamentMode)
            SceneManager.LoadScene("TournamentMatchScene");
        else
            SceneManager.LoadScene("LoginScreen");
    }
}