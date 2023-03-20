using Microsoft.AspNetCore.Identity;
using SenseCapitalBackTask.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using SenseCapitalBackTask.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SenseCapitalBackTask.Services
{
    public class PlayerService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _config;

        public PlayerService(DataContext dataContext, IConfiguration config)
        {
            _dataContext = dataContext;
            _config = config;
        }

        /// <summary>
        /// Сгенерировать токен
        /// </summary>
        /// <param name="player">Модель игрока</param>
        /// <returns>Токен</returns>
        public string GenerateToken(PlayerModel player)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, player.Name),
                new Claim(ClaimTypes.NameIdentifier, player.Id.ToString())
            };

            var token = new JwtSecurityToken(_config["JWT:Issuer"], _config["JWT:Audience"], claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Добавить игрока
        /// </summary>
        /// <param name="player">Модель игрока</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Модель игрока, прошедшую проверки</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<PlayerModel> AddPlayerAsync(PlayerModel player, CancellationToken cancellationToken)
        {
            if (Regex.IsMatch(player.Name, @"^[a-zA-Z]{4,}$"))
            {
                if (Regex.IsMatch(player.Password, @"^(?=.*[0-9])(?=.*[a-zA-Z]).{6,}$"))
                {
                    if (await _dataContext.Player.AnyAsync(u => u.Name == player.Name, cancellationToken))
                    {
                        throw new ApplicationException("Пользователь с таким логином уже существует!");
                    }
                    else
                    {
                        var newPlayer = new Player()
                        {
                            Name = player.Name,
                            Pass = PasswordHasher.Hash(player.Password, _config["Salt"])
                        };

                        await _dataContext.Player.AddAsync(newPlayer, cancellationToken);
                        await _dataContext.SaveChangesAsync(cancellationToken);
                    }
                }
                else
                {
                    throw new ApplicationException("Пароль должен содержать только символы английского алфавита и хотя бы одну цифру, а также состоять хотя бы из 6 символов!");
                }
            }
            else
            {
                throw new ApplicationException("Никнейм должен содержать только символы английского алфавита и состоять хотя бы из четырёх символов!");
            }

            return player;
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="playerName">Имя игрока</param>
        /// <param name="password">Пароль игрока</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Аутентифицированного пользователя</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<PlayerModel> AuthenticatePlayerAsync(string playerName, string password, CancellationToken cancellationToken)
        {
            var currentPlayer = new PlayerModel();

            var dalPlayer = await _dataContext.Player.FirstOrDefaultAsync(u => u.Name == playerName, cancellationToken);

            if (dalPlayer != null)
            {
                password = PasswordHasher.Hash(password, _config["Salt"]);

                if (password == dalPlayer.Pass)
                {
                    currentPlayer.Id = dalPlayer.Id;
                    currentPlayer.Name = playerName;
                    currentPlayer.Password = password;
                }
                else
                {
                    throw new ApplicationException("Неверный пароль! Проверьте введённые данные");
                }
            }
            else
            {
                throw new ApplicationException("Такого пользователя не существует! Проверьте введённые данные");
            }

            return currentPlayer;
        }

        /// <summary>
        /// Соперники
        /// </summary>
        /// <param name="request">Текстовый запрос</param>
        /// <param name="currentPlayerId">Идентификатор текущего игрока</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список соперников</returns>
        public async Task<List<PlayerModel?>> GetAllEnemiesAsync(string? request, int currentPlayerId, CancellationToken cancellationToken)
        {
            var playersOnline = _dataContext.Player.Where(i => i.Id != currentPlayerId);
            playersOnline = request != null ? playersOnline.Where(i => i.Name.Contains(request)).AsQueryable() : playersOnline.AsQueryable();

            return (await playersOnline.ToListAsync(cancellationToken)).Select(PlayerModel.Map).ToList();
        }
    }
}