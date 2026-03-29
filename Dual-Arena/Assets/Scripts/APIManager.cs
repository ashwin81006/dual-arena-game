using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class APIManager : MonoBehaviour
{
    public bool lastLoginSuccess;
    public bool lastRegisterSuccess;
    public string lastPlayerStatsJSON;
    string baseURL = "http://192.168.56.1:3000";

    public string lastMoveJSON;
    public string topPlayersJSON;
    public string matchHistoryJSON;
    public string mostPlayedMapJSON;
    public string winRateJSON;



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