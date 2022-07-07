
-- SQL-Complient


CREATE TABLE users
(
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    name text NOT NULL,
    email text NOT NULL unique,
    created_at timestamp DEFAULT now()
)

SELECT * FROM users WHERE email = 'mr.jasta9@gmail.com'

SELECT * FROM users WHERE email LIKE 'mr.jasta9@gmail%'

SELECT * FROM users WHERE email ILIKE 'mr.Jasta9@gmail%'

SELECT * FROM users WHERE email ILIKE '%com%' ORDER BY id DESC OFFSET 1 LIMIT 1

SELECT row_to_json(users) FROM users


INSERT INTO users (name,email) VALUES ('Abdillahi', 'mr.jasta9@gmail.com')
INSERT INTO users (name,email) VALUES ('DR barkhad', 'bca@gmil.com')

UPDATE users SET name = 'Barkhad BCA' WHERE id = 6

DELETE FROM users WHERE id = 5

SELECT * FROM "Schedules"