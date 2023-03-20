using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenseCapitalBackTask.Extensions;
using SenseCapitalBackTask.Models;
using SenseCapitalBackTask.Services;

namespace SenseCapitalBackTask.Controllers
{
    [ApiController]
    [Authorize]
    public class GameController : Controller
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Кидает вызов сопернику
        /// </summary>
        /// <param name="enemy">Никнейм игрока</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Новая игра</returns>
        [HttpPost("game/{enemy}")]
        public async Task<IActionResult> SendInvitation(string enemy, CancellationToken cancellationToken)
        {
            var response = await _gameService.SendInvintationAsync(HttpContext.GetPlayer().Id, enemy, cancellationToken);

            if (response)
            {
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// Получает список игр
        /// </summary>
        /// <param name="mode">Чей ход ожидается</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Если в mode пришло "in" - возвращаются игры, где ожидается ход игрока, "out" - игры, где ожидается ход соперника</returns>
        [HttpGet("games/{mode}")]
        public async Task<IActionResult> GetGameList(string mode, CancellationToken cancellationToken)
        {
            var games = await _gameService.GetGameListAsync(HttpContext.GetPlayer().Id, mode, cancellationToken);

            return Ok(games);
        }

        /// <summary>
        /// Делает ход
        /// </summary>
        /// <param name="move">Ход</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная модель игры</returns>
        [HttpPost("move")]
        public async Task<IActionResult> MakeMove(GameMoveModel move, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _gameService.MakeMoveAsync(move, cancellationToken);

                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Вернуть игру
        /// </summary>
        /// <param name="id">Идентификатор игры</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Возвращает игру по её идентификатору</returns>
        [HttpGet("game/{id}")]
        public async Task<IActionResult> GetGame(int id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _gameService.GetGameByIdAsync(HttpContext.GetPlayer().Id, id, cancellationToken);

                return Ok(response);
            }
            catch (ApplicationException ex) 
            { 
                return NotFound(ex.Message);
            }
        }
    }
}
