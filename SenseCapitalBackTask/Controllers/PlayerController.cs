using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SenseCapitalBackTask.Extensions;
using SenseCapitalBackTask.Models;
using SenseCapitalBackTask.Services;

namespace SenseCapitalBackTask.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class PlayerController : Controller
    {
        private readonly PlayerService _playerService;

        public PlayerController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// Авторизация игрока
        /// </summary>
        /// <param name="username">Имя игрока</param>
        /// <param name="password">Пароль игрока</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Токен игрока</returns>
        [HttpPost("user/login")]
        public async Task<IActionResult> Login(string username, string password, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _playerService.AuthenticatePlayerAsync(username, password, cancellationToken);
                var token = _playerService.GenerateToken(response);

                return Ok(token);
            }
            catch(ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }           
        }

        /// <summary>
        /// Регистрация игрока
        /// </summary>
        /// <param name="player">Новый игрок</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Модель игрока</returns>
        [HttpPost("user/register")]
        public async Task<IActionResult> Register(PlayerModel player, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _playerService.AddPlayerAsync(player, cancellationToken);

                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Список соперников
        /// </summary>
        /// <param name="request">Имя соперника (необязательно)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Взовращает список соперников по имени</returns>
        [Authorize]
        [HttpGet("enemies")]
        public async Task<IActionResult> GetEnemies(string? request, CancellationToken cancellationToken)
        {
            var response = await _playerService.GetAllEnemiesAsync(request, (int)HttpContext.GetPlayer().Id, cancellationToken);

            return Ok(response);
        }
    }
}
