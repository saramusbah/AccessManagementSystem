using AccessManagementSystem.Domain.Contracts;
using AccessManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccessManagementSystem.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/doors")]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorService _doorService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DoorsController> _logger;

        public DoorsController(IDoorService doorService, RoleManager<IdentityRole> roleManager, ILogger<DoorsController> logger)
        {
            _doorService = doorService;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Get all doors.
        /// </summary>
        /// <returns>A <see cref="ResponseResult"/> containing a collection of <see cref="DoorOutputModel"/></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseResult<DoorOutputModel[]>), 200)]
        public async Task<IActionResult> GetDoorsAsync()
        {
            var doors = await _doorService.GetDoors();
            return Ok(ResponseResult.SucceededWithData(doors));
        }

        /// <summary>
        /// Create a new door.
        /// </summary>
        /// <param name="model">A model describing the new door.</param>
        /// <response code="200">A successful response with an empty <see cref="ResponseResult{TData}"/>.</response>
        [HttpPost]
        public async Task<IActionResult> PostDoorAsync(CreateDoorInputModel model)
        {
            await _doorService.CreateDoor(model);
            return Ok(ResponseResult.Succeeded());
        }

        /// <summary>
        /// Set a door to a role.
        /// </summary>
        /// <param name="model">A model describing the new door.</param>
        /// <response code="200">A successful response with an empty <see cref="ResponseResult{TData}"/>.</response>
        [HttpPut("{doorId}/role")]
        public async Task<IActionResult> SetDoorToRole(int doorId, SetDoorToRoleInputModel model)
        {
            try
            {
                var doorExists = await _doorService.DoorExists(doorId);
                if (!doorExists)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredDoor));
                }

                var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
                if (!roleExists)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredRole));
                }

                await _doorService.SetDoorRole(doorId, model.RoleName);
                return Ok(ResponseResult.Succeeded());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DoorsController.SetDoorToRole");
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseResult.Failed());
            }
        }
    }
}
