using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticateController(ILogger<AuthenticateController> logger
        , IMediator mediaR) : ControllerBase
    {
        #region private properties
        private readonly ILogger<AuthenticateController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion

        #region public action methods
        [AllowAnonymous]
        [HttpPost("Register")]
        // - enregistrement d'un nouvel utilisateur
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // - appel du MediatR pour exécuter la commande d'ajout d'utilisateur
                var result = await _mediaR.Send(new AddUserCommand() { Register = register });

                if (result.Result is not null && result.Result.Succeeded)
                {
                    _logger.LogInformation("User {Login} registered successfully.", register.Login);
                    return this.Ok(result.Register);
                }
                else
                {
                    _logger.LogWarning("User {Login} registration failed: {Errors}", register.Login, string.Join(", ", result.Result!.Errors.Select(e => e.Description)));
                    return BadRequest(result.Result.Errors);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user {Login}.", register.Login);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        #endregion
    }
}
