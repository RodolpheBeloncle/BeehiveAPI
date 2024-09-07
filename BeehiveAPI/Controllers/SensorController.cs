// Controllers/SensorController.cs
using Microsoft.AspNetCore.Mvc;
using BeehiveAPI.Data;
using BeehiveAPI.Models;
using System.Linq;

namespace BeehiveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly BeehiveContext _context;

        public SensorController(BeehiveContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RecordSensorData(Sensor sensor)
        {
            sensor.RecordedAt = DateTime.Now;
            _context.Sensors.Add(sensor);
            _context.SaveChanges();
            return Ok(sensor);
        }

        [HttpGet("{beehiveId}")]
        public IActionResult GetSensorData(int beehiveId)
        {
            var sensors = _context.Sensors.Where(s => s.BeehiveId == beehiveId).ToList();
            return Ok(sensors);
        }
    }
}