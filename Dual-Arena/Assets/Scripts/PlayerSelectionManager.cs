using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform leftColumn;
    public Transform rightColumn;
    public GameObject playerRowPrefab;
    public GameObject registerPanel;

    public List<string> selectedPlayers = new List<string>();
    public PasswordManager passwordManager;
    public int totalPlayers;
    private int lockedPlayers = 0;

    public Button nextButton;
    void Start()
    {
        int count = GameData.tournamentPlayerCount;
        if (count == 0) count = 4;
        totalPlayers = count;

        GeneratePlayerRows(count);

        nextButton.interactable = false; // ❌ disabled initially
    }

    public void OnNextPressed()
    {
        GameData.tournamentPlayers = new List<string>(selectedPlayers);

        GenerateBracket();

        UnityEngine.SceneManagement.SceneManager.LoadScene("TournamentMatchScene");
    }

    void GenerateBracket()
    {
        GameData.tournamentMatches.Clear();

        List<string> players = new List<string>(selectedPlayers);

        // 🔀 shuffle players
        for (int i = 0; i < players.Count; i++)
        {
            string temp = players[i];
            int rand = Random.Range(i, players.Count);
            players[i] = players[rand];
            players[rand] = temp;
        }

        // 🎯 create matches
        for (int i = 0; i < players.Count; i += 2)
        {
            MatchData match = new MatchData();
            match.player1 = players[i];
            match.player2 = players[i + 1];

            GameData.tournamentMatches.Add(match);
        }

        foreach (var m in GameData.tournamentMatches)
        {
            Debug.Log(m.player1 + " vs " + m.player2);
        }
    }

    public void AddPlayer(string username)
    {
        if (!selectedPlayers.Contains(username))
        {
            selectedPlayers.Add(username);
        }
    }

    public void OpenRegister()
    {
        registerPanel.SetActive(true);
    }

    public void CloseRegister()
    {
        registerPanel.SetActive(false);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("TournamentScene"); // or previous scene
    }
    public void OnPlayerLocked()
    {
        lockedPlayers++;

        if (lockedPlayers >= totalPlayers)
        {
            nextButton.interactable = true; // ✅ enable next
        }
    }

    void GeneratePlayerRows(int count)
    {
        // clear old
        foreach (Transform child in leftColumn) Destroy(child.gameObject);
        foreach (Transform child in rightColumn) Destroy(child.gameObject);

        int half = count / 2;

        for (int i = 0; i < count; i++)
        {
            Transform parent = (i < half) ? leftColumn : rightColumn;

            GameObject rowObj = Instantiate(playerRowPrefab, parent);

            PlayerRow row = rowObj.GetComponentInChildren<PlayerRow>();
            row.SetPlayerNumber(i + 1);

            // TEMP usernames
            row.passwordManager = passwordManager;
            row.dropdown.ClearOptions();
            row.manager = this;
            row.dropdown.AddOptions(new List<string>
            {
                "Ashwin",
                "Mithun",
                "Player3",
                "Player4",
                "Player5",
                "Player6",
                "Player7",
                "Player8"
            });

        }
    }
}