using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;


namespace KronoGeo_Blazor.Components.Api
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    [ApiController]
    public class AuthBFFController(  IMediator mediaR
        , ILogger<AuthBFFController> logger ) : Controller
    {
        #region private readonly properties
        private readonly IMediator _mediaR = mediaR;
        //private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<AuthBFFController> _logger = logger;
        #endregion

        /// <summary>
        /// Bff Backends for Frontends pour mettre en place au niveau
        /// de blazor client pour l'authentification
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RegisterDTO login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = ModelState.ToString() });
                }

                // -- requete vers l'APi
                var result = await _mediaR.Send(new LoginUserCommand() { Register = login });

                if (result.IsSuccess)
                {
                    if (!string.IsNullOrEmpty(result.Register?.Token))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(result.Register.Token);
                        var roles = jwtToken.Claims.Where( r => r.Type == ClaimTypes.Role );

                        // -- Émettre le Cookie d'authentification vers le navigateur
                        var claims = new List<Claim>
                        {
                            new (ClaimTypes.Name, result.Register.Login )
                        };

                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Value));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims
                            , CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties();

                        authProperties.StoreTokens(
                        [
                            new AuthenticationToken { Name = "jwt_token", Value = result.Register.Token}
                        ]);

                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Cette ligne émet le Cookie HttpOnly de façon transparente !
                        await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                            , claimsPrincipal, authProperties );

                        result.Register.Token = string.Empty;

                        return Ok(result);
                    }
                }
                return Ok(result);

            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Erreur interne {message}", ex.Message);
                return Ok(
                    new ResponseApiAuthenticate()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = "Erreur interne, une exception a été levé."
                    });
            }
            
        }


    }
}
