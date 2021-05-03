using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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
            if (!(celestialObjects?.Any() ?? false))
            {
                return NotFound();
            }
            celestialObjects.ForEach(o =>
            {
                o.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == o.Id).ToList();
            });
            return Ok(celestialObjects);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            celestialObjects.ForEach(o =>
            {
                o.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == o.Id).ToList();
            });
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute(routeName: "GetById", routeValues: new { id = celestialObject.Id }, value: celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = celestialObject.Name;
            obj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            obj.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = name;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objs = _context.CelestialObjects.Where(c => c.Id == id || (c.OrbitedObjectId.HasValue && c.OrbitedObjectId.Value == id));
            if (!(objs?.Any() ?? false))
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(objs);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
