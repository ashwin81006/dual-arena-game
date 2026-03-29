using UnityEngine;
using TMPro;

public class PasswordManager : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField inputField;

    PlayerRow currentRow;

    public void Open(PlayerRow row)
    {
        panel.SetActive(true);
        inputField.text = "";
        currentRow = row;
    }

    public void Confirm()
    {
        string entered = inputField.text;

        // TEMP password logic
        if (entered == "1234")
        {
            currentRow.LockPlayer();
            panel.SetActive(false);
        }
        else
        {
            Debug.Log("Wrong password");
        }
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}