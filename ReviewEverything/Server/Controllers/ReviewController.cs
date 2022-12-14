﻿using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.ReviewService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewResponse>>> GetAll(int? categoryId, string? userId, string? idTags)
        {
            try
            {
                List<int> tags = null!;
                if (idTags is { Length: > 1 })
                {
                    tags = idTags.Split('.').Select(int.Parse).ToList();
                }
                else if (idTags is { Length: 1 })
                {
                    tags = new List<int>() { int.Parse(idTags) };
                }

                var reviews = await _service.GetReviewsAsync(categoryId, userId, tags);
                return Ok(_mapper.Map<List<ReviewResponse>>(reviews));
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var review = await _service.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ArticleReviewResponse>(review));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> GetEditById([FromRoute] int id)
        {
            var review = await _service.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ReviewRequest>(review));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewRequest request)
        {
            var review = _mapper.Map<Review>(request);
            review.AuthorId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            review.CreationDate = DateTime.UtcNow;

            var result = await _service.CreateReviewAsync(review);
            return Created(Url.Action($"GetById", new { id = review.Id })!, _mapper.Map<ReviewResponse>(review));
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> Update([FromRoute] int reviewId, [FromBody] ReviewRequest request)
        {
            var review = _mapper.Map<Review>(request);
            review.Id = reviewId;

            var updated = await _service.UpdateReviewAsync(review);
            if (updated)
                return Ok(review);

            return NotFound();
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> Delete([FromRoute] int reviewId)
        {
            var deleted = await _service.DeleteReviewAsync(reviewId);
            if (deleted)
                return NoContent();

            return NotFound();
        }

    }
}
