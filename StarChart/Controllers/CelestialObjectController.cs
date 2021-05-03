using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext applicationDbContext)
        {
            this._context = applicationDbContext;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null) return NotFound();
            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == id)
                .ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetById")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            if(!(celestialObjects?.Any() ?? false))
            {
                return NotFound();
            }
            celestialObjects.ForEach(o => {
                o.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == o.Id).ToList();
            });
            return Ok(celestialObjects);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            celestialObjects.ForEach(o => {
                o.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == o.Id).ToList();
            });
            return Ok(celestialObjects);
        }
    }
}
