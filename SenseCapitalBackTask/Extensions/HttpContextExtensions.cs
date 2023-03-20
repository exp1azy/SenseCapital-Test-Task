using SenseCapitalBackTask.Models;
using System.Security.Claims;

namespace SenseCapitalBackTask.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Расширение HttpContext
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <returns>Возвращает авторизованного игрока</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static PlayerModel GetPlayer(this HttpContext context)
        {
            var idClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var nameClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (!int.TryParse(idClaim?.Value, out var id) || nameClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            return new PlayerModel
            {
                Id = id,
                Name = nameClaim.Value
            };
        }
    }
}
