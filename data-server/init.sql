
--- User table
CREATE TABLE IF NOT EXISTS stud (
    id              SERIAL PRIMARY KEY,
    email           VARCHAR(255) UNIQUE,
    username        VARCHAR(50) NOT NULL,
    password_hash   TEXT        NOT NULL
);

--- Inventories table

CREATE TABLE IF NOT EXISTS bricklink_inventory (
    id    INT PRIMARY KEY,
    inventory_data  JSONB,
    user_id         INT NOT NULL,

    CONSTRAINT fk_user_id
            FOREIGN KEY(user_id) REFERENCES stud(id)
            ON DELETE CASCADE
)
