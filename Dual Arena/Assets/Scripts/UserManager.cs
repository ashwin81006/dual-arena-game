using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public List<User> users = new List<User>();

    string saveKey = "USER_DATA";

    void Awake()
    {
        LoadUsers();
    }

    public void SaveUsers()
    {
        string json = JsonUtility.ToJson(new UserListWrapper { users = users });
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadUsers()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            users = JsonUtility.FromJson<UserListWrapper>(json).users;
        }
    }

    public bool Register(string username, string password)
    {
        // check duplicate
        foreach (var user in users)
        {
            if (user.username == username)
                return false;
        }

        users.Add(new User { username = username, password = password });
        SaveUsers();
        return true;
    }

    public bool Login(string username, string password)
    {
        foreach (var user in users)
        {
            if (user.username == username && user.password == password)
                return true;
        }
        return false;
    }
}