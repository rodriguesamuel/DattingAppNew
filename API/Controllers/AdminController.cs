using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
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
        private readonly IPhotoService _photoService;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                username = u.UserName,
                roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            return Ok(await _unitOfWork.PhotosRepository.GetUnapprovedPhotos());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("aprove-photo/{photoId}")]
        public async Task<ActionResult> AprovePhoto(int photoId)
        {
            Photo photo = await _unitOfWork.PhotosRepository.GetPhotoById(photoId);

            if (photo is null) return BadRequest("This photo dont exist");

            if (photo.IsApproved) return BadRequest("This is already aproved");

            photo.IsApproved = true;

            var mainPhotos = await _unitOfWork.PhotosRepository.GetMainPhotoByUserId(photo.AppUserId);

            if (mainPhotos == 0)
            {
                photo.IsMain = true;
            }

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to aprove photo");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotosRepository.GetPhotoById(photoId);

            if (photo is null) return NotFound();

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            _unitOfWork.PhotosRepository.RemovePhoto(photo);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to delete de photo");
        }
    }
}