CREATE DATABASE DuelArenaDB;
USE DuelArenaDB;

-- =========================
-- USERS
-- =========================
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) UNIQUE,
    Password VARCHAR(50),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =========================
-- MAPS
-- =========================
CREATE TABLE Maps (
    MapID INT AUTO_INCREMENT PRIMARY KEY,
    MapName VARCHAR(50)
);

INSERT INTO Maps (MapName) VALUES
('Desert'),
('Forest'),
('Arena'),
('Snow');

-- =========================
-- MATCHES
-- =========================
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

-- =========================
-- ROUNDS
-- =========================
CREATE TABLE Rounds (
    RoundID INT AUTO_INCREMENT PRIMARY KEY,
    MatchID INT,
    RoundNumber INT,

    FOREIGN KEY (MatchID) REFERENCES Matches(MatchID)
);

-- =========================
-- MOVES
-- =========================
CREATE TABLE Moves (
    MoveID INT AUTO_INCREMENT PRIMARY KEY,
    RoundID INT,
    PlayerID INT,
    MoveType VARCHAR(20),
    DamageDealt INT,

    FOREIGN KEY (RoundID) REFERENCES Rounds(RoundID),
    FOREIGN KEY (PlayerID) REFERENCES Users(UserID)
);

-- =========================
-- PLAYER STATS
-- =========================
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

-- =========================
-- MAP STATS
-- =========================
CREATE TABLE MapStats (
    MapStatID INT AUTO_INCREMENT PRIMARY KEY,
    MapID INT,
    TotalMatchesPlayed INT DEFAULT 0,
    Player1Wins INT DEFAULT 0,
    Player2Wins INT DEFAULT 0,

    FOREIGN KEY (MapID) REFERENCES Maps(MapID)
);

-- =========================
-- TOURNAMENTS
-- =========================
CREATE TABLE Tournaments (
    TournamentID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentName VARCHAR(50),
    StartDate DATE,
    EndDate DATE
);

-- =========================
-- TOURNAMENT MATCHES
-- =========================
CREATE TABLE TournamentMatches (
    TM_ID INT AUTO_INCREMENT PRIMARY KEY,
    TournamentID INT,
    MatchID INT,
    Stage VARCHAR(50),

    FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
    FOREIGN KEY (MatchID) REFERENCES Matches(MatchID)
);

-- =========================
-- SAMPLE USERS
-- =========================
INSERT INTO Users (Username, Password) VALUES
('Ashwin', '123'),
('Mithun', '1234'),
('Player3', '123'),
('Player4', '123'),
('Player5', '123'),
('Player6', '123');

-- =========================
-- SAMPLE MATCHES
-- =========================
INSERT INTO Matches (Player1ID, Player2ID, WinnerID, MapID) VALUES
(1, 2, 1, 1),
(2, 3, 2, 2),
(3, 4, 3, 3),
(1, 4, 1, 1);

INSERT INTO PlayerStats (UserID, TotalMatches, Wins, Losses)
VALUES
(1, 5, 3, 2),
(2, 4, 2, 2),
(3, 3, 1, 2);

INSERT INTO Rounds (MatchID, RoundNumber) VALUES
(1,1),
(1,2),
(2,1),
(2,2);

INSERT INTO Moves (RoundID, PlayerID, MoveType, DamageDealt)
VALUES
(1, 1, 'Light', 10),
(1, 1, 'Light', 12),
(1, 1, 'Heavy', 20),
(2, 1, 'Light', 8),

(3, 2, 'Heavy', 25),
(3, 2, 'Heavy', 22),
(4, 2, 'Block', 0);

-- =========================
-- 🥇 TOP PLAYERS
-- =========================
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

-- =========================
-- 📜 MATCH HISTORY
-- =========================
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

-- =========================
-- 🗺️ MOST PLAYED MAP
-- =========================
DELIMITER //
CREATE PROCEDURE GetMostPlayedMap()
BEGIN
    SELECT mp.MapName, COUNT(*) AS TimesPlayed
    FROM Matches m
    JOIN Maps mp ON m.MapID = mp.MapID
    GROUP BY m.MapID
    ORDER BY TimesPlayed DESC
    LIMIT 1;
END //
DELIMITER ;

-- =========================
-- 📊 WIN RATE
-- =========================

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
