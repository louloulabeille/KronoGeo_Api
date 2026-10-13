using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Infrastructure.Test;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
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

        #region public action methods Register 
        [AllowAnonymous]
        [HttpPost("Register")]
        // - enregistrement d'un nouvel utilisateur
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = ModelState.ToString() });
                }

                if (string.IsNullOrEmpty(register.Email) || !EmailTest.IsValidEmail(register.Email))
                {
                    _logger.LogWarning("User registration failed: Email and PhoneNumber are both missing for {Login}.", register.Login);
                    return BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = "Invalid email address." });
                }

                // - appel du MediatR pour exécuter la commande d'ajout d'utilisateur
                var result = await _mediaR.Send(new AddUserCommand() { Register = register });

                if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    //_logger.LogInformation("User {Login} registered successfully.", register.Login);
                    return this.Ok(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.Success, Register = result.Register });
                }
                else
                {
                    if ( result.IdentityResult is null)
                    {
                        _logger.LogWarning("User {Login} registration failed: no result object.", register.Login);
                        return BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = "Internal error." });
                    }
                    _logger.LogWarning("User {Login} registration failed: {Errors}", register.Login, 
                        string.Join(", ", result.IdentityResult.Errors.Select(e => e.Description)));
                    return BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = result.IdentityResult.Errors.ToString() });
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user {Login}.", register.Login);
                return this.Problem("An error occurred while processing your request.");
            }
        }
        #endregion


        #region public action methods Login
        [AllowAnonymous]
        [HttpPost("Login")]
        // - connexion d'un utilisateur existant au niveau de l'API
        public async Task<IActionResult> Login([FromBody] RegisterDTO login)
        {
            try
            {
                if (!ModelState.IsValid && !string.IsNullOrEmpty(login.Email))
                {
                    return BadRequest(ModelState);
                }
                var result = await _mediaR.Send(new LoginUserCommand() { Register = login });
                if (result.SignInResult is not null && result.SignInResult.Succeeded)
                {
                    //_logger.LogInformation("User {Login} logged in successfully.", login.Login);
                    return Ok( new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.Success, Register = result.Register });
                }
                else if (result.SignInResult is not null && result.SignInResult.IsLockedOut)
                {

                    _logger.LogWarning("User {Login} account is locked out.", login.Login);
                    return this.BadRequest( new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.Locked, Message = "Your account is locked. Please try again later." });
                }
                else
                {
                    _logger.LogWarning("User {Login} login failed: Invalid credentials.", login.Login);
                    return this.NotFound( new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.NotFound, Message = "Invalid login or password." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user {Login}.", login.Login);
                return this.Problem ( "An error occurred while processing your request." );
            }
        }
        #endregion

        

    }
}
