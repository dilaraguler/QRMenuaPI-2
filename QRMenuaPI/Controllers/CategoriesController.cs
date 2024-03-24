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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
           
          if (_context.Categories == null)
          {
              return NotFound();
          }
            return await _context.Categories.Where(s=>s.StateId == 1).ToListAsync();
        }

        // GET: api/Categories/5
        [Authorize(Roles = "CompanyAdministrator, RestaurantAdministartor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
           
          if (_context.Categories == null)
          {
              return NotFound();
          }
            var category = await _context.Categories!.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize  (Roles ="RestaurantAdministrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
           var category1=  _context.Categories.Find(id);
            if(User.HasClaim("RestauranId", category1.RestaurantId.ToString()) == false)
            {
                return Unauthorized();
            }
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                 _context.SaveChangesAsync().Wait();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize (Roles ="RestaurantAdministrator")]
        [HttpPost]

        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
        
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [Authorize (Roles ="RestaurantAdministrator")]
        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)

        {
            var cat = _context.Categories.Find(id);
            if(User.HasClaim("RestaurantId", cat.RestaurantId.ToString()) == false)
            {
                return Unauthorized();

            }
            if (_context.Categories == null)
            {
                return NotFound();
            }

            var category = _context.Categories.Find(id);
            if (category != null)
            {

                category.StateId = 0;

                _context.Categories.Update(category);
                IQueryable<Food> foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                foreach(Food food in foods)
                {
                    food.StateId = 0;
                    _context.Foods.Update(food);
                }
            }


            _context.SaveChanges();
            return NoContent();

        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
