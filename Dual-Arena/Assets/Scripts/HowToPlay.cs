using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rrule;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI rulesText;
    public GameObject BackBtn;
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
        BackBtn.SetActive(true);

        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);
        rrule.text = "Rules (1v1 Battle)";
        leftText.text =
"GAME FLOW:\n" +
"- Two players fight each other\n" +
"- Each player selects 3 moves in advance\n" +
"- Hide your screen from the opponent while choosing your moves\n" +
"- Moves play automatically in order\n" +
 "- Reduce opponent HP to 0 to win\n\n" +

"MOVES:\n" +
"- Light Attack:\n" +
"  Fast attack, low damage\n\n" +
"- Heavy Attack:\n" +
"  Slower attack, high damage\n\n" +
"- Block:\n" +
"  Defensive move that can stop attacks\n";

        rightText.text =
        "MOVE RESULTS:\n\n" +
        "- Light vs Heavy:\n" +
        "  Heavy attacker beats Light attacker, light attacker takes significant damage\n\n" +

        "- Heavy vs Block:\n" +
        "  Block stops the attack and reflects damage back\n" +
        "  The Heavy attacker takes damage\n\n" +

        "- Light vs Block:\n" +
        "  Light attack breaks through Block. The player who blocked takes slight damage\n\n" +

        "- Same move by both player = NO HP Loss:\n";

    }

    public void ShowTournamentRules()
    {
        b1.SetActive(false);
        b2.SetActive(false);
        b3.SetActive(false);
        BackBtn.SetActive(true);

        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);

        rrule.text = "Rules (Tournament)";

        leftText.text =
        "TOURNAMENT STRUCTURE\n\n" +

        "- Players compete in a knockout bracket\n" +
        "- Supported sizes: 4, 8, or 16 players\n" +
        "- Each match is a 1v1 battle\n" +
        "- Players are eliminated after losing a match\n\n" +

        "MATCH FLOW\n" +
        "- Each player selects 3 moves per match\n" +
        "- Moves are executed round by round\n" +
        "- Each round compares both players' moves\n" +
        "- Damage is applied based on move interactions\n";

        rightText.text =
        "MOVE INTERACTIONS\n\n" +

        "- Light vs Heavy: Heavy wins and deals high damage\n" +
        "- Heavy vs Block: Block reflects damage back to attacker\n" +
        "- Light vs Block: Light still deals damage\n" +
        "- Same moves: No damage (draw round)\n\n" +

        "WIN CONDITION\n" +
        "- Reduce opponent HP to 0 to win the match\n" +
        "- Winner advances to next round\n" +
        "- Final match determines the champion\n";
    }

    public void BackGo()
    {
        rrule.text = "Rules";
        b1.SetActive(true);
        b2.SetActive(true);
        b3.SetActive(true);
        BackBtn.SetActive(false);

        leftText.gameObject.SetActive(false);
        rightText.gameObject.SetActive(false);

    }
}