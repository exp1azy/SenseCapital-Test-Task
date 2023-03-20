using Microsoft.EntityFrameworkCore;
using SenseCapitalBackTask.Data;
using SenseCapitalBackTask.Models;

namespace SenseCapitalBackTask.Services
{
    public class GameService
    {
        private readonly DataContext _dataContext;

        public GameService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Кидает вызов сопернику и создает новую игру
        /// </summary>
        /// <param name="playerId">Идентификатор игрока, который кидает вызов</param>
        /// <param name="enemyName">Имя соперника, которому кидают вызов</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если соперник существует, либо False, если соперник не найден</returns>
        public async Task<bool> SendInvintationAsync(int playerId, string enemyName, CancellationToken cancellationToken)
        {
            var getEnemyId = await _dataContext.Player.Where(i => i.Name == enemyName).FirstOrDefaultAsync(cancellationToken);

            var enemy = PlayerModel.Map(getEnemyId);

            if (enemy != null)
            {
                var newGame = new Game()
                {
                    ZeroPlayerId = playerId,
                    CrossPlayerId = (int)enemy.Id,
                    Whose = (int)enemy.Id
                };

                var cells = new List<Cell>();

                for (var col = 0; col < 3; col++)
                {
                    for (var row = 0; row < 3; row++)
                    {
                        cells.Add(new Cell
                        {
                            Row = row,
                            Col = col,
                            State = null,
                            Mark = false
                        });
                    }                      
                }
                    
                newGame.Cells = cells;

                await _dataContext.Game.AddAsync(newGame, cancellationToken);
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получает список игр
        /// </summary>
        /// <param name="playerId">Идентификатор текущего игрока</param>
        /// <param name="mode">Чей ход ожидается</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список игр, где если в mode пришло "in" - возвращаются игры, где ожидается ход игрока, "out" - игры, где ожидается ход соперника, иначе возвращается пустой список</returns>
        public async Task<List<GameHeaderModel>> GetGameListAsync(int playerId, string mode, CancellationToken cancellationToken)
        {
            var query = _dataContext.Game.Include(i => i.Cells).Include(i => i.ZeroPlayer).Include(i => i.CrossPlayer).Where(g => g.ZeroPlayerId == playerId || g.CrossPlayerId == playerId);
            query = mode.ToLower() == "in" ? query.Where(g => g.Whose == playerId)
                : mode.ToLower() == "out" ? query.Where(g => g.Whose != playerId)
                : query.Where(g => false);

            return (await query.ToListAsync(cancellationToken)).Select(s => new GameHeaderModel()
            {
                Id = s.Id,
                EnemyName = s.CrossPlayerId == playerId ? s.ZeroPlayer.Name : s.CrossPlayer.Name,
                IsNew = !s.Cells.Any(c => c.State != null)
            }).ToList();
        }

        /// <summary>
        /// Делает ход
        /// </summary>
        /// <param name="move">Ход</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная модель игры</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<GameModel> MakeMoveAsync(GameMoveModel move, CancellationToken cancellationToken)
        {
            var game = await _dataContext.Game.Include(g => g.Cells).Where(i => i.Id == move.GameId).FirstOrDefaultAsync(cancellationToken);
            if (game == null)
                throw new ApplicationException("Игра не найдена");

            if (game.Cells.Any(c => c.Mark))
                throw new ApplicationException("Игра завершена");

            var cell = game.Cells.FirstOrDefault(c => c.Row == move.Row);
            if (cell == null)
                throw new ApplicationException("Ячейка не найдена");

            if (cell.State != null)
                throw new ApplicationException("Ячейка занята");

            if (game.Whose != move.PlayerId)
                throw new ApplicationException("Ход другого игрока");

            cell.State = move.PlayerId == game.CrossPlayerId;
            game.Whose = move.PlayerId == game.CrossPlayerId ? game.ZeroPlayerId : game.CrossPlayerId;

            CheckGameFinished(game);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return GameModel.Map(game);
        }

        /// <summary>
        /// Проверка, что игра завершилась
        /// </summary>
        /// <param name="game">Игра</param>
        private void CheckGameFinished(Game game)
        {
            var row = game.Cells.GroupBy(c => c.Row).FirstOrDefault(r => r.All(cl => cl.State == true) || r.All(cl => cl.State == false));
            if (row != null)
            {
                foreach (var cell in row)
                {
                    cell.Mark = true;
                }
                return;
            }

            var col = game.Cells.GroupBy(c => c.Col).FirstOrDefault(r => r.All(cl => cl.State == true) || r.All(cl => cl.State == false));
            if (col != null)
            {
                foreach (var cell in col)
                {
                    cell.Mark = true;
                }
                return;
            }

            var diag = game.Cells.Where(c => c.Col == c.Row);
            if (diag.All(cl => cl.State == true) || diag.All(cl => cl.State == false))
            {
                foreach (var cell in diag)
                {
                    cell.Mark = true;
                }
                return;
            }

            diag = game.Cells.Where(c => (c.Col == 0 && c.Row == 2) || (c.Col == 1 && c.Row == 1) || (c.Col == 2 && c.Row == 0));
            if (diag.All(cl => cl.State == true) || diag.All(cl => cl.State == false))
            {
                foreach (var cell in diag)
                {
                    cell.Mark = true;
                }
            }
        }

        /// <summary>
        /// Вернуть игру по идентификатору
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <param name="currentPlayerId">Идентификатор текущего игрока</param>
        /// <returns>Модель игры</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<GameModel> GetGameByIdAsync(int currentPlayerId, int gameId, CancellationToken cancellationToken)
        {
            var game = await _dataContext.Game.Include(g => g.Cells).FirstOrDefaultAsync(g => g.Id == gameId, cancellationToken);

            if (game == null)
                throw new ApplicationException("Игра не найдена");

            if (game.CrossPlayerId != currentPlayerId && game.ZeroPlayerId != currentPlayerId)
                throw new ApplicationException("Игра не найдена");

            return GameModel.Map(game);
        }
    }
}
