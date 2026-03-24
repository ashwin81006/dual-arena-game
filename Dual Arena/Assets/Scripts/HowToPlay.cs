using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI rulesText;
    public GameObject b1;
    public GameObject b2;
    public GameObject b3;

    public void OpenPanel()
    {
        panel.SetActive(true);
        rulesText.text = "";
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void Show1v1Rules()
    {
        b1.SetActive(false);
        b2.SetActive(false);
        b3.SetActive(false);

        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);

        leftText.text =
        "1v1 BATTLE\n\n" +

        "GAME FLOW:\n" +
        "- Two players fight\n" +
        "- Each selects 3 moves\n" +
        "- Fight runs automatically\n\n" +

        "MOVES:\n" +
        "- Light: fast, low damage\n" +
        "- Heavy: slow, high damage\n" +
        "- Block: reduces damage";

        rightText.text =
        "MECHANICS:\n" +
        "- Light beats Heavy often\n" +
        "- Heavy can deal big damage\n" +
        "- Block reduces incoming damage\n" +
        "- Same moves = random hit\n\n" +

        "WIN CONDITION:\n" +
        "- Reduce opponent HP to 0\n\n" +

        "TIP:\n" +
        "- Mix moves smartly\n" +
        "- Predict opponent";
    }

    public void ShowTournamentRules()
    {
        b1.SetActive(false);
        b2.SetActive(false);
        b3.SetActive(false);

        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);

        leftText.text =
        "TOURNAMENT MODE\n\n" +

        "FORMAT:\n" +
        "- Multiple players\n" +
        "- 1v1 matches\n\n" +

        "PROGRESSION:\n" +
        "- Winners advance\n" +
        "- Losers eliminated";

        rightText.text =
        "FINAL:\n" +
        "- Last 2 players fight\n" +
        "- Winner becomes champion\n\n" +

        "STRATEGY:\n" +
        "- Adapt each round\n" +
        "- Learn opponent patterns\n\n" +

        "GOAL:\n" +
        "- Win all matches";
    }
}