using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    public class SlugController : ControllerBase
    {
        private readonly SlugService _slugService;

        public SlugController(SlugService slugService)
        {
            _slugService = slugService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetBySlug(string name)
        {
            var result = await _slugService.GenerateSlug(name);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var slug = await _slugService.GetByIdAsync(id);
            if (slug == null)
            {
                return NotFound();
            }
            return Ok(slug);
        }
    }
}
