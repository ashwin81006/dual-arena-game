using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class MoveSelectionManager : MonoBehaviour
{
    public List<int> player1Moves = new List<int>();
    public TextMeshProUGUI statusText;
    public List<int> player2Moves = new List<int>();
    public bool simulationRunning = false;
    int currentPlayer = 1;

    public void SelectLight()
    {
        RegisterMove(0);
    }

    public void SelectHeavy()
    {
        RegisterMove(1);
    }

    public void SelectBlock()
    {
        RegisterMove(2);
    }

    string MoveName(int move)
    {
        if (move == 0) return "Light";
        if (move == 1) return "Heavy";
        if (move == 2) return "Block";
        return "?";
    }
    void RegisterMove(int move)
    {
        if (simulationRunning)
            return;

        if (currentPlayer == 1)
        {
            player1Moves.Add(move);

            statusText.text = "Player 1 selecting... (" + player1Moves.Count + "/3)";

            if (player1Moves.Count == 3)
            {
                currentPlayer = 2;
                statusText.text = "Player 2 - Choose Moves";
            }
        }
        else
        {
            player2Moves.Add(move);

            statusText.text = "Player 2 selecting... (" + player2Moves.Count + "/3)";

            if (player2Moves.Count == 3)
            {
                statusText.text = "Simulation Starting...";
                simulationRunning = true;

                FindFirstObjectByType<RoundManager>().StartSimulation();
            }
        }
    }
    public void ResetTurn()
    {
        currentPlayer = 1;
        statusText.text = "Player 1 - Choose Moves";
    }
}