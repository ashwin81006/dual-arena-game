using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Wrapper
{
    public List<RoundDataSend> rounds;
}

public class APIManager : MonoBehaviour
{
    public bool lastLoginSuccess;
    public bool lastRegisterSuccess;
    public string lastPlayerStatsJSON;
    string baseURL = "http://localhost:3000";

    public string lastMoveJSON;
    public string topPlayersJSON;
    public string matchHistoryJSON;
    public string mostPlayedMapJSON;
    public string winRateJSON;

    public int createdTournamentID;
    [System.Serializable]
    public class Tournament
    {
        public int TournamentID;
        public string TournamentName;
    }

    [System.Serializable]
    public class TournamentListWrapper
    {
        public List<Tournament> tournaments;
    }

    public List<Tournament> fetchedTournaments = new List<Tournament>();

    [System.Serializable]
    public class TournamentMatch
    {
        public string Player1;
        public string Player2;
        public string Winner;
        public int RoundNumber;
        public int MatchOrder;
    }

    [System.Serializable]
    public class MatchListWrapper
    {
        public List<TournamentMatch> matches;
    }

    public List<TournamentMatch> fetchedMatches = new List<TournamentMatch>();

    public IEnumerator GetTournamentMatches(int tournamentId)
    {
        UnityWebRequest www = UnityWebRequest.Get(
            baseURL + "/tournament/getMatches/" + tournamentId);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("MATCHES: " + json);

            MatchListWrapper wrapper =
                JsonUtility.FromJson<MatchListWrapper>("{\"matches\":" + json + "}");

            fetchedMatches = wrapper.matches;
        }
        else
        {
            Debug.LogError("❌ Failed to fetch matches: " + www.error);
        }
    }

    public IEnumerator GetTournaments()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "/tournament/getTournaments");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("Tournaments: " + json);

            TournamentListWrapper wrapper =
                JsonUtility.FromJson<TournamentListWrapper>("{\"tournaments\":" + json + "}");

            fetchedTournaments = wrapper.tournaments;
        }
        else
        {
            Debug.LogError("❌ Error fetching tournaments: " + www.error);
        }
    }

    public IEnumerator CreateTournament(string name, int playerCount)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("playerCount", playerCount);

        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/tournament/createTournament", form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("Create Tournament Response: " + json);

            createdTournamentID = int.Parse(json.Split(':')[2].Replace("}", "").Trim());
        }
        else
        {
            Debug.LogError("❌ CreateTournament failed: " + www.error);
        }
    }
    public IEnumerator SaveTournamentMatch(int tournamentId, int matchIndex, string winner)
    {
        WWWForm form = new WWWForm();
        form.AddField("tournamentId", tournamentId);
        form.AddField("matchIndex", matchIndex);
        form.AddField("winner", winner);
        form.AddField("roundNumber", GameData.currentRound);

        using (UnityEngine.Networking.UnityWebRequest www =
            UnityEngine.Networking.UnityWebRequest.Post(baseURL + "/tournament/saveMatch", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ SaveTournamentMatch failed: " + www.error);
            }
            else
            {
                Debug.Log("✅ Tournament match saved");
            }
        }
    }

    public IEnumerator GetMostUsedMove(string username)
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "/stats/mostUsedMove/" + username);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            lastMoveJSON = www.downloadHandler.text;
            Debug.Log("Most Used Move: " + lastMoveJSON);
        }
    }

    public IEnumerator SaveMatchWithRounds(string p1, string p2, string winner, int mapId, List<RoundDataSend> rounds)
    {
        Wrapper w = new Wrapper();
        w.rounds = rounds;

        string json = JsonUtility.ToJson(w);

        WWWForm form = new WWWForm();
        form.AddField("player1", p1);
        form.AddField("player2", p2);
        form.AddField("winner", winner);
        form.AddField("mapId", mapId);
        form.AddField("rounds", json);

        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/match/saveMatch", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Match + Rounds saved!");
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
    public IEnumerator SaveMatch(string p1, string p2, string winner, int mapId)
    {
        WWWForm form = new WWWForm();
        form.AddField("player1", p1);
        form.AddField("player2", p2);
        form.AddField("winner", winner);
        form.AddField("mapId", mapId);

        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/match/saveMatch", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Match saved!");
        }
        else
        {
            Debug.Log("Error saving match: " + www.error);
        }
    }
    public IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/auth/login", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string response = www.downloadHandler.text;
            Debug.Log("Login Response: " + response);

            lastLoginSuccess = response.Contains("true");
        }
        else
        {
            lastLoginSuccess = false;
        }
    }

    public IEnumerator GetPlayerStats(string username)
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "/stats/playerStats/" + username);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            lastPlayerStatsJSON = www.downloadHandler.text;
            Debug.Log("Player Stats: " + lastPlayerStatsJSON);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    public List<string> fetchedUsers = new List<string>();
    public IEnumerator GetTopPlayers()
    {
        UnityEngine.Networking.UnityWebRequest www =
            UnityEngine.Networking.UnityWebRequest.Get(baseURL + "/stats/topPlayers");

        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            topPlayersJSON = www.downloadHandler.text;
            Debug.Log(topPlayersJSON);
        }
    }

    public IEnumerator GetMatchHistory()
    {
        UnityEngine.Networking.UnityWebRequest www =
            UnityEngine.Networking.UnityWebRequest.Get(baseURL + "/stats/matchHistory");

        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            matchHistoryJSON = www.downloadHandler.text;
            Debug.Log(matchHistoryJSON);
        }
    }
    public IEnumerator GetUsers()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "/auth/users");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("Users: " + json);

            // convert JSON → list
            UserListWrapper wrapper = JsonUtility.FromJson<UserListWrapper>("{\"users\":" + json + "}");

            fetchedUsers.Clear();
            foreach (var u in wrapper.users)
            {
                fetchedUsers.Add(u.Username);
            }
        }
        else
        {
            Debug.Log("Error fetching users: " + www.error);
        }
    }

    [System.Serializable]
    public class User
    {
        public string Username;
    }

    [System.Serializable]
    public class UserListWrapper
    {
        public List<User> users;
    }
    public IEnumerator Register(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        Debug.Log("Calling" + baseURL + "/auth/register");
        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/auth/register", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            lastRegisterSuccess = www.downloadHandler.text.Contains("true");
        }
        else
        {
            lastRegisterSuccess = false;
        }
    }


    public IEnumerator GetMostPlayedMap()
    {
        UnityEngine.Networking.UnityWebRequest www =
            UnityEngine.Networking.UnityWebRequest.Get(baseURL + "/stats/mostPlayedMap");

        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            mostPlayedMapJSON = www.downloadHandler.text;
            Debug.Log(mostPlayedMapJSON);
        }
    }

    public IEnumerator GetWinRate()
    {
        UnityEngine.Networking.UnityWebRequest www =
            UnityEngine.Networking.UnityWebRequest.Get(baseURL + "/stats/winRate");

        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            winRateJSON = www.downloadHandler.text;
            Debug.Log(winRateJSON);
        }
    }
}