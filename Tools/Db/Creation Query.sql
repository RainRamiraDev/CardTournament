
CREATE DATABASE IF NOT EXISTS `ctdatabase`;
USE `ctdatabase`;

CREATE TABLE IF NOT EXISTS `t_cards` (
  `id_card` int NOT NULL AUTO_INCREMENT,
  `illustration` varchar(30) NOT NULL,
  `attack` int DEFAULT 0,
  `defense` int DEFAULT 0,
  PRIMARY KEY (`id_card`)
) ENGINE=InnoDB AUTO_INCREMENT=77 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_series` (
  `id_series` int NOT NULL AUTO_INCREMENT,
  `series_name` varchar(30) NOT NULL,
  `release_date` datetime NOT NULL,
  PRIMARY KEY (`id_series`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_card_series` (
  `id_card_series` int NOT NULL AUTO_INCREMENT,
  `id_card` int DEFAULT NULL,
  `id_series` int DEFAULT NULL,
  PRIMARY KEY (`id_card_series`),
  KEY `fk_card_cseries` (`id_card`),
  KEY `fk_series_cseries` (`id_series`),
  CONSTRAINT `fk_card_cseries` FOREIGN KEY (`id_card`) REFERENCES `t_cards` (`id_card`),
  CONSTRAINT `fk_series_cseries` FOREIGN KEY (`id_series`) REFERENCES `t_series` (`id_series`)
) ENGINE=InnoDB AUTO_INCREMENT=77 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_countries` (
  `id_country` int NOT NULL AUTO_INCREMENT,
  `country_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id_country`)
) ENGINE=InnoDB AUTO_INCREMENT=251 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_roles` (
  `id_rol` int NOT NULL AUTO_INCREMENT,
  `rol` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_users` (
  `id_user` int NOT NULL AUTO_INCREMENT,
  `id_country` int DEFAULT NULL,
  `id_rol` int DEFAULT NULL,
  `fullname` varchar(50) DEFAULT NULL,
  `alias` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `passcode` varchar(255) DEFAULT NULL,
  `games_won` int DEFAULT 0,
  `games_lost` int DEFAULT 0,
  `avatar_url` varchar(255) DEFAULT NULL,
  `ki` int DEFAULT NULL,
  `active` TINYINT(1) DEFAULT 1,
  PRIMARY KEY (`id_user`),
  KEY `fk_country_user` (`id_country`),
  KEY `fk_rol_user` (`id_rol`),
  CONSTRAINT `fk_country_user` FOREIGN KEY (`id_country`) REFERENCES `t_countries` (`id_country`),
  CONSTRAINT `fk_rol_user` FOREIGN KEY (`id_rol`) REFERENCES `t_roles` (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_refresh_tokens` (
  `id_token` int NOT NULL AUTO_INCREMENT,
  `id_user` int DEFAULT NULL,
  `token` varchar(255) NOT NULL,
  `expiry_date` datetime NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_token`),
  UNIQUE KEY `token` (`token`),
  KEY `fk_user_token` (`id_user`),
  CONSTRAINT `fk_user_token` FOREIGN KEY (`id_user`) REFERENCES `t_users` (`id_user`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_tournaments` (
  `id_tournament` int NOT NULL AUTO_INCREMENT,
  `id_country` int DEFAULT NULL,
  `id_organizer` int DEFAULT NULL,
  `start_datetime` datetime DEFAULT NULL,
  `end_datetime` datetime DEFAULT NULL,
  `current_phase` int DEFAULT NULL,
  PRIMARY KEY (`id_tournament`),
  KEY `fk_tourn_country` (`id_country`),
  KEY `fk_tourn_organizer` (`id_organizer`),
  CONSTRAINT `fk_tourn_country` FOREIGN KEY (`id_country`) REFERENCES `t_countries` (`id_country`),
  CONSTRAINT `fk_tourn_organizer` FOREIGN KEY (`id_organizer`) REFERENCES `t_users` (`id_user`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_tourn_decks` (
  `id_tourn_deck` int NOT NULL AUTO_INCREMENT,
  `id_tournament` int DEFAULT NULL,
  `id_card_series` int DEFAULT NULL,
  `id_owner` int DEFAULT NULL,
  PRIMARY KEY (`id_tourn_deck`),
  KEY `fk_tourn_deck_tourn` (`id_tournament`),
  KEY `fk_tourn_deck_cseries` (`id_card_series`),
  KEY `fk_tourn_deck_owner` (`id_owner`),
  CONSTRAINT `fk_tourn_deck_cseries` FOREIGN KEY (`id_card_series`) REFERENCES `t_card_series` (`id_card_series`),
  CONSTRAINT `fk_tourn_deck_owner` FOREIGN KEY (`id_owner`) REFERENCES `t_users` (`id_user`),
  CONSTRAINT `fk_tourn_deck_tourn` FOREIGN KEY (`id_tournament`) REFERENCES `t_tournaments` (`id_tournament`)
) ENGINE=InnoDB AUTO_INCREMENT=2077 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_tourn_disqualifications` (
  `id_tourn_disqualification` int NOT NULL AUTO_INCREMENT,
  `id_tournament` int DEFAULT NULL,
  `id_player` int DEFAULT NULL,
  `id_judge` int DEFAULT NULL,
  PRIMARY KEY (`id_tourn_disqualification`),
  KEY `fk_tourn_disq_tourn` (`id_tournament`),
  KEY `fk_tourn_disq_player` (`id_player`),
  KEY `fk_tourn_disq_judge` (`id_judge`),
  CONSTRAINT `fk_tourn_disq_tourn` FOREIGN KEY (`id_tournament`) REFERENCES `t_tournaments` (`id_tournament`),
  CONSTRAINT `fk_tourn_disq_player` FOREIGN KEY (`id_player`) REFERENCES `t_users` (`id_user`),
  CONSTRAINT `fk_tourn_disq_judge` FOREIGN KEY (`id_judge`) REFERENCES `t_users` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_tourn_judges` (
  `id_tourn_judge` int NOT NULL AUTO_INCREMENT,
  `id_tournament` int DEFAULT NULL,
  `id_judge` int DEFAULT NULL,
  PRIMARY KEY (`id_tourn_judge`),
  KEY `fk_tourn_judges_tourn` (`id_tournament`),
  KEY `fk_tourn_judges_judge` (`id_judge`),
  CONSTRAINT `fk_tourn_judges_judge` FOREIGN KEY (`id_judge`) REFERENCES `t_users` (`id_user`),
  CONSTRAINT `fk_tourn_judges_tourn` FOREIGN KEY (`id_tournament`) REFERENCES `t_tournaments` (`id_tournament`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_tourn_players` (
  `id_tourn_player` int NOT NULL AUTO_INCREMENT,
  `id_tournament` int DEFAULT NULL,
  `id_player` int DEFAULT NULL,
  PRIMARY KEY (`id_tourn_player`),
  KEY `fk_tourn_player_tourn` (`id_tournament`),
  KEY `fk_tourn_player_player` (`id_player`),
  CONSTRAINT `fk_tourn_player_player` FOREIGN KEY (`id_player`) REFERENCES `t_users` (`id_user`),
  CONSTRAINT `fk_tourn_player_tourn` FOREIGN KEY (`id_tournament`) REFERENCES `t_tournaments` (`id_tournament`)
) ENGINE=InnoDB AUTO_INCREMENT=134 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_tourn_series` (
  `id_tourn_series` int NOT NULL AUTO_INCREMENT,
  `id_tournament` int DEFAULT NULL,
  `id_series` int DEFAULT NULL,
  PRIMARY KEY (`id_tourn_series`),
  KEY `fk_tourn_series_tourn` (`id_tournament`),
  KEY `fk_tourn_series_series` (`id_series`),
  CONSTRAINT `fk_tourn_series_series` FOREIGN KEY (`id_series`) REFERENCES `t_series` (`id_series`),
  CONSTRAINT `fk_tourn_series_tourn` FOREIGN KEY (`id_tournament`) REFERENCES `t_tournaments` (`id_tournament`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_rounds` (
  `id_round` int NOT NULL AUTO_INCREMENT,
  `id_tournament` int DEFAULT NULL,
  `round_number` int DEFAULT NULL,
  `judge` int DEFAULT NULL,
  PRIMARY KEY (`id_round`),
  KEY `fk_round_tournament` (`id_tournament`),
  CONSTRAINT `fk_round_tournament` FOREIGN KEY (`id_tournament`) REFERENCES `t_tournaments` (`id_tournament`),
  CONSTRAINT `fk_judge_rounds` FOREIGN KEY (`judge`) REFERENCES `t_users` (`id_user`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE IF NOT EXISTS `t_matches` (
  `id_match` int NOT NULL AUTO_INCREMENT,
  `match_date` Datetime NOT NULL,
  `id_round` int NOT NULL,
  `id_player1` int NOT NULL,
  `id_player2` int NOT NULL,
  `winner` int DEFAULT NULL,
  PRIMARY KEY (`id_match`),
  KEY `fk_match_round` (`id_round`),
  KEY `fk_match_player1` (`id_player1`),
  KEY `fk_match_player2` (`id_player2`),
  KEY `fk_match_winner` (`winner`),
  CONSTRAINT `fk_match_player1` FOREIGN KEY (`id_player1`) REFERENCES `t_users` (`id_user`),
  CONSTRAINT `fk_match_player2` FOREIGN KEY (`id_player2`) REFERENCES `t_users` (`id_user`),
  CONSTRAINT `fk_match_round` FOREIGN KEY (`id_round`) REFERENCES `t_rounds` (`id_round`),
  CONSTRAINT `fk_match_winner` FOREIGN KEY (`winner`) REFERENCES `t_users` (`id_user`)
) ENGINE=InnoDB AUTO_INCREMENT=107 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;





