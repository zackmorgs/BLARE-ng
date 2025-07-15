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

        // Get a subset of tags - default for tag display on home page
        [HttpGet("/some")]
        public async Task<IActionResult> GetSomeTags()
        {
            var tags = await _tagService.GetSome();
            return Ok(tags);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTags([FromQuery] string name)
        {
            List<MusicTag> tags = await _tagService.SearchTagsAsync(name);

            if (tags.Count == 0)
            {
                return NotFound($"No tags found with name: {name}");
            }
            else
            {
                return Ok(tags);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTag(string name)
        {
            // Return the created tag with a 201 Created response
            if (name == null || name == String.Empty)
            {
                return BadRequest("Tag cannot be null");
            }
            else
            {
                var formattedName = name.Trim().ToLower().Replace(" ", "-");
                var tag = new MusicTag { Name = formattedName };

                await _tagService.CreateAsync(tag);

                return Ok(tag);
            }
        }
    }
}
