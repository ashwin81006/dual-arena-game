const mysql = require("mysql");

const db = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "Mithun@1313",
    database: "DuelArenaDB"
});

db.connect(err => {
    if (err) {
        console.log("DB Error:", err);
    } else {
        console.log("MySQL Connected");
    }
});

module.exports = db;