using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class PlayerStat
{
    public string Username;
    public int Wins;
}

[System.Serializable]
public class WinRateData
{
    public string Username;
    public float WinRate;
}

[System.Serializable]
public class MapData
{
    public string MapName;
    public int TimesPlayed;
}

[System.Serializable]
public class PlayerStatList
{
    public PlayerStat[] data;
}

[System.Serializable]
public class MatchDataUI
{
    public string Player1;
    public string Player2;
    public string Winner;
}
public class StatsUIManager : MonoBehaviour
{
    public APIManager api;

    public GameObject resultsPanel;
    public TextMeshProUGUI resultsText;

    // 🏆 TOP PLAYERS
    public void ShowTopPlayers()
    {
        StartCoroutine(TopPlayersRoutine());
    }

    IEnumerator TopPlayersRoutine()
    {
        yield return StartCoroutine(api.GetTopPlayers());

        PlayerStat[] players = JsonHelper.FromJson<PlayerStat>(api.topPlayersJSON);

        resultsPanel.SetActive(true);

        resultsText.text = "TOP PLAYERS\n\n";

        for (int i = 0; i < players.Length; i++)
        {
            resultsText.text +=
                (i + 1) + ". " + players[i].Username +
                " - " + players[i].Wins + " Wins\n";
        }
    }

    // 📜 MATCH HISTORY
    public void ShowMatchHistory()
    {
        StartCoroutine(MatchHistoryRoutine());
    }

    IEnumerator MatchHistoryRoutine()
    {
        yield return StartCoroutine(api.GetMatchHistory());

        MatchDataUI[] matches = JsonHelper.FromJson<MatchDataUI>(api.matchHistoryJSON);

        resultsPanel.SetActive(true);
        resultsText.text = "MATCH HISTORY\n\n";

        foreach (var m in matches)
        {
            resultsText.text +=
                m.Player1 + " vs " + m.Player2 +
                " → Winner: " + m.Winner + "\n";
        }
    }
    IEnumerator MapRoutine()
    {
        yield return StartCoroutine(api.GetMostPlayedMap());

        MapData[] maps = JsonHelper.FromJson<MapData>(api.mostPlayedMapJSON);

        resultsPanel.SetActive(true);

        if (maps.Length > 0)
        {
                resultsText.text =
        "MOST PLAYED MAP\n\n" +
        maps[0].MapName + " (" + maps[0].TimesPlayed + " matches)";
        }
        else
        {
            resultsText.text = "MOST PLAYED MAP\n\nNo data";
        }
    }

    // 🗺️ MOST PLAYED MAP
    public void ShowMostPlayedMap()
    {
        StartCoroutine(MapRoutine());
    }

    // 📊 WIN RATE
    public void ShowWinRate()
    {
        StartCoroutine(WinRateRoutine());
    }

    IEnumerator WinRateRoutine()
    {
        yield return StartCoroutine(api.GetWinRate());

        WinRateData[] data = JsonHelper.FromJson<WinRateData>(api.winRateJSON);

        resultsPanel.SetActive(true);
        resultsText.text = "WIN RATE\n\n";

        if (data.Length == 0)
        {
            resultsText.text += "No data";
            yield break;
        }

        foreach (var player in data)
        {
            resultsText.text +=
                player.Username + " - " + player.WinRate.ToString("0.0") + "%\n";
        }
    }
    // 🔁 SCENE NAVIGATION
    public void goos()
    {
        SceneManager.LoadScene("StatsScene");
    }

    public void gops()
    {
        SceneManager.LoadScene("StatLogin");
    }

    public void gobak()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void gobak2()
    {
        SceneManager.LoadScene("PreStat");
    }
    public void hidebak()
    {
        resultsPanel.SetActive(false);
    }
}