using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class MoveDataSend
{
    public string player;
    public string move;
    public int damage;
}

[System.Serializable]
public class RoundDataSend
{
    public int roundNumber;
    public List<MoveDataSend> moves;
}

public class RoundManager : MonoBehaviour
{
    public MoveSelectionManager moveManager;
    public SpriteRenderer backgroundRenderer;
    public Sprite[] maps;
    public APIManager api;

    List<RoundDataSend> allRounds = new List<RoundDataSend>();

    public GameObject p1Shield;
    public GameObject p2Shield;

    public Transform p1Transform;
    public Transform p2Transform;
    Vector3 p1StartPos;
    Vector3 p2StartPos;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI statsText;
    public Slider player1HPBar;
    public Slider player2HPBar;

    public GameObject statusPanel;

    public GameObject playAgainButton;
    public GameObject nextMatchButton;
    public GameObject MapSelectButton;
    public GameObject mainMenuButton;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI p1MoveText;
    public TextMeshProUGUI p2MoveText;

    public TextMeshProUGUI player1HPText;
    public TextMeshProUGUI player2HPText;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI roundText;

    public Animator p1Anim;
    public Animator p2Anim;

    public int player1HP = 100;
    public int player2HP = 100;
    public GameObject[] uiButtons;
    int lastP1Damage = 0;
    int lastP2Damage = 0;

    int roundNumber = 1;

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToMap()
    {
        SceneManager.LoadScene("MapSelection");
    }
    void Start()
    {
        player1NameText.text = GameData.player1Name;
        player2NameText.text = GameData.player2Name;

        backgroundRenderer.sprite = maps[GameData.selectedMap];
        backgroundRenderer.transform.localScale = new Vector3(1.21f, 1.21f, 1f);

        p1StartPos = p1Transform.position;
        p2StartPos = p2Transform.position;

        p1Shield.SetActive(false);
        p2Shield.SetActive(false);

        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        player1HPBar.value = player1HP;
        player2HPBar.value = player2HP;

        player1HPText.text = player1HP.ToString();
        player2HPText.text = player2HP.ToString();
    }

    public void StartSimulation()
    {
        SetButtons(false);
        StartCoroutine(SimulateRounds());
    }

    string MoveName(int move)
    {
        if (move == 0) return "Light";
        if (move == 1) return "Heavy";
        if (move == 2) return "Block";
        return "Unknown";
    }

    void ShowResult(string winnerName)
    {
        StartCoroutine(api.SaveMatchWithRounds(
            GameData.player1Name,
            GameData.player2Name,
            winnerName,
            GameData.selectedMap,
            allRounds
        ));
        resultPanel.SetActive(true);

        resultText.text = winnerName + " WINS!";

        statsText.text =
            GameData.player1Name + ": " + player1HP + "\n" +
            GameData.player2Name + ": " + player2HP;
        GameData.lastWinner = winnerName;
        GameData.winnerProcessed = false; // 🔥 reset flag
        // 🔥 TOURNAMENT MODE LOGIC
        if (GameData.isTournamentMode)
        {
            GameData.lastWinner = winnerName;

            playAgainButton.SetActive(false);
            mainMenuButton.SetActive(false);
            nextMatchButton.SetActive(true);
            MapSelectButton.SetActive(false);
        }
        else
        {
            playAgainButton.SetActive(true);
            mainMenuButton.SetActive(true);
            MapSelectButton.SetActive(true);
            nextMatchButton.SetActive(false);
        }
    }
    public void GoToNextMatch()
    {
        SceneManager.LoadScene("TournamentMatchScene");
    }
    IEnumerator SimulateRounds()
    {
        List<int> p1Moves = moveManager.player1Moves;
        List<int> p2Moves = moveManager.player2Moves;

        roundText.text = "ROUND " + roundNumber;

        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(ResolveRound(p1Moves[i], p2Moves[i]));

            if (player1HP <= 0 || player2HP <= 0)
                break;

            yield return new WaitForSeconds(2f);
        }

        if (player1HP <= 0)
        {
            ShowResult(GameData.player2Name);
            p1Anim.SetTrigger("Death");
            yield break;
        }
        else if (player2HP <= 0)
        {
            ShowResult(GameData.player1Name);
            p2Anim.SetTrigger("Death");
            yield break;
        }

        moveManager.player1Moves.Clear();
        moveManager.player2Moves.Clear();

        moveManager.simulationRunning = false;
        moveManager.ResetTurn();
        SetButtons(true);
        roundNumber++;
    }
    void SetButtons(bool state)
    {
        foreach (GameObject btn in uiButtons)
        {
            btn.SetActive(state);
        }
    }
    int GetWinner(int p1, int p2)
    {
        if (p1 == p2) return 0;

        if (p1 == 0 && p2 == 2) return 1;
        if (p2 == 0 && p1 == 2) return 2;

        if (p1 == 2 && p2 == 1) return 1;
        if (p2 == 2 && p1 == 1) return 2;

        if (p1 == 1 && p2 == 0) return 1;
        if (p2 == 1 && p1 == 0) return 2;

        return 0;
    }

    IEnumerator ResolveRound(int p1Move, int p2Move)
    {
        p1MoveText.text = GameData.player1Name + ": " + MoveName(p1Move);
        yield return new WaitForSeconds(1f);
        p2MoveText.text = GameData.player2Name + ": " + MoveName(p2Move);

        statusText.text = ""; // clear bottom

        yield return new WaitForSeconds(2f);

        int winner = GetWinner(p1Move, p2Move);

        PlayAnimations(p1Move, p2Move, winner);

        yield return new WaitForSeconds(1.2f);

        if (winner == 0)
        {
            statusText.text += "\nDraw!";
            yield return new WaitForSeconds(1.5f);
            yield break;
        }
        ShowResultMessage(winner, p1Move, p2Move);
        ApplyDamage(winner, p1Move, p2Move);

        yield return new WaitForSeconds(1.5f);
        // 🔥 SAVE ROUND DATA
        RoundDataSend round = new RoundDataSend();
        round.roundNumber = roundNumber;
        round.moves = new List<MoveDataSend>();

        // Player 1
        round.moves.Add(new MoveDataSend
        {
            player = GameData.player1Name,
            move = MoveName(p1Move),
            damage = lastP1Damage
        });

        // Player 2
        round.moves.Add(new MoveDataSend
        {
            player = GameData.player2Name,
            move = MoveName(p2Move),
            damage = lastP2Damage
        });

        allRounds.Add(round);
        p1MoveText.text = "";
        p2MoveText.text = "";
    }

    void ShowResultMessage(int winner, int p1Move, int p2Move)
    {
        string p1 = GameData.player1Name;
        string p2 = GameData.player2Name;

        // Draw
        if (winner == 0)
        {
            statusText.text = "Same moves!\nNo HP loss!";
            return;
        }

        // Player1 wins
        if (winner == 1)
        {
            if (p1Move == 0) // Light
                statusText.text = p1 + " lands a quick strike on " + p2;
            else if (p1Move == 1) // Heavy
                statusText.text = p1 + " delivers a powerful blow on " + p2;
            else if (p1Move == 2) // Block
                statusText.text = p1 + " perfectly defends!";
        }
        // Player2 wins
        else if (winner == 2)
        {
            if (p2Move == 0)
                statusText.text = p2 + " lands a quick strike on " + p1;
            else if (p2Move == 1)
                statusText.text = p2 + " delivers a powerful blow on " + p1;
            else if (p2Move == 2)
                statusText.text = p2 + " perfectly defends!";
        }
    }

    IEnumerator MoveAttackReturn(Transform player, Animator anim, int move, Vector3 startPos, float dir, float distance, Animator targetToHit)
    {
        Vector3 target = startPos + new Vector3(distance * dir, 0, 0);

        float t = 0;

        // MOVE
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            player.position = Vector3.Lerp(startPos, target, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        // ATTACK
        TriggerMove(anim, move);

        // HIT SYNC
        if (targetToHit != null)
        {
            yield return new WaitForSeconds(0.2f);
            targetToHit.SetTrigger("Hit");
        }

        yield return new WaitForSeconds(0.8f);

        // RETURN
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;
            player.position = Vector3.Lerp(target, startPos, t);
            yield return null;
        }
    }

    void PlayAnimations(int p1Move, int p2Move, int winner)
    {
        // SHOW SHIELDS
        // SHOW BLOCK ONLY WHEN VALID

        // block vs block
        if (p1Move == 2 && p2Move == 2)
        {
            p1Shield.SetActive(true);
            p2Shield.SetActive(true);

            Invoke(nameof(HideP1Shield), 1.2f);
            Invoke(nameof(HideP2Shield), 1.2f);
        }

        // heavy vs block (block works)
        else if (p1Move == 1 && p2Move == 2)
        {
            p2Shield.SetActive(true);
            Invoke(nameof(HideP2Shield), 1.2f);
        }
        else if (p2Move == 1 && p1Move == 2)
        {
            p1Shield.SetActive(true);
            Invoke(nameof(HideP1Shield), 1.2f);
        }

        // ❌ DO NOT SHOW BLOCK for light vs block
        // BLOCK vs BLOCK
        if (p1Move == 2 && p2Move == 2)
        {
            p1Anim.SetTrigger("Block");
            p2Anim.SetTrigger("Block");
            return;
        }

        // HEAVY vs BLOCK
        if (p1Move == 1 && p2Move == 2)
        {
            StartCoroutine(MoveAttackReturn(p1Transform, p1Anim, 1, p1StartPos, 1, 6.5f, null));
            p2Anim.SetTrigger("Block");
            return;
        }

        if (p2Move == 1 && p1Move == 2)
        {
            StartCoroutine(MoveAttackReturn(p2Transform, p2Anim, 1, p2StartPos, -1, 6.5f, null));
            p1Anim.SetTrigger("Block");
            return;
        }

        // LIGHT vs HEAVY → ONLY HEAVY
        if (p1Move == 0 && p2Move == 1)
        {
            StartCoroutine(MoveAttackReturn(p2Transform, p2Anim, 1, p2StartPos, -1, 6.5f, p1Anim));
            return;
        }

        if (p2Move == 0 && p1Move == 1)
        {
            StartCoroutine(MoveAttackReturn(p1Transform, p1Anim, 1, p1StartPos, 1, 6.5f, p2Anim));
            return;
        }

        // SAME MOVE
        if (p1Move == p2Move)
        {
            StartCoroutine(MoveAttackReturn(p1Transform, p1Anim, p1Move, p1StartPos, 1, 3.5f, null));
            StartCoroutine(MoveAttackReturn(p2Transform, p2Anim, p2Move, p2StartPos, -1, 3.5f, null));
            return;
        }

        // DEFAULT → WINNER ONLY
        if (winner == 1)
        {
            StartCoroutine(MoveAttackReturn(p1Transform, p1Anim, p1Move, p1StartPos, 1, 6.5f, p2Anim));
        }
        else if (winner == 2)
        {
            StartCoroutine(MoveAttackReturn(p2Transform, p2Anim, p2Move, p2StartPos, -1, 6.5f, p1Anim));
        }
    }

    void TriggerMove(Animator anim, int move)
    {
        if (move == 0) anim.SetTrigger("Light");
        if (move == 1) anim.SetTrigger("Heavy");
        if (move == 2) anim.SetTrigger("Block");
    }

    void HideP1Shield() => p1Shield.SetActive(false);
    void HideP2Shield() => p2Shield.SetActive(false);

    void ApplyDamage(int winner, int p1Move, int p2Move)
    {
        int lightDamage = Random.Range(8, 15);
        int heavyDamage = Random.Range(20, 30);
        int blockReflect = Random.Range(7, 13);

        // 🔥 HEAVY vs BLOCK (REFLECT DAMAGE)
        // HEAVY vs BLOCK
        if (p1Move == 1 && p2Move == 2)
        {
            lastP1Damage = 0;
            lastP2Damage = blockReflect;

            statusText.text = GameData.player2Name + " deflects the attack! Hard luck for " + GameData.player1Name;
            DamagePlayer1(blockReflect);
            return;
        }

        if (p2Move == 1 && p1Move == 2)
        {
            lastP2Damage = 0;
            lastP1Damage = blockReflect;

            statusText.text = GameData.player1Name + " deflects the attack! Hard luck for " + GameData.player2Name;
            DamagePlayer2(blockReflect);
            return;
        }

        // NORMAL DAMAGE
        lastP1Damage = 0;
        lastP2Damage = 0;

        if (winner == 1)
        {
            if (p1Move == 1)
            { 
                DamagePlayer2(heavyDamage);
            }
            else if (p1Move == 0)
            { 
                DamagePlayer2(lightDamage);
            }
        }
        else if (winner == 2)
        {
            if (p2Move == 1)
            { 
                DamagePlayer1(heavyDamage);
            }
            else if (p2Move == 0)
            { 
                DamagePlayer1(lightDamage);
            }
        }
        Debug.Log("P1 Damage: " + lastP1Damage + " | P2 Damage: " + lastP2Damage);
    }

    void DamagePlayer1(int dmg)
    {
        string p1 = GameData.player1Name;
        int actualDamage = Mathf.Min(dmg, player1HP);
        player1HP -= actualDamage;

        // 🔥 IMPORTANT
        lastP2Damage = actualDamage;

        statusText.text += "\n-" + actualDamage + " HP for " + p1;
        UpdateHPUI();
    }
    void DamagePlayer2(int dmg)
    {
        string p2 = GameData.player2Name;
        int actualDamage = Mathf.Min(dmg, player2HP);
        player2HP -= actualDamage;

        // 🔥 IMPORTANT
        lastP1Damage = actualDamage;

        statusText.text += "\n-" + actualDamage + " HP for " + p2;
        UpdateHPUI();
    }
}