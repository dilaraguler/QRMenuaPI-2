using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QRMenuaPI.Models;
using SQLitePCL;
using QRMenuaPI.Data;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       

        public UsersController(ApplicationContext context,SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;


        }

        [HttpGet]
        public ActionResult<List<ApplicationUser>> GetUsers()
        {
            return _signInManager.UserManager.Users.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<ApplicationUser> GetApplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            return applicationUser;
        }

        [Authorize(Roles = "RestaurantAdministrator")]
        [HttpPut("{id}")]
        public OkResult PutApplicationUser(ApplicationUser applicationUser)
        {
            ApplicationUser existingApplicationUser = _signInManager.UserManager.FindByIdAsync(applicationUser.Id).Result;

            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;
            existingApplicationUser.UserName = applicationUser.UserName;
            _signInManager.UserManager.UpdateAsync(existingApplicationUser);
            return Ok();
        }

        [Authorize(Roles = "RestaurantAdministrator")]
        [HttpPost]
        public string PostApplicationUser(ApplicationUser applicationUser, string passWord, int restauarntId)
        {
            RestaurantUser restaurantUser = new RestaurantUser();
            _signInManager.UserManager.CreateAsync(applicationUser, passWord).Wait();

            restaurantUser.RestaurantId = restauarntId;
            restaurantUser.UserId = applicationUser.Id;

            _context.RestaurantUsers.Add(restaurantUser);
            _context.SaveChanges();

            return applicationUser.Id;
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public ActionResult DeleteApplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.StateId = 0;
            _signInManager.UserManager.UpdateAsync(applicationUser);
            return Ok();
        }

        [HttpPost("LogIn")]
        public bool LogIn(string userName, string passWord)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            Claim claim;

            if (applicationUser == null)
            {
                return false;
            }
            signInResult = _signInManager.PasswordSignInAsync(applicationUser, passWord, false, false).Result;
            if (signInResult.Succeeded == true)
            {
                claim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
                _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();
            }
            return signInResult.Succeeded;
        }

        [HttpGet("LogOut")]
        public void LogOut()
        {
            _signInManager.SignOutAsync().Wait();
        }


        [HttpPost("ReSetPassWord")]
        public void ReSetPassWord(string userName, string passWord)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return;
            }
            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            _signInManager.UserManager.AddPasswordAsync(applicationUser, passWord);
        }

        [HttpPost("PassWordReSet")]
        public string? PassWordReSet(string userName)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return null;
            }
            return _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
        }

        [HttpPost("ValidateToken")]
        public ActionResult<string?> ValidateToken(string userName, string token, string newPassWord)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassWord).Result;
            if (identityResult.Succeeded == false)
            {
                return identityResult.Errors.First().Description;
            }
            return Ok();
        }

        [HttpPost("AssignRole")]
        public void AssignRole(string userId, string roleId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole identityRole = _roleManager.FindByIdAsync(roleId).Result;

            _signInManager.UserManager.AddToRoleAsync(applicationUser, identityRole.Name).Wait();
        }
        [HttpPost("RemoveRole")]
        public void RemoveRole(string userId, string roleId)

        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole identityRole = _roleManager.FindByIdAsync(roleId).Result;

            _signInManager.UserManager.RemoveFromRoleAsync(applicationUser, identityRole.Name).Wait();
        }
    }
}