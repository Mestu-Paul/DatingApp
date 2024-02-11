using API.Data;
using API.Entities;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork){
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles(){
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new {
                    u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                }).ToListAsync();
            return Ok(users);   
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles){
            if(string.IsNullOrEmpty(roles))return BadRequest("You must at least one role");

            var selectedRoles = roles.Split(",").ToArray();
            
            var user = await _userManager.FindByNameAsync(username);
            if(user == null)return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded)return BadRequest("Failed to add to roles");
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded)return BadRequest("Failed to remove from roles");
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration(){
            return Ok(await _unitOfWork.PhotoRepository.GetUnapprovedPhotos());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("photos-to-moderate/approve/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId){
            if(await _unitOfWork.PhotoRepository.ApprovePhoto(photoId))return Ok();
            return BadRequest("Problem approving photo");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("photos-to-moderate/delete/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            if(await _unitOfWork.PhotoRepository.DeletePhoto(photoId))return Ok();
            return BadRequest("Problem deleting photo");
        }
    }
}