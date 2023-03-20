CREATE TABLE player
(
	player_id serial NOT NULL PRIMARY KEY,
	player_name varchar(20) NOT NULL,
	player_password varchar(20) NOT NULL
);

CREATE TABLE game
(
	game_id serial NOT NULL PRIMARY KEY,
	zero_id int NOT NULL REFERENCES player(player_id),
	cross_id int NOT NULL REFERENCES player(player_id),
	whose int NOT NULL
);

CREATE TABLE cell
(
	game_id int NOT NULL REFERENCES game(game_id),
	cell_row int NOT NULL,
	cell_col int NOT NULL,
	cell_state boolean,
	mark boolean NOT NULL,
	PRIMARY KEY(game_id, cell_row, cell_col)
);