using UnityEngine;
using TMPro;

public class RegisterManager : MonoBehaviour
{
    public GameObject panel;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI feedbackText;

    public void Open()
    {
        panel.SetActive(true);
        feedbackText.text = "";
        usernameInput.text = "";
        passwordInput.text = "";
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    public void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Enter username & password";
            return;
        }

        // 🔥 TEMP LOGIC (later DB)
        Debug.Log("Registered: " + username);

        feedbackText.text = "Registered successfully!";

        // Optional: auto close
        Invoke(nameof(Close), 1.5f);
    }
}