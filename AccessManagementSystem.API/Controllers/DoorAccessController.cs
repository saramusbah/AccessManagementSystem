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
    [Route("api/door-access")]
    public class DoorAccessController : ControllerBase
    {
        private readonly IDoorService _doorService;
        private readonly IAccessService _accessService;
        private readonly UserManager<User> _userManager;

        public DoorAccessController(IDoorService doorService, IAccessService accessService, UserManager<User> userManager)
        {
            _doorService = doorService;
            _accessService = accessService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get a door's access history.
        /// </summary>
        /// <returns>A <see cref="ResponseResult"/> containing a collection of <see cref="AccessHistoryOutputModel"/></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{doorId}")]
        [ProducesResponseType(typeof(ResponseResult<AccessHistoryOutputModel[]>), 200)]
        public async Task<IActionResult> GetDoorAccessHistoryAsync(int doorId)
        {
            var doorExists = await _doorService.DoorExists(doorId);
            if (!doorExists)
            {
                return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredDoor));
            }

            var doorAccessHistory = await _accessService.GetDoorAccessHistoryAsync(doorId);
            return Ok(ResponseResult.SucceededWithData(doorAccessHistory));
        }


        /// <summary>
        /// open a door with a tag or remotly
        /// </summary>
        /// <response code="200">A successful response with an empty <see cref="ResponseResult{TData}"/>.</response>
        [HttpPost("{doorId}/grant-access")]
        public async Task<IActionResult> OpenDoor(int doorId, bool hasTag)
        {
            var currentUserName = _userManager.GetUserName(User);
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            if (currentUser == null)
            {
                return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredUser));
            }

            var doorExists = await _doorService.DoorExists(doorId);
            if (!doorExists)
            {
                return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredDoor));
            }

            bool isAccessGranted = await _accessService.CanGrantAccessAsync(currentUser.Id, doorId);

            if (hasTag)
            {
                await _accessService.LogTagUserDoorEventAsync(currentUser.Id, doorId, isAccessGranted);
            }
            else
            {
                await _accessService.LogRemoteUserDoorEventAsync(currentUser.Id, doorId, isAccessGranted);
            }

            return Ok(ResponseResult.Succeeded());
        }
    }
}
