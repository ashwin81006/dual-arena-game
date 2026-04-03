using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TournamentBracketManager : MonoBehaviour
{
    public TextMeshProUGUI lbracketText;
    public TextMeshProUGUI rbracketText;
    public TextMeshProUGUI bracketText;
    public TextMeshProUGUI matchInfoText;

    public GameObject nextButton;
    public GameObject mmButton;

    public APIManager api;

    IEnumerator Start()
    {
        yield return StartCoroutine(api.GetTournamentMatches(GameData.currentTournamentID));

        if (api.fetchedMatches == null || api.fetchedMatches.Count == 0)
        {
            Debug.LogError("❌ No matches found in DB");
            yield break;
        }

        // rebuild GameData
        GameData.tournamentMatches.Clear();

        foreach (var m in api.fetchedMatches)
        {
            MatchData match = new MatchData();
            match.player1 = m.Player1;
            match.player2 = m.Player2;
            match.winner = m.Winner;

            GameData.tournamentMatches.Add(match);
        }

        GameData.tournamentMatchIndex = 0;

        DisplayBracket();
        UpdateCurrentMatch();
    }
    void HandleWinnerReturn()
    {
        if (GameData.tournamentFinished) return; // 🛑 STOP HERE

        if (string.IsNullOrEmpty(GameData.lastWinner)) return;

        RecordWinner(GameData.lastWinner);

        GameData.lastWinner = "";
    }
    public void RecordWinner(string winner)
    {
        var match = GameData.tournamentMatches[GameData.tournamentMatchIndex];

        match.winner = winner;

        Debug.Log("✅ Winner set for: " + match.player1 + " vs " + match.player2);

        GameData.tournamentMatchIndex++;

        if (AllMatchesCompleted())
        {
            GenerateNextRound();
            GameData.tournamentMatchIndex = 0;
        }

        DisplayBracket();
        UpdateCurrentMatch();

        Debug.Log("Match Index: " + GameData.tournamentMatchIndex);
        Debug.Log("Total Matches: " + GameData.tournamentMatches.Count);
        GameData.winnerProcessed = true;
    }
    void GenerateNextRound()
    {
        List<string> winners = new List<string>();

        foreach (var match in GameData.tournamentMatches)
        {
            winners.Add(match.winner);
        }

        // 🏆 FINAL WINNER
        if (winners.Count == 1)
        {
            Debug.Log("TOURNAMENT WINNER: " + winners[0]);

            GameData.tournamentWinner = winners[0]; // ✅ STORE CORRECTLY
            GameData.tournamentFinished = true;

            return;
        }
        GameData.tournamentMatches.Clear();

        for (int i = 0; i < winners.Count; i += 2)
        {
            MatchData m = new MatchData();
            m.player1 = winners[i];
            m.player2 = winners[i + 1];

            GameData.tournamentMatches.Add(m);
        }
        GameData.currentRound++;
        StartCoroutine(SendNextRoundToDB());

        GameData.tournamentPlayers = winners;
    }

    IEnumerator SendNextRoundToDB()
    {
        List<PlayerSelectionManager.MatchSend> matchList = new List<PlayerSelectionManager.MatchSend>();

        for (int i = 0; i < GameData.tournamentMatches.Count; i++)
        {
            var m = GameData.tournamentMatches[i];

            PlayerSelectionManager.MatchSend data = new PlayerSelectionManager.MatchSend();
            data.player1 = m.player1;
            data.player2 = m.player2;
            data.roundNumber = GameData.currentRound; // or track round separately
            data.matchOrder = i;

            matchList.Add(data);
        }

        PlayerSelectionManager.MatchWrapper wrapper = new PlayerSelectionManager.MatchWrapper();
        wrapper.matches = matchList;

        string json = JsonUtility.ToJson(wrapper);

        WWWForm form = new WWWForm();
        form.AddField("tournamentId", GameData.currentTournamentID);
        form.AddField("matches", json);

        UnityWebRequest www = UnityWebRequest.Post(
            "http://localhost:3000/tournament/createMatches", form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log("✅ Next round saved");
        else
            Debug.LogError("❌ Failed to save next round");
    }
    string GetRoundName(int totalPlayers)
    {
        if (totalPlayers == 16) return "Round of 16";
        if (totalPlayers == 8) return "Quarter Final";
        if (totalPlayers == 4) return "Semi Final";
        if (totalPlayers == 2) return "Final";

        return "Match";
    }

    bool AllMatchesCompleted()
    {
        foreach (var match in GameData.tournamentMatches)
        {
            Debug.Log("Checking winner: " + match.winner);

            if (string.IsNullOrEmpty(match.winner))
                return false;
        }
        return true;
    }
    void DisplayBracket()
    {
        int total = GameData.tournamentPlayers.Count;
        string roundName = GetRoundName(total);

        // Clear all
        lbracketText.text = "";
        rbracketText.text = "";
        bracketText.text = roundName + "\n\n";

        int matchCount = GameData.tournamentMatches.Count;

        // FINAL → center only
        if (matchCount == 1)
        {
            var match = GameData.tournamentMatches[0];

            bracketText.text +=
                roundName + "\n" +
                match.player1 + " vs " + match.player2;

            return;
        }

        // SPLIT LEFT & RIGHT
        int half = matchCount / 2;


        lbracketText.text = roundName + "\n\n" + lbracketText.text;
        for (int i = 0; i < matchCount; i++)
        {
            var match = GameData.tournamentMatches[i];

            string p1 = GetColoredName(match.player1, match.winner);
            string p2 = GetColoredName(match.player2, match.winner);

            string matchText =
                "M" + (i + 1) + "\n" +
                p1 + " vs " + p2 + "\n\n";
            if (i == GameData.tournamentMatchIndex)
            {
                matchText = "<b><color=yellow>" + matchText + "</color></b>";
            }

            if (i < half)
                lbracketText.text += matchText;
            else
                rbracketText.text += matchText;
        }
    }
    void UpdateCurrentMatch()
    {
        int total = GameData.tournamentPlayers.Count;
        string roundName = GetRoundName(total);

        if (GameData.tournamentMatches == null || GameData.tournamentMatches.Count == 0)
        {
            Debug.LogError("❌ No matches available");
            return;
        }

        if (GameData.tournamentMatchIndex >= GameData.tournamentMatches.Count)
        {
            Debug.LogError("❌ Index out of range: " + GameData.tournamentMatchIndex);
            return;
        }

        var match = GameData.tournamentMatches[GameData.tournamentMatchIndex];

        matchInfoText.text =
            "Next Game:\n" + roundName + " - Match " + (GameData.tournamentMatchIndex + 1) + "\n" +
            match.player1 + " vs " + match.player2;
    }
    string GetColoredName(string player, string winner)
    {
        if (string.IsNullOrEmpty(winner))
            return player; // not played yet

        if (player == winner)
            return "<color=green>" + player + "</color>";

        return "<color=red>" + player + "</color>";
    }

    public void mm()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void StartMatch()
    {
        var match = GameData.tournamentMatches[GameData.tournamentMatchIndex];

        GameData.player1Name = match.player1;
        GameData.player2Name = match.player2;
        SceneManager.LoadScene("MapSelection");
    }
}