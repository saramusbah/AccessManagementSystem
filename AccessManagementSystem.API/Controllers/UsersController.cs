using AccessManagementSystem.Domain.Entities;
using AccessManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AccessManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityConfig _identityConfig;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IdentityConfig identityConfig, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _identityConfig = identityConfig;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user by email and password.
        /// </summary>
        /// <param name="model">A model describing the registration parameters.</param>
        /// <returns>A <see cref="ResponseResult"/> containing a <see cref="LoginOutputModel"/>.</returns>
        /// <response code="200">A successful response with an empty <see cref="ResponseResult{TData}"/>.</response>
        /// <response code="400">
        /// If the user already exists, the error code returned is <see cref="ErrorCode.ExisitingAccountError"/>.
        ///
        /// If a validation error happens, the error code returned is <see cref="ErrorCode.ValidationError"/>. Check <see cref="ResponseResult{TData}.ValidationErrors"/>
        /// </response>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.ExisitingAccountError));
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    TokenVersion = Guid.NewGuid().ToString()
                };


                var userCreated = await _userManager.CreateAsync(user, model.Password);
                if (!userCreated.Succeeded)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.ValidationError, userCreated.Errors.Select(e => e.Code).ToArray()));
                }

                return Ok(ResponseResult.Succeeded());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UsersController.Register");
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseResult.Failed());
            }
        }

        /// <summary>
        /// Add existing user to existing role.
        /// Only Admin can add a user to a role
        /// </summary>
        /// <param name="model">A model describing the input parameters.</param>
        /// <response code="200">A successful response with an empty <see cref="ResponseResult{TData}"/>.</response>
        /// <response code="400">
        /// If the user doesn't exist, the error code returned is <see cref="ErrorCode.NotRegisteredUser"/>.
        /// If the role doesn't exist, the error code returned is <see cref="ErrorCode.NotRegisteredRole"/>.
        ///
        /// If a validation error happens, the error code returned is <see cref="ErrorCode.ValidationError"/>. Check <see cref="ResponseResult{TData}.ValidationErrors"/>
        /// </response>[HttpPost]
        [HttpPost]
        [Route("AddToRole")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseResult<LoginOutputModel>), 200)]
        [ProducesResponseType(typeof(ResponseResult<object>), 401)]
        [ProducesResponseType(typeof(ResponseResult<object>), 403)]
        public async Task<IActionResult> AddToRole([FromBody] AddUserToRoleInputModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser == null)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredUser));
                }

                var roleExists = await _roleManager.RoleExistsAsync(model.Role);

                if (!roleExists)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.NotRegisteredRole));
                }

                var addedToRole = await _userManager.AddToRoleAsync(existingUser, model.Role);
                if (!addedToRole.Succeeded)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.ValidationError, addedToRole.Errors.Select(e => e.Description).ToArray()));
                }

                existingUser.TokenVersion = Guid.NewGuid().ToString();
                var userUpdated = await _userManager.UpdateAsync(existingUser);

                if (!userUpdated.Succeeded)
                {
                    return BadRequest(ResponseResult.Failed(ErrorCode.ValidationError, userUpdated.Errors.Select(e => e.Description).ToArray()));
                }

                return Ok(ResponseResult.Succeeded());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UsersController.AddToRole");
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseResult.Failed());
            }
        }

        /// <summary>
        /// Logs a user in.
        /// </summary>
        /// <param name="model">A model describing the login parameters.</param>
        /// <returns>A <see cref="ResponseResult"/> containing a <see cref="LoginOutputModel"/>.</returns>
        /// <response code="200">A successful response with a token and user data inside <see cref="LoginOutputModel"/>.</response>
        /// <response code="401">A bad login attempt. The error code returned is <see cref="ErrorCode.InvalidLoginError"/>.</response>
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(ResponseResult<LoginOutputModel>), 200)]
        [ProducesResponseType(typeof(ResponseResult<object>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginInputModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, ResponseResult.Failed(ErrorCode.InvalidLoginError));
                }

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, ResponseResult.Failed(ErrorCode.InvalidLoginError));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(model.Email, user, roles);

                return Ok(ResponseResult.SucceededWithData(
                        LoginOutputModel.
                        CreateForLogin(token, user.Email)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UsersController.Login");
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseResult.Failed());
            }
        }


        private string GenerateJwtToken(string email, User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("TokenVersion", user.TokenVersion)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identityConfig.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_identityConfig.TokenExpiryInMinutes);

            var token = new JwtSecurityToken(
                _identityConfig.Issuer,
                _identityConfig.Audience,
                claims,
                expires: expires,
                signingCredentials: creds);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}