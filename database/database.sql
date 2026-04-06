DROP DATABASE IF EXISTS DuelArenaDB;
CREATE DATABASE DuelArenaDB;
USE DuelArenaDB;

CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) UNIQUE,
    Password VARCHAR(50),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Maps (
    MapID INT AUTO_INCREMENT PRIMARY KEY,
    MapName VARCHAR(50)
);
 

CREATE TABLE Matches (
    MatchID INT AUTO_INCREMENT PRIMARY KEY,
    Player1ID INT,
    Player2ID INT,
    WinnerID INT,
    MapID INT,
    MatchDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (Player1ID) REFERENCES Users(UserID),
    FOREIGN KEY (Player2ID) REFERENCES Users(UserID),
    FOREIGN KEY (WinnerID) REFERENCES Users(UserID),
    FOREIGN KEY (MapID) REFERENCES Maps(MapID)
);

CREATE TABLE Rounds (
    RoundID INT AUTO_INCREMENT PRIMARY KEY,
    MatchID INT,
    RoundNumber INT,

    FOREIGN KEY (MatchID) REFERENCES Matches(MatchID)
);

CREATE TABLE Moves (
    MoveID INT AUTO_INCREMENT PRIMARY KEY,
    RoundID INT,
    PlayerID INT,
    MoveType VARCHAR(20),
    DamageDealt INT,

    FOREIGN KEY (RoundID) REFERENCES Rounds(RoundID),
    FOREIGN KEY (PlayerID) REFERENCES Users(UserID)
);

CREATE TABLE PlayerStats (
    StatID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    TotalMatches INT DEFAULT 0,
    Wins INT DEFAULT 0,
    Losses INT DEFAULT 0,
    TotalDamageDealt INT DEFAULT 0,
    TotalDamageTaken INT DEFAULT 0,

    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE MapStats (
    MapStatID INT AUTO_INCREMENT PRIMARY KEY,
    MapID INT,
    TotalMatchesPlayed INT DEFAULT 0,
    Player1Wins INT DEFAULT 0,
    Player2Wins INT DEFAULT 0,

    FOREIGN KEY (MapID) REFERENCES Maps(MapID)
);
CREATE TABLE TournamentMatches (
    MatchID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT,
    Player1 VARCHAR(50),
    Player2 VARCHAR(50),
    Winner VARCHAR(50) DEFAULT NULL,
    RoundNumber INT,
    MatchOrder INT
); 

CREATE TABLE Tournaments (
    TournamentID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentName VARCHAR(100),
    TotalPlayers INT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

DELIMITER //
CREATE PROCEDURE GetTopPlayers()
BEGIN
    SELECT u.Username, COUNT(*) AS Wins
    FROM Matches m
    JOIN Users u ON m.WinnerID = u.UserID
    GROUP BY m.WinnerID
    ORDER BY Wins DESC;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE GetMatchHistory()
BEGIN
    SELECT 
        u1.Username AS Player1,
        u2.Username AS Player2,
        uw.Username AS Winner,
        mp.MapName,
        m.MatchDate
    FROM Matches m
    JOIN Users u1 ON m.Player1ID = u1.UserID
    JOIN Users u2 ON m.Player2ID = u2.UserID
    LEFT JOIN Users uw ON m.WinnerID = uw.UserID
    JOIN Maps mp ON m.MapID = mp.MapID
    ORDER BY m.MatchDate DESC;
END //
DELIMITER ;


DELIMITER //
CREATE PROCEDURE GetMostPlayedMap()
BEGIN
    SELECT mp.MapName, COUNT(*) AS TimesPlayed
    FROM Matches m
    JOIN Maps mp ON m.MapID = mp.MapID
    GROUP BY m.MapID
    ORDER BY TimesPlayed DESC;
END //
DELIMITER ;


DELIMITER //
CREATE PROCEDURE GetWinRate()
BEGIN
    SELECT 
        u.Username,

        IFNULL(
            (
                SELECT COUNT(*) 
                FROM Matches m 
                WHERE m.WinnerID = u.UserID
            ) * 100.0
            /
            NULLIF(
                (
                    SELECT COUNT(*) 
                    FROM Matches m 
                    WHERE m.Player1ID = u.UserID OR m.Player2ID = u.UserID
                ), 0
            ),
        0) AS WinRate

    FROM Users u;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE GetPlayerStats(IN uname VARCHAR(50))
BEGIN
    SELECT 
        u.Username,

        COUNT(m.MatchID) AS TotalMatches,

        SUM(CASE 
            WHEN m.WinnerID = u.UserID THEN 1 
            ELSE 0 
        END) AS Wins,

        SUM(CASE 
            WHEN m.WinnerID IS NOT NULL AND m.WinnerID != u.UserID THEN 1 
            ELSE 0 
        END) AS Losses

    FROM Users u
    LEFT JOIN Matches m 
        ON m.Player1ID = u.UserID OR m.Player2ID = u.UserID

    WHERE u.Username = uname
    GROUP BY u.UserID;
END //
DELIMITER ;

DELIMITER //

CREATE PROCEDURE GetMostUsedMove(IN uname VARCHAR(50))
BEGIN
    SELECT MoveType, COUNT(*) AS UsageCount
    FROM Moves mv
    JOIN Users u ON mv.PlayerID = u.UserID
    WHERE u.Username = uname
    GROUP BY MoveType
    ORDER BY UsageCount DESC
    LIMIT 1;
END //
DELIMITER ;

DELIMITER //

CREATE PROCEDURE GetTournamentStats()
BEGIN
    SELECT 
        t.TournamentID,
        t.TournamentName,
        t.TotalPlayers,

        COUNT(tm.MatchID) AS TotalMatches,

        -- winner = last match winner (final)
        MAX(CASE 
            WHEN tm.RoundNumber = 1 THEN tm.Winner 
            ELSE NULL 
        END) AS Winner,

        CASE 
            WHEN MAX(CASE WHEN tm.RoundNumber = 1 AND tm.Winner IS NOT NULL THEN 1 ELSE 0 END) = 1
            THEN 'Completed'
            ELSE 'Ongoing'
        END AS Status

    FROM Tournaments t

    LEFT JOIN TournamentMatches tm 
        ON t.TournamentID = tm.TournamentID

    GROUP BY t.TournamentID

    ORDER BY t.TournamentID DESC;
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE GetRules(IN rtype VARCHAR(50))
BEGIN
    SELECT * FROM Rules WHERE RuleType = rtype;
END //

DELIMITER ;

CREATE TABLE Rules (
    RuleType VARCHAR(50) PRIMARY KEY,
    Title TEXT,
    LeftText TEXT,
    RightText TEXT
);

INSERT INTO Rules VALUES
(
'1v1',
'Rules (1v1 Battle)',
'GAME FLOW:
- Two players fight each other
- Each player selects 3 moves in advance
- Hide your screen from opponent
- Moves play automatically
- Reduce opponent HP to 0 to win

MOVES:
- Light Attack: Fast low damage
- Heavy Attack: Slow high damage
- Block: Defensive move',
'MOVE RESULTS:

- Light vs Heavy:
Heavy beats Light

- Heavy vs Block:
Block reflects damage

- Light vs Block:
Light breaks block

- Same move:
No HP loss'
);

INSERT INTO Rules VALUES
(
'tournament',
'Rules (Tournament)',
'TOURNAMENT STRUCTURE

- Players compete in knockout
- 4, 8 or 16 players
- Each match is 1v1
- Lose = eliminated

MATCH FLOW
- Each player selects 3 moves
- Moves executed automatically
- Damage applied each round',
'MOVE INTERACTIONS

- Light vs Heavy → Heavy wins
- Heavy vs Block → Block reflects
- Light vs Block → Light wins
- Same moves → No damage

WIN CONDITION
- Reduce HP to 0
- Winner advances
- Final decides champion'
);

INSERT INTO Maps (MapName) VALUES
('Castle Arena'), ('Colosseum Arena'), ('Prison Arena'), ('Cave Arena');
 
INSERT INTO MapStats (MapID) VALUES (1),(2),(3),(4);

