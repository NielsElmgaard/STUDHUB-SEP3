CREATE TABLE IF NOT EXISTS stud (
    email VARCHAR(255) PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password_hash TEXT NOT NULL
);
