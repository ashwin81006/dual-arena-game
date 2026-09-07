# Duel Arena

Duel Arena is a turn-based 1v1 fighting game built with Unity, C#, Node.js, Express, and MySQL. It combines a Unity game client with backend services for authentication, matches, tournaments, statistics, game rules, and persistent gameplay data.

## Features

- 1v1 turn-based combat
- 4, 8, and 16-player tournaments
- Multiple battle maps
- Player registration and login
- Match history and results
- Round and move tracking
- Player and gameplay statistics
- Database-driven game rules

## Architecture

```text
Unity Client (C#)
       |
    REST API
       |
Node.js + Express
       |
MySQL Database
       |
Players | Matches | Rounds | Tournaments | Rules
```

## Tech Stack

| Layer | Technology |
|---|---|
| Game Engine | Unity |
| Game Logic | C# |
| Backend | Node.js, Express |
| Database | MySQL |
| Communication | REST API |
| Package Manager | npm |

## Project Structure

```text
Duel-Arena/
├── Dual-Arena/          # Unity game project
├── backend/             # Node.js + Express API
│   ├── routes/
│   │   ├── auth.js
│   │   ├── match.js
│   │   ├── rules.js
│   │   ├── stats.js
│   │   └── tournament.js
│   ├── db.js
│   └── server.js
├── database/            # MySQL schema
├── package.json
├── package-lock.json
└── README.md
```

## Backend Modules

| Route | Purpose |
|---|---|
| `/auth` | Player registration and login |
| `/match` | Match creation and match data |
| `/rules` | Game-rule retrieval |
| `/stats` | Player and gameplay statistics |
| `/tournament` | Tournament management |

## Game Flow

```text
Register / Login
      ↓
Enter Arena
      ↓
Start 1v1 Match
      ↓
Players Take Turns
      ↓
Rounds and Moves Recorded
      ↓
Winner Determined
      ↓
Statistics and Match History Updated
```

## Setup

### 1. Clone

```bash
git clone https://github.com/Mithunsurya-Kumarasamy/dual-arena-game.git
cd dual-arena-game
```

### 2. Setup MySQL

Create the database and tables using:

```text
database/database.sql
```

### 3. Install Backend Dependencies

```bash
cd backend
npm install
```

### 4. Configure Environment Variables

Create `.env` inside the backend directory:

```env
DB_HOST=localhost
DB_USER=your_user
DB_PASSWORD=your_password
DB_NAME=DuelArenaDB
```

### 5. Start the Backend

```bash
node server.js
```

### 6. Run the Unity Client

Open `Dual-Arena` in Unity, allow the project to import, open the configured scene, and run the game.

## Data Persistence

The backend stores:

- Player accounts
- Match results
- Players and winners
- Rounds and moves
- Maps
- Player statistics
- Tournament data
- Game rules

## Project Highlights

- Full-stack architecture connecting Unity with a REST backend.
- Modular Express backend with dedicated route handlers.
- Persistent relational data using MySQL.
- Tournament support for multiple player counts.
- Data-driven rules and gameplay statistics.
- Match history and detailed gameplay tracking.

## Authors

**Mithunsurya Kumarasamy**  
**Ashwin Siva**

---

A full-stack game project demonstrating Unity development, REST API design, backend architecture, relational database integration, and persistent gameplay analytics.
