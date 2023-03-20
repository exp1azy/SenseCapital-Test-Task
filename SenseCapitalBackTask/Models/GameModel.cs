﻿using SenseCapitalBackTask.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SenseCapitalBackTask.Models
{
    public class GameModel
    {
        public int Id { get; set; } // Идентификатор игры

        public int ZeroId { get; set; } // Идентификатор игрока, который играет ноликами

        public int CrossId { get; set; } // Идентификатор игрока, который играет крестиками

        public int Whose { get; set; } // Идентификатор игрока, который сейчас ходит

        public bool IsNew { get; set; } // Новая ли игра

        public bool IsFinished { get; set; } // Закончена ли игра

        public ICollection<CellsModel> Cells { get; set; } // Клетки игрового поля

        public static GameModel? Map(Game game) => game == null ? null : new GameModel() // Возвращает модель таблицы
        {
            Id = game.Id,
            ZeroId = game.ZeroPlayerId,
            CrossId = game.CrossPlayerId,
            Whose = game.Whose,
            IsNew = game.Cells != null && game.Cells.Any(c => c.State != null),
            IsFinished = game.Cells != null && game.Cells.Any(c => c.Mark),
            Cells = game.Cells != null ? game.Cells.Select(CellsModel.Map).ToList() : null
        };
    }
}