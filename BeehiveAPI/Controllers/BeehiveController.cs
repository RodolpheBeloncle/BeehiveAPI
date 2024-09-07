// Controllers/BeehiveController.cs
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BeehiveAPI.Data;
using BeehiveAPI.Models;

namespace BeehiveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeehiveController : ControllerBase
    {
        private readonly BeehiveContext _context;

        public BeehiveController(BeehiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBeehives()
        {
            var beehives = _context.Beehives.ToList();
            return Ok(beehives);
        }

        [HttpPost]
        public IActionResult CreateBeehive(Beehive beehive)
        {
            beehive.CreatedAt = DateTime.Now;
            _context.Beehives.Add(beehive);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetBeehives), new { id = beehive.Id }, beehive);
        }
    }
}
