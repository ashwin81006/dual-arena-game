const express = require("express");
const router = express.Router();
const db = require("../db");

// CREATE MATCH
router.post("/create", (req, res) => {
    const { p1, p2, map } = req.body;

    const sql = `
        INSERT INTO Matches (Player1ID, Player2ID, MapID)
        VALUES (?, ?, ?)
    `;

    db.query(sql, [p1, p2, map], (err, result) => {
        if (err) throw err;

        res.send({ success: true, matchID: result.insertId });
    });
});

// UPDATE WINNER
router.post("/winner", (req, res) => {
    const { matchID, winnerID } = req.body;

    const sql = `
        UPDATE Matches SET WinnerID=? WHERE MatchID=?
    `;

    db.query(sql, [winnerID, matchID], (err) => {
        if (err) throw err;

        res.send({ success: true });
    });
});

router.post("/saveMatch", (req, res) => {
    const { player1, player2, winner, mapId, rounds } = req.body;
    fixedMapId = parseInt(mapId) + 1;
    const parsedRounds = JSON.parse(rounds);

    const getUserQuery = "SELECT UserID FROM Users WHERE Username = ?";

    // 1. Get user IDs
    db.query(getUserQuery, [player1], (err, p1Res) => {
        if (err || p1Res.length === 0) return res.send({ success: false });

        db.query(getUserQuery, [player2], (err, p2Res) => {
            if (err || p2Res.length === 0) return res.send({ success: false });

            db.query(getUserQuery, [winner], (err, wRes) => {
                if (err || wRes.length === 0) return res.send({ success: false });

                const p1ID = p1Res[0].UserID;
                const p2ID = p2Res[0].UserID;
                const wID = wRes[0].UserID;

                // 2. Insert match
                const insertMatch = `
                    INSERT INTO Matches (Player1ID, Player2ID, WinnerID, MapID)
                    VALUES (?, ?, ?, ?)
                `;

                db.query(insertMatch, [p1ID, p2ID, wID, fixedMapId], (err, result) => {
                    if (err) {
                        console.log(err);
                        return res.send({ success: false });
                    }

                    const matchId = result.insertId;

                    // 3. Update MapStats
                    const updateMap = `
                        UPDATE MapStats
                        SET
                            TotalMatchesPlayed = TotalMatchesPlayed + 1,
                            Player1Wins = Player1Wins + IF(? = ?, 1, 0),
                            Player2Wins = Player2Wins + IF(? = ?, 1, 0)
                        WHERE MapID = ?
                    `;

                    db.query(updateMap, [wID, p1ID, wID, p2ID, mapId]);

                    // 4. Update PlayerStats (ONLY ONCE)
                    const updatePlayerStats = `
                        UPDATE PlayerStats
                        SET
                            TotalMatches = TotalMatches + 1,
                            Wins = Wins + IF(UserID = ?, 1, 0),
                            Losses = Losses + IF(UserID != ?, 1, 0)
                        WHERE UserID IN (?, ?)
                    `;

                    db.query(updatePlayerStats, [wID, wID, p1ID, p2ID]);

                    // 5. Insert Rounds + Moves
                    parsedRounds.rounds.forEach((round) => {

                        const insertRound = `
                            INSERT INTO Rounds (MatchID, RoundNumber)
                            VALUES (?, ?)
                        `;

                        db.query(insertRound, [matchId, round.roundNumber], (err, roundResult) => {
                            if (err) return console.log(err);

                            const roundId = roundResult.insertId;

                            round.moves.forEach((move) => {
                                db.query(getUserQuery, [move.player], (err, userRes) => {
                                    if (err || userRes.length === 0) return;

                                    const playerID = userRes[0].UserID;

                                    const insertMove = `
                                        INSERT INTO Moves (RoundID, PlayerID, MoveType, DamageDealt)
                                        VALUES (?, ?, ?, ?)
                                    `;

                                    db.query(insertMove, [
                                        roundId,
                                        playerID,
                                        move.move,
                                        move.damage
                                    ]);
                                    // Damage dealt
                                    db.query(`
                                        UPDATE PlayerStats
                                        SET TotalDamageDealt = TotalDamageDealt + ?
                                        WHERE UserID = ?
                                    `, [move.damage, playerID]);

                                    // Damage taken (other player)
                                    const opponentID = (playerID === p1ID) ? p2ID : p1ID;

                                    db.query(`
                                        UPDATE PlayerStats
                                        SET TotalDamageTaken = TotalDamageTaken + ?
                                        WHERE UserID = ?
                                    `, [move.damage, opponentID]);

                                });
                            });
                        });
                    });

                    res.send({ success: true, matchId: matchId });
                });
            });
        });
    });
});
module.exports = router;