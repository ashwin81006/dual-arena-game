
using System.Collections.Generic;
public static class GameData
{
    public static string player1Name;
    public static string player2Name;
    public static int selectedMap = 0;
    public static string currentUser;
    public static int tournamentPlayerCount;
    public static string lastWinner;
    public static bool isTournamentMode = false;
    public static List<string> tournamentPlayers = new List<string>();
    public static List<MatchData> tournamentMatches = new List<MatchData>();
    public static bool winnerProcessed = false;
    public static int tournamentMatchIndex = 0;
    public static string tournamentWinner;
    public static bool tournamentFinished = false;
}