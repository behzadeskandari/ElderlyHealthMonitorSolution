using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ElderlyHealthMonitor.DTOS.DTO;

namespace ElderlyHealthMonitorSolution.API.Controllers
{
    [ApiController]
    [Route("api/readings")]
    public class ReadingsController : ControllerBase
    {
        private readonly IReadingService _readingService;
        public ReadingsController(IReadingService readingService) { _readingService = readingService; }


        [HttpPost]
        public async Task<IActionResult> Ingest([FromBody] IEnumerable<SensorReadingDto> dto)
        {
            if (dto == null || !dto.Any()) return BadRequest("no readings");
            var count = await _readingService.IngestReadingsAsync(dto);
            return Ok(new { ingested = count });
        }
    }
}
