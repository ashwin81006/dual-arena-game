using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour
{
    public TextMeshProUGUI playerLabel;
    public TMP_Dropdown dropdown;
    public Button loginButton;
    public string selectedUsername;
    int playerIndex;
    bool isLocked = false;

    public PlayerSelectionManager manager;

    public void SetPlayerNumber(int index)
    {
        playerIndex = index;
        playerLabel.text = "Player " + index;
    }

    public PasswordManager passwordManager;

    public void OnLoginClicked()
    {
        if (isLocked) return;
        Debug.Log("LOGIN CLICKED"); // 👈 ADD THIS



        passwordManager.Open(this);
    }



    public void LockPlayer()
    {
        string username = dropdown.options[dropdown.value].text;

        if (manager.selectedPlayers.Contains(username))
        {
            Debug.Log("Player already selected!");
            return;
        }

        isLocked = true;
        selectedUsername = username;

        dropdown.interactable = false;
        loginButton.interactable = false;

        manager.AddPlayer(username);
        manager.OnPlayerLocked();
    }
}