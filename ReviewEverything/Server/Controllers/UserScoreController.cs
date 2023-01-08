using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.UserScoreService;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserScoreController : ControllerBase
    {
        private readonly IUserScoreService _service;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<UserScoreController> _localizer;

        public UserScoreController(IUserScoreService service, IMapper mapper, IStringLocalizer<UserScoreController> localizer)
        {
            _service = service;
            _mapper = mapper;
            _localizer = localizer;
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] UserScoreRequest request)
        {
            try
            {
                await _service.CreateOrUpdateScopeAsync(_mapper.Map<UserScore>(request));
                return Ok(_localizer["Пользовательский рейтинг изменен"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{compositionId}")]
        public async Task<IActionResult> Delete([FromRoute] int compositionId)
        {
            try
            {
                var deleted = await _service.DeleteScopeAsync(compositionId);
                if (deleted)
                    return NoContent();

                return BadRequest();
            }
            catch (HttpStatusRequestException e) when(e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer[e.Message]);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}