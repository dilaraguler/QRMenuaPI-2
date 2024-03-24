using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuaPI.Data;
using QRMenuaPI.Models;

namespace QRMenuaPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FoodsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Foods
        [Authorize (Roles ="RestaurantAdministrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFood()
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            return await _context.Foods.Where(r=>r.StateId ==1).ToListAsync();
        }

        // GET: api/Foods/5
        [Authorize (Roles ="RestaurantAdministrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="RestaurantAdministrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            var food1= _context.Foods.Find(id);
            var categoryId = food1.CategoryId;
            var category1 = _context.Categories.Find(categoryId);
            

            if (User.HasClaim("RestaurantId", category1.RestaurantId.ToString()) == false)
            {
                return Unauthorized();
            }
            if (id != food.Id)
            {
                return BadRequest();
            }

            _context.Entry(food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="RestaurantAdministrator")]
        [HttpPost]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
          if (_context.Foods == null)
          {
              return Problem("Entity set 'ApplicationContext.Food'  is null.");
          }
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // DELETE: api/Foods/5
        [Authorize (Roles ="RestaurantAdministrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food1 = _context.Foods.Find(id);
            var categoryId = food1.CategoryId;
            var category1 = _context.Categories.Find(categoryId);

            if (User.HasClaim("RestaurantId", category1.RestaurantId.ToString()) == false)
            {
                return Unauthorized();
            }
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            food.StateId = 0;
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
