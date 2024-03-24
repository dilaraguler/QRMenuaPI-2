using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuaPI.Data;
using QRMenuaPI.Models;

namespace QRMenuaPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(ApplicationContext context, UserManager<ApplicationUser> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Restaurants
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            return await _context.Restaurants.Where(r=> r.StateId == 1).ToListAsync();
        }

        // GET: api/Restaurants/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        [HttpGet("Menu/{id}")]
        public ActionResult<Restaurant> GetMenu(int id)
        {
            if(_context.Restaurants == null)
            {
                return NotFound();

            }
            var restaurant = _context.Restaurants.Include(r => r.Categories).ThenInclude(c => c.Foods).FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
            
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize (Roles ="RestaurantAdministrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurant(int id, Restaurant restaurant)
        {

            if (User.HasClaim("RestaurantId", id.ToString()) == false)
            {
                return Unauthorized();
            }

                if (id != restaurant.Id)
            {
                return BadRequest();
            }

            _context.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
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

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize (Roles ="CompanyAdministrator")]
        [HttpPost]
        public int PostRestaurant(Restaurant restaurant)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;

            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();

            applicationUser.UserName = restaurant.Name.Replace(" ", "")+restaurant.Id;
            applicationUser.Name = restaurant.Name;
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.Email = restaurant.EMail;
            applicationUser.Phone = restaurant.Phone;
            applicationUser.StateId = restaurant.StateId;
            applicationUser.CompanyId = restaurant.CompanyId;

            _userManager.CreateAsync(applicationUser, "Admin123!").Wait();
            claim = new Claim("RestaurantId", restaurant.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            _userManager.AddToRoleAsync(applicationUser, "RestaurantAdministrator").Wait();
            RestaurantUser restaurantUser = new RestaurantUser();
            restaurantUser.UserId = applicationUser.Id;
            restaurantUser.RestaurantId = restaurant.Id;

            _context.RestaurantUsers.Add(restaurantUser);
            _context.SaveChanges();

            return restaurant.Id;
        }

        // DELETE: api/Restaurants/5
        [Authorize (Roles = "CompanyAdministrator,RestaurantAdministrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            if(User.HasClaim("RestaurantId", id.ToString())== false)
            {
                return Unauthorized();
            }
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                restaurant.StateId = 0;
                _context.Restaurants.Update(restaurant);
                IQueryable<Category> categories = _context.Categories.Where(r => r.RestaurantId == id);
                foreach (Category category in categories)
                {
                    category.StateId = 0;
                    _context.Categories.Update(category);
                    IQueryable<Food> foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                    foreach (Food food in foods)
                    {
                        food.StateId = 0;
                        _context.Foods.Update(food);
                    }
                }

                IQueryable<RestaurantUser> users = _context.RestaurantUsers.Where(u => u.RestaurantId == id);
                foreach(RestaurantUser user in users)
                {
                    _context.RestaurantUsers.Remove(user);
                }

            }


            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
