using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.UserScoreService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserScoreController : ControllerBase
    {
        private readonly IUserScoreService _service;
        private readonly IMapper _mapper;

        public UserScoreController(IUserScoreService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] UserScoreRequest request)
        {
            try
            {
                await _service.CreateOrUpdateScopeAsync(_mapper.Map<UserScore>(request));
                return Ok();
            }
            catch
            {
                return BadRequest();
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

                return NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}