namespace Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;

    [ApiController]
    [Route("api/[controller]")]
    public class StreamController : ControllerBase
    {


        [HttpGet("{filename}")]
        public IActionResult StreamAudio(string filename)
        {
            var filePath = Path.Combine("wwwroot/tracks", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return File(stream, "audio/mpeg", enableRangeProcessing: true);
        }
    }
}
