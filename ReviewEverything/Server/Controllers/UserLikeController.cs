using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Services.UserLikeService;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserLikeController : ControllerBase
    {
        private readonly IUserLikeService _service;

        public UserLikeController(IUserLikeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] int reviewId)
        {
            try
            {
                var setLike = await _service.AddLikeToUserAsync(reviewId);
                if (setLike)
                    return Ok(setLike);
                return NotFound("Обзор для добавления лайка не найден");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время добавления лайка на обзор произошла внутренняя ошибка сервера");
            }

        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> UnLike(int reviewId)
        {
            try
            {
                var setLike = await _service.RemoveLikeFromUserAsync(reviewId);
                if (setLike)
                    return Ok(setLike);
                return NotFound("Обзор для удаления лайка не найден");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время добавления лайка на обзор произошла внутренняя ошибка сервера");
            }
        }
    }
}