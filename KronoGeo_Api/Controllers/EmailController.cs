using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KronoGeo_Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    // - controller de modification du mail du compte d'utilisateur
    public class EmailController (ILogger<AuthenticateController> logger
        , IMediator mediaR) : Controller
    {
        #region private properties
        private readonly ILogger<AuthenticateController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion


        #region public action method Confirmation Email
        [HttpGet("ConfirmEmail/{user}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string user, string token)
        {
            try
            {
                var result = await _mediaR.Send(new ConfirmEmailCommand() { Id = user, Token = token });
                if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok("Email confirmed successfully.");
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(result.IdentityResult.Errors);

                throw new Exception("ConfirmEmail erreur interne du handler - pas de result.Result IdentityResult manquant.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming email for user {user}.", user);
                return this.Problem("An error occurred while processing your request.");
            }
        }

        [HttpGet("ConfirmUpdateEmail/{user}/{email}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmUpdateEmail(string user, string email, string token)
        {
            try
            {
                var result = await _mediaR.Send(new ConfirmEmailCommand() { Id = user, Token = token });
                /*if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok("Email confirmed successfully.");
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(result.IdentityResult.Errors);

                throw new Exception("ConfirmEmail erreur interne du handler - pas de result.Result IdentityResult manquant.");*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming email for user {user}.", user);
                return this.Problem("An error occurred while processing your request.");
            }
            return this.Ok();
        }

        [HttpGet("ConfirmRecupEmail/{user}/{email}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmRecupEmail(string user, string email, string token)
        {
            try
            {
                var result = await _mediaR.Send(new ConfirmEmailCommand() { Id = user, Token = token });
                /*if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok("Email confirmed successfully.");
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(result.IdentityResult.Errors);

                throw new Exception("ConfirmEmail erreur interne du handler - pas de result.Result IdentityResult manquant.");*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming email for user {user}.", user);
                return this.Problem("An error occurred while processing your request.");
            }
            return this.Ok();
        }
        #endregion

        #region action method Update Identity Email
        [HttpPost("UpdateEmail")]
        [Authorize(Roles = "Admin,User,Manager")]
        // - Modification du compte utilisateur
        // - Passworld & email 
        public async Task<IActionResult> UpdateEmail([FromBody] RegisterDTO register)
        {
            try
            {
                var result = await _mediaR.Send(new UpdateUserEmailCommand() { Register = register });
                if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok(result.Register);
                }
                else
                {
                    return this.BadRequest(result.IdentityResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user {register.login}.", register.Login);
                return this.Problem("An error occurred while processing your request.");
            }
        }
        #endregion
    }
}
