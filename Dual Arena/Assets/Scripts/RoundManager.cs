using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class RoundManager : MonoBehaviour
{
    public MoveSelectionManager moveManager;
    public Slider player1HPBar;
    public Slider player2HPBar;

    public TextMeshProUGUI player1HPText;
    public TextMeshProUGUI player2HPText;
    public int player1HP = 100;
    public int player2HP = 100;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI roundText;

    int roundNumber = 1;
    void UpdateHPUI()
    {
        player1HPBar.value = player1HP;
        player2HPBar.value = player2HP;

        player1HPText.text = "" + player1HP;
        player2HPText.text = "" + player2HP;
    }
    public void StartSimulation()
    {
        StartCoroutine(SimulateRounds());
    }
    string MoveName(int move)
    {
        if (move == 0) return "Light";
        if (move == 1) return "Heavy";
        if (move == 2) return "Block";
        return "Unknown";
    }
    IEnumerator SimulateRounds()
    {
        List<int> p1Moves = moveManager.player1Moves;
        List<int> p2Moves = moveManager.player2Moves;
        roundText.text = "ROUND " + roundNumber;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Round " + (i + 1) +
            " | P1: " + MoveName(p1Moves[i]) +
            " vs P2: " + MoveName(p2Moves[i]));
            

            yield return StartCoroutine(ResolveRound(p1Moves[i], p2Moves[i]));

            if (player1HP <= 0 || player2HP <= 0)
            {
                Debug.Log("Fight ended early!");
                break;
            }

            yield return new WaitForSeconds(3f);
        }
        if (player1HP <= 0)
        {
            Debug.Log("PLAYER 2 WINS!");
            statusText.text = "PLAYER 2 WINS!";
            moveManager.simulationRunning = true; // lock game
            yield break;
        }
        else if (player2HP <= 0)
        {
            Debug.Log("PLAYER 1 WINS!");
            statusText.text = "PLAYER 1 WINS!";
            moveManager.simulationRunning = true; // lock game
            yield break;
        }

        // ONLY runs if no one died
        Debug.Log("Simulation Finished");

        moveManager.player1Moves.Clear();
        moveManager.player2Moves.Clear();

        moveManager.simulationRunning = false;

        moveManager.ResetTurn();
        roundNumber++;
    }

    IEnumerator ResolveRound(int p1Move, int p2Move)
    {
        // Step 1: Show moves
        statusText.text =
            "P1 uses " + MoveName(p1Move);

        yield return new WaitForSeconds(1f);

        statusText.text +=
            "\nP2 uses " + MoveName(p2Move);

        yield return new WaitForSeconds(1f);

        // Step 2: Resolve logic
        float roll = Random.Range(0f, 1f);

        if (p1Move == 2 && p2Move == 2)
        {
            statusText.text += "\nBoth blocked!";
            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        if (p1Move == p2Move)
        {
            if (Random.value < 0.5f)
                DamagePlayer2(Random.Range(12, 18));
            else
                DamagePlayer1(Random.Range(12, 18));

            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        // LIGHT vs HEAVY
        if (p1Move == 0 && p2Move == 1)
        {
            if (roll < 0.6f)
                DamagePlayer2(Random.Range(12, 18));
            else
                DamagePlayer1(Random.Range(25, 35));
        }

        // HEAVY vs LIGHT
        else if (p1Move == 1 && p2Move == 0)
        {
            if (roll < 0.6f)
                DamagePlayer1(Random.Range(12, 18));
            else
                DamagePlayer2(Random.Range(25, 35));
        }

        // BLOCK vs HEAVY
        else if (p1Move == 2 && p2Move == 1)
        {
            int dmg = Mathf.RoundToInt(Random.Range(25, 35) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer1(dmg);
        }

        else if (p1Move == 1 && p2Move == 2)
        {
            int dmg = Mathf.RoundToInt(Random.Range(25, 35) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer2(dmg);
        }

        // LIGHT vs BLOCK
        else if (p1Move == 0 && p2Move == 2)
        {
            int dmg = Mathf.RoundToInt(Random.Range(12, 18) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer2(dmg);
        }

        else if (p1Move == 2 && p2Move == 0)
        {
            int dmg = Mathf.RoundToInt(Random.Range(12, 18) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer1(dmg);
        }

        yield return new WaitForSeconds(1.5f);
    }

    void DamagePlayer1(int dmg)
    {
        player1HP -= dmg;

        if (player1HP < 0)
            player1HP = 0;

        Debug.Log("Player1 took " + dmg + " damage.");
        statusText.text += "\nPlayer1 loses " + dmg + " HP!";

        UpdateHPUI();
    }
    void DamagePlayer2(int dmg)
    {
        player2HP -= dmg;

        if (player2HP < 0)
            player2HP = 0;

        Debug.Log("Player2 took " + dmg + " damage.");
        statusText.text += "\nPlayer2 loses " + dmg + " HP!";

        UpdateHPUI();
    }
}