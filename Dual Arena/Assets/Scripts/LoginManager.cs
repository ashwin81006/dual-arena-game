using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour
{
    public UserManager userManager;

    // PLAYER 1
    public TMP_Dropdown p1Dropdown;
    public TMP_InputField p1Password;
    public TextMeshProUGUI p1Status;

    // PLAYER 2
    public TMP_Dropdown p2Dropdown;
    public TMP_InputField p2Password;
    public TextMeshProUGUI p2Status;

    // REGISTER
    public TMP_InputField regUsername;
    public TMP_InputField regPassword;
    public TextMeshProUGUI registerStatus;
    public GameObject registerPanel;

    bool p1Logged = false;
    bool p2Logged = false;

    void Start()
    {
        PopulateDropdowns();
    }

    void PopulateDropdowns()
    {
        p1Dropdown.ClearOptions();
        p2Dropdown.ClearOptions();

        List<string> names = new List<string>();

        foreach (var user in userManager.users)
        {
            names.Add(user.username);
        }

        p1Dropdown.AddOptions(names);
        p2Dropdown.AddOptions(names);
    }

    // 🔐 PLAYER 1 LOGIN
    public void LoginP1()
    {
        if (p1Password.text == "")
        {
            p1Status.text = "Enter password!";
            p1Status.color = Color.red;
            return;
        }

        string username = p1Dropdown.options[p1Dropdown.value].text;

        if (userManager.Login(username, p1Password.text))
        {
            p1Status.text = "Logged in";
            p1Status.color = Color.green;

            GameData.player1Name = username;
            p1Logged = true;

            p1Password.text = "";
        }
        else
        {
            p1Status.text = "Wrong password";
            p1Status.color = Color.red;
        }
    }

    // 🔐 PLAYER 2 LOGIN
    public void LoginP2()
    {
        if (p2Password.text == "")
        {
            p2Status.text = "Enter password!";
            p2Status.color = Color.red;
            return;
        }

        string username = p2Dropdown.options[p2Dropdown.value].text;

        if (userManager.Login(username, p2Password.text))
        {
            p2Status.text = "Logged in";
            p2Status.color = Color.green;

            GameData.player2Name = username;
            p2Logged = true;

            p2Password.text = "";
        }
        else
        {
            p2Status.text = "Wrong password";
            p2Status.color = Color.red;
        }
    }

    // ▶ START GAME
    public void StartGame()
    {
        if (!p1Logged || !p2Logged)
        {
            if (!p1Logged)
            {
                p1Status.text = "Login required";
                p1Status.color = Color.red;
            }

            if (!p2Logged)
            {
                p2Status.text = "Login required";
                p2Status.color = Color.red;
            }

            return;
        }

        if (GameData.player1Name == GameData.player2Name)
        {
            p1Status.text = "Same user!";
            p2Status.text = "Same user!";

            p1Status.color = Color.red;
            p2Status.color = Color.red;

            return;
        }

        SceneManager.LoadScene("DualScene");
    }

    // 🆕 REGISTER
    public void RegisterUser()
    {
        if (regUsername.text == "" || regPassword.text == "")
        {
            registerStatus.text = "Fill all fields!";
            registerStatus.color = Color.red;
            return;
        }

        bool success = userManager.Register(regUsername.text, regPassword.text);

        if (success)
        {
            registerStatus.text = "Registered!";
            registerStatus.color = Color.green;

            PopulateDropdowns();

            regUsername.text = "";
            regPassword.text = "";
        }
        else
        {
            registerStatus.text = "Username exists!";
            registerStatus.color = Color.red;
        }
    }

    // 🪟 OPEN/CLOSE REGISTER
    public void OpenRegister()
    {
        registerPanel.SetActive(true);
    }

    public void CloseRegister()
    {
        registerPanel.SetActive(false);
    }
    
    // 🔙 BACK
    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}