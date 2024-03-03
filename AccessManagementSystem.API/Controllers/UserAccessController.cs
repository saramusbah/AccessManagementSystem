using AccessManagementSystem.Domain.Contracts;
using AccessManagementSystem.Domain.Entities;
using AccessManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccessManagementSystem.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/user-access")]
    public class UserAcccessController : ControllerBase
    {
        private readonly IAccessService _accessService;
        private readonly UserManager<User> _userManager;

        public UserAcccessController(IAccessService accessService, UserManager<User> userManager)
        {
            _accessService = accessService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get user's access history.
        /// </summary>
        /// <returns>A <see cref="ResponseResult"/> containing a collection of <see cref="AccessHistoryOutputModel"/></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseResult<AccessHistoryOutputModel[]>), 200)]
        public async Task<IActionResult> GetUserAccessHistoryAsync([FromQuery] string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser == null)
            {
                return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredUser));
            }

            var userAccessHistory = await _accessService.GetUserAccessHistoryAsync(currentUser.Id);
            return Ok(ResponseResult.SucceededWithData(userAccessHistory));
        }
    }
}
