using MatchMaker.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Controllers
{

    public class AdminController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("user-with-roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> GetUserWithRoles()
        {


            var users = await _userManager.Users
                .OrderBy(x => x.UserName)
                .Select(u => new
                {
                    Id = u.Id,
                    Username = u.UserName,
                    u.Email,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }


        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photos-for-moderation")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Only admin and moderators can access this");
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles))
            {
                return BadRequest("No roles provided");
            }

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("No username provided");
            }

            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove roles");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}
