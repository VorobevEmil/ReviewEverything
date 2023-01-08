using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Services.UserLikeService;
using System.Net;
using Microsoft.Extensions.Localization;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserLikeController : ControllerBase
    {
        private readonly IUserLikeService _service;
        private readonly IStringLocalizer<UserLikeController> _localizer;

        public UserLikeController(IUserLikeService service, IStringLocalizer<UserLikeController> localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] int reviewId)
        {
            try
            {
                var setLike = await _service.AddLikeToUserAsync(reviewId);
                if (setLike)
                    return Ok(setLike);

                return BadRequest(_localizer["Не удалось поставить лайк"].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer["Обзор для добавления лайка не найден"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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

                return BadRequest(_localizer["Не удалось удалить лайк"].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer["Обзор для удаления лайка не найден"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}