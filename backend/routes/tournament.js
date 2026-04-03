const express = require("express");
const router = express.Router();
const db = require("../db");


router.post("/saveMatch", (req, res) => {
    const { tournamentId, matchIndex, winner, roundNumber } = req.body;

    console.log("=== SAVE MATCH ===");
    console.log("TID:", tournamentId);
    console.log("Index:", matchIndex);
    console.log("Winner:", winner);
    console.log("Round Number:", roundNumber);

    const sql = `
        UPDATE TournamentMatches
        SET Winner = ?
        WHERE TournamentID = ? AND MatchOrder = ? AND RoundNumber = ?
    `;

    db.query(sql, [winner, tournamentId, matchIndex, roundNumber], (err, result) => {
        if (err) {
            console.log("❌ DB ERROR:", err);
            return res.send({ success: false });
        }

        console.log("✅ Updated match");
        res.send({ success: true });
    });
});


router.post("/createTournament", (req, res) => {
    const { name, playerCount } = req.body;

    const sql = `
        INSERT INTO Tournaments (TournamentName, TotalPlayers)
        VALUES (?, ?)
    `;

    db.query(sql, [name, playerCount], (err, result) => {
        if (err) {
            console.log("❌ CREATE TOURNAMENT ERROR:", err);
            return res.send({ success: false });
        }

        res.send({
            success: true,
            tournamentId: result.insertId // 🔥 IMPORTANT
        });
    });
});

router.post("/createMatches", (req, res) => {
    const { tournamentId, matches } = req.body;

    console.log("RAW:", matches); // debug

    const parsedMatches = JSON.parse(matches); // ✅ ADD HERE

    console.log("PARSED:", parsedMatches);

    const values = parsedMatches.matches.map(m => [
        tournamentId,
        m.player1,
        m.player2,
        "",
        m.roundNumber,
        m.matchOrder
    ]);

    const sql = `
        INSERT INTO TournamentMatches
        (TournamentID, Player1, Player2, Winner, RoundNumber, MatchOrder)
        VALUES ?
    `;

    db.query(sql, [values], (err, result) => {
        if (err) {
            console.log("INSERT ERROR:", err);
            return res.send({ success: false });
        }

        res.send({ success: true });
    });
});

router.get("/getTournaments", (req, res) => {
    const sql = "SELECT TournamentID, TournamentName FROM Tournaments";

    db.query(sql, (err, result) => {
        if (err) {
            console.log("❌ FETCH TOURNAMENTS ERROR:", err);
            return res.send([]);
        }

        res.send(result);
    });
});

router.get("/getMatches/:tournamentId", (req, res) => {
    const { tournamentId } = req.params;

    const sql = `
        SELECT Player1, Player2, Winner, RoundNumber, MatchOrder
        FROM TournamentMatches
        WHERE TournamentID = ?
        ORDER BY RoundNumber ASC, MatchOrder ASC
    `;

    db.query(sql, [tournamentId], (err, result) => {
        if (err) {
            console.log("❌ FETCH MATCHES ERROR:", err);
            return res.send([]);
        }

        res.send(result);
    });
});
module.exports = router;