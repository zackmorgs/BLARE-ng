using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagService.GetAsync();
            return Ok(tags);
        }

        // [HttpPost]
        // public async Task<IActionResult> CreateTag(Tag tag)
        // {
        //     // Set timestamps
        //     tag.CreatedAt = DateTime.UtcNow;
        //     tag.UpdatedAt = DateTime.UtcNow;

        //     await _tagService.CreateAsync(tag);
        //     return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, tag);
        // }
    }
}
