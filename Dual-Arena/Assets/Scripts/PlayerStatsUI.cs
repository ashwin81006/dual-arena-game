using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

#region DATA CLASSES

[System.Serializable]
public class PlayerStats
{
    public string Username;
    public int TotalMatches;
    public int Wins;
    public int Losses;
    public float WinRate;
}

[System.Serializable]
public class PlayerStatsWrapper
{
    public PlayerStats[] array;
}

[System.Serializable]
public class MoveData
{
    public string MoveType;
    public int UsageCount;
}

[System.Serializable]
public class MoveWrapper
{
    public MoveData[] array;
}

#endregion

public class PlayerStatsUI : MonoBehaviour
{
    public APIManager api;

    public TMP_Dropdown usernameDropdown;
    public TMP_InputField passwordInput;

    public GameObject statsPanel;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI statusText;

    void Start()
    {
        statsPanel.SetActive(false);
        StartCoroutine(LoadUsernames());
    }
    public void bak()
    {
        statsPanel.SetActive(false);
        passwordInput.text = "";
    }

    public void bak1()
    {
        SceneManager.LoadScene("PreStat");
    }
    IEnumerator LoadUsernames()
    {
        yield return StartCoroutine(api.GetUsers());

        usernameDropdown.ClearOptions();
        usernameDropdown.AddOptions(api.fetchedUsers);
    }

    public void LoginAndShowStats()
    {
        StartCoroutine(LoginThenFetch());
    }

    IEnumerator LoginThenFetch()
    {
        string username = usernameDropdown.options[usernameDropdown.value].text;
        string password = passwordInput.text;

        if (password == "")
        {
            statusText.text = "Enter password!";
            yield break;
        }

        // 🔐 LOGIN
        yield return StartCoroutine(api.Login(username, password));

        if (!api.lastLoginSuccess)
        {
            statusText.text = "Wrong password";
            yield break;
        }

        statusText.text = "Login Success";

        // 📊 FETCH DATA
        yield return StartCoroutine(api.GetPlayerStats(username));
        yield return StartCoroutine(api.GetMostUsedMove(username));
        yield return StartCoroutine(api.GetWinRate());

        Debug.Log("WINRATE JSON: " + api.winRateJSON);
        // 📦 PARSE WIN RATE
        float actualWinRate = 0f;

        if (!string.IsNullOrEmpty(api.winRateJSON) && api.winRateJSON != "[]")
        {
            WinRateData[] winRates = JsonHelper.FromJson<WinRateData>(api.winRateJSON);

            foreach (var w in winRates)
            {
                if (w.Username == username)
                {
                    actualWinRate = w.WinRate;
                    break;
                }
            }
        }

        // 📦 PARSE PLAYER STATS
        string json = api.lastPlayerStatsJSON;

        Debug.Log("RAW JSON: " + json);

        if (string.IsNullOrEmpty(json) || json == "[]")
        {
            statusText.text = "No stats found!";
            yield break;
        }

        PlayerStatsWrapper wrapper = JsonUtility.FromJson<PlayerStatsWrapper>(
            "{\"array\":" + json + "}"
        );

        if (wrapper.array == null || wrapper.array.Length == 0)
        {
            statusText.text = "No stats available!";
            yield break;
        }

        PlayerStats p = wrapper.array[0];

        // 📦 PARSE MOVE DATA
        string moveJson = api.lastMoveJSON;
        Debug.Log("MOVE JSON: " + moveJson);

        string moveText = "N/A";

        if (!string.IsNullOrEmpty(moveJson) && moveJson != "[]")
        {
            MoveWrapper moveWrapper = JsonUtility.FromJson<MoveWrapper>(
                "{\"array\":" + moveJson + "}"
            );

            if (moveWrapper.array != null && moveWrapper.array.Length > 0)
            {
                moveText = moveWrapper.array[0].MoveType +
                           " (" + moveWrapper.array[0].UsageCount + ")";
            }
        }

        // 🏆 RANK LOGIC
        string rank = "Beginner";
        if (actualWinRate >= 70) rank = "Pro Player";
        else if (actualWinRate >= 50) rank = "Intermediate";

        // 🎨 COLOR CODING
        if (actualWinRate >= 70)
            statsText.color = Color.green;
        else if (actualWinRate >= 50)
            statsText.color = Color.yellow;
        else
            statsText.color = Color.red;

        // 🖥️ FINAL DISPLAY
        statsText.text =
            "PLAYER PROFILE\n\n" +
            "Username   : " + p.Username + "\n" +
            "Matches    : " + p.TotalMatches + "\n" +
            "Wins       : " + p.Wins + "\n" +
            "Losses     : " + p.Losses + "\n" +
            "Win Rate   : " + actualWinRate.ToString("F1") + "%\n" +
            "Most Move  : " + moveText + "\n\n" +
            "Rank       : " + rank;

        statsPanel.SetActive(true);
    }
}