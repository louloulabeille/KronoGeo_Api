using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Models.Model.DTO;
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
        /// <summary>
        /// action de confirmation du mail quand le compte a été créé
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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


                _logger.LogCritical("Erreur critique dans la confirmation de mail pour {user} & token : {token}", user, token);
                return this.BadRequest("Internal Error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming email for user {user}.", user);
                return this.Problem("An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Action de modification de l'adresse mail et de vérification du nouveau mail
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("ConfirmUpdateEmail/{user}/{email}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmUpdateEmail(string user, string email, string token)
        {
            try
            {
                var result = await _mediaR.Send(new ConfirmUpdateOrNotEmailCommand()
                { 
                    Id = user, 
                    Token = token,
                    Email = email,
                    Recup = true // - envoi du mail de récupération vers l'ancienne adresse mail
                });
                if (result.IdentityResult is not null && result.IdentityResult.Succeeded && result.SignInResult is null )
                {
                    return this.Ok("Email change successfully.");
                }

                // - le changement de l'adresse mail ok mais l'envoi du mail de récuperation n'a pas été envoyé
                if (result.IdentityResult is not null && result.IdentityResult.Succeeded && result.SignInResult is not null)
                {
                    return this.Ok("Email change successfully. But email recovery failed.");
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(result.IdentityResult.Errors);

                throw new Exception("Confirm update Email erreur interne du handler.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming email for user {user}.", user);
                return this.Problem("An error occurred while processing your request.");
            }
        }


        /// <summary>
        /// Action pour la récupération de l'adresse mail
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("ConfirmRecupEmail/{user}/{email}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmRecupEmail(string user, string email, string token)
        {
            try
            {
                var result = await _mediaR.Send(new ConfirmUpdateOrNotEmailCommand()
                {
                    Id = user,
                    Token = token,
                    Email = email,
                    Recup = false // - non envoi vers l'ancienne adresse mail pour la récupération
                });
                if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok("Rollback Email change successfully.");
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(result.IdentityResult.Errors);

                throw new Exception("Confirm update Email erreur interne du handler.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming email for user {user}.", user);
                return this.Problem("An error occurred while processing your request.");
            }
        }
        #endregion

        #region action method Update Identity Email
        /// <summary>
        /// action qui prépare la modification du mail par l'envoi du lien de confirmation  
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("UpdateEmail")]
        [Authorize(Roles = "Admin,User,Manager")]
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
