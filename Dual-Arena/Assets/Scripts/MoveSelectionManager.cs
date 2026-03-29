using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class MoveSelectionManager : MonoBehaviour
{
    public List<int> player1Moves = new List<int>();
    public List<int> player2Moves = new List<int>();

    public TextMeshProUGUI statusText;

    public bool simulationRunning = false;
    int currentPlayer = 1;

    public void SelectLight() => RegisterMove(0);
    public void SelectHeavy() => RegisterMove(1);
    public void SelectBlock() => RegisterMove(2);

    string MoveName(int move)
    {
        if (move == 0) return "Light";
        if (move == 1) return "Heavy";
        if (move == 2) return "Block";
        return "?";
    }
    void Start()
    {
        ResetTurn();
    }
    void RegisterMove(int move)
    {
        if (simulationRunning)
            return;

        string p1 = GameData.player1Name;
        string p2 = GameData.player2Name;

        if (currentPlayer == 1)
        {
            player1Moves.Add(move);

            statusText.text = p1 + " selecting... (" + player1Moves.Count + "/3)";

            if (player1Moves.Count == 3)
            {
                currentPlayer = 2;
                statusText.text = p2 + " - Choose Moves";
            }
        }
        else
        {
            player2Moves.Add(move);

            statusText.text = p2 + " selecting... (" + player2Moves.Count + "/3)";

            if (player2Moves.Count == 3)
            {
                simulationRunning = true;

                FindFirstObjectByType<RoundManager>().StartSimulation();
            }
        }
    }

    public void ResetTurn()
    {
        currentPlayer = 1;

        player1Moves.Clear();
        player2Moves.Clear();

        statusText.text = GameData.player1Name + " - Choose Moves";
    }
}