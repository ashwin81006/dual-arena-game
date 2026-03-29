using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour
{
    public UserManager userManager;
    public APIManager api;

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

    public void PopulateDropdowns()
    {
        StartCoroutine(PopulateFromAPI());
    }

    IEnumerator PopulateFromAPI()
    {
        yield return StartCoroutine(api.GetUsers());

        p1Dropdown.ClearOptions();
        p2Dropdown.ClearOptions();

        p1Dropdown.AddOptions(api.fetchedUsers);
        p2Dropdown.AddOptions(api.fetchedUsers);
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

        if (p2Logged && GameData.player2Name == username)
        {
            p1Status.text = "Already used by Player 2!";
            p1Status.color = Color.red;
            return;
        }

        StartCoroutine(LoginP1_API(username, p1Password.text));
    }

    IEnumerator LoginP1_API(string username, string password)
    {
        yield return StartCoroutine(api.Login(username, password));

        if (api.lastLoginSuccess)
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

        if (p1Logged && GameData.player1Name == username)
        {
            p2Status.text = "Already used by Player 1!";
            p2Status.color = Color.red;
            return;
        }

        StartCoroutine(LoginP2_API(username, p2Password.text));
    }

    IEnumerator LoginP2_API(string username, string password)
    {
        yield return StartCoroutine(api.Login(username, password));

        if (api.lastLoginSuccess)
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
    public void GoToMapSelection()
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
            return;
        }
        SceneManager.LoadScene("MapSelection");
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

        StartCoroutine(Register_API(regUsername.text, regPassword.text));
    }

    IEnumerator Register_API(string username, string password)
    {
        yield return StartCoroutine(api.Register(username, password));

        if (api.lastRegisterSuccess)
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

    // 🪟 REGISTER PANEL
    public void OpenRegister()
    {
        registerPanel.SetActive(true);
    }

    public void CloseRegister()
    {
        registerPanel.SetActive(false);
    }

    public void OnP1DropdownChanged()
    {
        p1Logged = false;
        GameData.player1Name = "";

        p1Status.text = "Not Logged In";
        p1Status.color = Color.white;
    }

    public void OnP2DropdownChanged()
    {
        p2Logged = false;
        GameData.player2Name = "";

        p2Status.text = "Not Logged In";
        p2Status.color = Color.white;
    }

    // 🔙 BACK
    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}